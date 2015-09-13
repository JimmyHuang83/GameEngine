using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
namespace GameEngine
{

	public class DataRow
	{
		protected Dictionary<string,string> tableDic;
		protected string[] keyIDList;
		public DataRow(){tableDic = new Dictionary<string, string> ();}
		public DataRow(Dictionary<string,string> dic,string[] keys)
		{
			keyIDList = keys;
			if (dic == null) tableDic = new Dictionary<string, string> ();
			else  tableDic = dic;
		}
		
		public DataRow(DataRow dr)
		{
			keyIDList = dr.keyIDList;
			tableDic = dr.tableDic;
		}
		
		public int Count { get { return tableDic.Count; }}

		public string this[int index]
		{
			get{
				if (index < keyIDList.Length)
					return getValue(keyIDList[index]);
				return null;
			}
		}
		
		public string getValue(string key)
		{
			string value = null;
			if (tableDic != null && tableDic.TryGetValue(key, out value))
			{
				return value;
			}
			return value;
		}
		
		string[]  _keys;
		public string[] keys
		{
			get 
			{
				if (keyIDList != null && keyIDList.Length > 0)
					return keyIDList;
				if (_keys == null) 
				{
					_keys = new string[tableDic.Keys.Count];
					tableDic.Keys.CopyTo(_keys, 0);
				}
				return _keys;
			}
		}
		public T getValue<T>(int index)
		{
			return GameUtils.ParseValue<T>(this[index]);
		}
		
		public T getValue<T>(string key)
		{
			string value = getValue (key);
			return GameUtils.ParseValue<T> (value);
		}
	}
	
	public interface IParseDataRow
	{
		void ParseFromDataRow(DataRow dr);
	}

	public class TableManager  
	{

		public static bool isInit = false;
		public static void Initlizatize(TextAsset[] assertList, bool readHead = true)
		{
			string tName = "";
			for(int idx = 0; idx < assertList.Length; idx++)
			{
				tName = assertList[idx].name;
				List<DataRow> list = new List<DataRow>();
				using (StringReader f = new StringReader(assertList[idx].text))
				{
					string line;
					bool isReadTitle = false;
					string[] properties = new string[1];
					while ((line = f.ReadLine()) != null)
					{
						try
						{
							string[] ss = line.Split(new string[] { "\t" }, System.StringSplitOptions.None);
							if(isReadTitle == false)
							{
								if (readHead)
								{
									properties = ss;
									isReadTitle = true;
									continue;
								}
								else
								{
									properties = new string[ss.Length];
									for(int k = 0; k < properties.Length; k++)
										properties[k] = "Field"+k;
								}
							}
							Dictionary<string,string> rowDict = new Dictionary<string, string>();
							for (int i = 0; i < ss.Length; i++)
							{
								rowDict[properties[i]] = ss[i];
							}
							list.Add(new DataRow(rowDict,properties));

						}
						catch (System.Exception ex)
						{
							Debug.Log("exception," + ex.ToString());
						}
					}
				}
				tableDictionary[tName] =  list;
			}
			isInit = true;
		}



		//////////////////////////////////////////////Extener API////////////////////////////////////////////////////////////////////
		/// 
		/// 
		/// 
		static Dictionary<string,List<DataRow>> tableDictionary = new Dictionary<string, List<DataRow>> ();

		public static List<T> SearchRows<T>(string tableName) where T : IParseDataRow,new()
		{
			List<DataRow> list = SearchRows (tableName);
			List<T> retList = new List<T> ();
			foreach(DataRow dr in list)
			{
				T item = new T();
				item.ParseFromDataRow(dr);
				retList.Add(item);
			}
			return retList;
		}

		public static List<DataRow> SearchRows(string tableName)
		{
			List<DataRow> list;
			if (tableDictionary.TryGetValue (tableName, out list))
			{
				return list;
			}
			return list;

		}
		public static ElemType[][] SearchRowsByElemType<ElemType>(string tableName)
		{
			List<DataRow> rows = SearchRows (tableName);
			ElemType[][] list = new ElemType[rows.Count][];
			for(int i = 0; i < rows.Count; i++)
			{
				list[i] = new ElemType[rows[i].Count];
				for (int j = 0; j < rows[i].Count; j++)
				{
					list[i][j] = rows[i].getValue<ElemType>(j);
				}
			}
			return list;
		}

		public static ElemType[][] SearchRowsByElemType<ElemType>(string tableName, string where)
		{
			List<DataRow> rows = SearchRows (tableName, where);
			ElemType[][] list = new ElemType[rows.Count][];
			for(int i = 0; i < rows.Count; i++)
			{
				list[i] = new ElemType[rows[i].Count];
				for (int j = 0; j < rows[i].Count; j++)
				{
					list[i][j] = rows[i].getValue<ElemType>(j);
				}
			}
			return list;
		}

		public static List<T> SearchRows<T>(string tableName, string where) where T : IParseDataRow,new()
		{
			List<DataRow> list = SearchRows (tableName);
			List<DataRow> rows = list.FindAll (x => parseWhere (x, where));
			List<T> retList = new List<T> ();
			foreach(DataRow dr in rows)
			{
				T item = new T();
				item.ParseFromDataRow(dr);
				retList.Add(item);
			}
			return retList;
		}

		public static List<DataRow> SearchRows(string tableName, string where)
		{
			List<DataRow> list = SearchRows (tableName);
			return list.FindAll (x => parseWhere (x, where));
		}


		public static T SearchFirstRow<T>(string tableName, string where, Dictionary<string,DataRow> cache = null) where T : IParseDataRow,new()
		{
			string kk = tableName + "_" + where;
			DataRow dr = null;
			if (cache != null && cache.ContainsKey (kk))
				dr = cache [kk];
			else
				dr = SearchFirstRow (tableName, where);
			if (dr != null)
			{
				if (cache != null)
					cache[kk] = dr;
				T info = new T();
				info.ParseFromDataRow(dr);
				return info;
			}
			return (T)(object)null;
		}

		public static DataRow SearchFirstRow(string tableName, string where, Dictionary<string,DataRow> cache = null)
		{
			
			string kk = tableName + "_" + where;
			DataRow dr = null;
			if (cache != null && cache.ContainsKey (kk)) 
			{
				dr = cache [kk];
			}
			else
			{
				List<DataRow> list = SearchRows (tableName);
				dr = list.Find (x => parseWhere (x, where));
				if (cache != null)
					cache [kk] = dr;
			}
			return dr;
		}

		static string[] getWherePaires(string where)
		{
			where = where.Replace ("{", "");
			where = where.Replace ("}", "");
			string[] arry = where.Split (new string[]{","}, StringSplitOptions.None);
			arry = arry [0].Split (new string[]{":"}, StringSplitOptions.None);
			return arry;
		}

		public static T SearchFirstRowAndField<T>(string tableName, string where, string fieldName, Dictionary<string,object> cache = null)
		{
			string kk = tableName + "_" + where + "_" + fieldName;
			if (cache != null && cache.ContainsKey(kk))
				return (T)cache[kk];
			DataRow drow = SearchFirstRow (tableName, where);
			T retv;
			if (drow != null) 
			{
				retv = drow.getValue<T> (fieldName);
			}
			else
			{
				retv = GameMath.ConvertNullType<T>();
			}
			if (cache != null)
				cache [kk] = retv;
			return retv;
		}

		static bool parseWhere(DataRow row, string where)
		{
			bool returnBool = true;
			where = where.Replace ("{", "");
			where = where.Replace ("}", "");
			string[] arry = where.Split (new string[]{","}, StringSplitOptions.None);
			for (int i = 0; i < arry.Length; i++)
			{
				string[] tmpAry = arry[i].Split (logicStrs, StringSplitOptions.None);

				if (tmpAry.Length < 1)
				{
					returnBool = false;
					continue;
				}
				returnBool &= judgeLogic(row.getValue(tmpAry[0]),tmpAry[1],arry[i]);
			}

			return returnBool;
		}

		static string[] logicStrs = new string[]{":","<=",">=","<",">"};
		static bool judgeLogic(string rowVal, string targetVal, string whereStr)
		{
			bool ret = false;
			string logicStr = null;
			foreach(string str in logicStrs)
			{
				if (whereStr.IndexOf(str) > 0)
				{
					logicStr = str;
					break;
				}
			}
			
			switch(logicStr)
			{
			case ":":
				ret = rowVal==targetVal;
				break;
			case "<=":
				ret = float.Parse(rowVal) <= float.Parse(targetVal);
				break;
			case ">=":
				ret = float.Parse(rowVal) >= float.Parse(targetVal);
				break;
			case "<":
				ret = float.Parse(rowVal) < float.Parse(targetVal);
				break;
			case ">":
				ret = float.Parse(rowVal) > float.Parse(targetVal);
				break;
			default:
				ret = false;
				break;
			}
			return ret;
		}
	}

}
