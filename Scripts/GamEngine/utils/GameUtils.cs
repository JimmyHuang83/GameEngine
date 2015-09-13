using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace GameEngine
{
	public class GameUtils
	{

		public static void PlayVfx(GameObject effobj, Vector3 worldPos)
		{
			if (effobj == null)
				return;
			GameObject go = GameObject.Instantiate (effobj) as GameObject;
			go.transform.position = worldPos;
			ParticleSystem ps = go.GetComponentInChildren<ParticleSystem> ();
			float duration = 3f;
			if (ps != null)
			{
				duration = ps.duration;
				ps.Play();
			}
			go.AddComponent<AutoDestroy> ().duration = duration;
		}

		public static T Clone<T>(string prefabPath, Transform parentCon) where T : Component
		{

			string path = prefabPath;
			GameObject prefabObj = Resources.Load (path) as GameObject;
			return Clone<T> (prefabObj, parentCon);
		}

		public static T Clone<T>(GameObject prefab, Transform parentCon) where T : Component
		{
			GameObject go = GameObject.Instantiate (prefab) as GameObject;
			go.transform.parent = parentCon;
			go.transform.localScale = Vector3.one;
			go.transform.localRotation = Quaternion.identity;
			return go.GetComponent<T> ();
		}

		public static void DestroyGameObjets(IList<GameObject> list)
		{
			while(list.Count > 0)
			{
				GameObject o = list[0];
				list.RemoveAt(0);
				GameObject.DestroyImmediate(o);
			}
		}

		public static void DestroyGameObjets<T>(IList<T> list) where T:Component
		{
			while(list.Count > 0)
			{
				GameObject o = list[0].gameObject;
				list.RemoveAt(0);
				if (o != null)
					GameObject.DestroyImmediate(o);
			}
		}

		public static void DestroyGameObjets(IList<Transform> list)
		{
			while(list.Count > 0)
			{
				Transform o = list[0];
				list.RemoveAt(0);
				GameObject.DestroyImmediate(o.gameObject);
			}
		}

		public static List<int> SplitString(string str, string splitChar = ",")
		{
			List<int> intary = new List<int> ();
			string[] ary = str.Split (new string[]{splitChar},System.StringSplitOptions.None);
			for(int i = 0; i < ary.Length; i++)
			{
				if (!string.IsNullOrEmpty(ary[i]))
					intary.Add(int.Parse(ary[i]));
			}
			return intary;
		}
		
		public static string JoinToString(int[] ary,string splitchar = ",")
		{
			string str = "";
			for(int i = 0; i < ary.Length; i++)
			{
				str += ary[i];
				if (i < ary.Length-1)
					str += splitchar;
			}
			return str;
		}
		
		public static string JoinToString(List<int> ary,string splitchar = ",")
		{
			string str = "";
			for(int i = 0; i < ary.Count; i++)
			{
				str += ary[i];
				if (i < ary.Count-1)
					str += splitchar;
			}
			return str;
		}
		
		public static bool CopyObjectValue (object source, object target)
		{
			if (source == null || target == null)
				return false;
			if (source.GetType () != target.GetType ())
				return false;
			FieldInfo[] fields = source.GetType ().GetFields();
			foreach(FieldInfo field in fields)
			{
				field.SetValue(target, field.GetValue(source));
			}
			return false;
		}

		public static string formatDigtail(int num)
		{
			return num <= 9 ? "0" + num : num.ToString ();
		}

		public static T ParseValue<T>(string value)
		{
			object obj = null;
			int i = 0;
			string[] sss;
			if (typeof(T) == typeof(int))
			{
				obj = int.Parse(value);
			}
			else if (typeof(T) == typeof(ulong))
			{
				double v = 0;
				if (chkPowNum(value,out v))
				{
					obj = (ulong)v;
				}
				else
					obj = ulong.Parse(value);
			}
			else if (typeof(T) == typeof(double))
			{
				double v = 0;
				if (chkPowNum(value,out v))
				{
					obj = v;
				}
				else
					obj = double.Parse(value);
			}
			else if (typeof(T) == typeof(int[]))
			{
				int[] intAry;
				if (value == "")
				{
					intAry = new int[0];
				}
				else
				{
					sss = value.Split(new string[]{","},System.StringSplitOptions.None);
					intAry = new int[sss.Length];
					for(i = 0; i < sss.Length; i++)
					{
						try
						{
							intAry[i] = int.Parse(sss[i]);
						}catch(System.Exception ee)
						{
							Debug.Log(ee.Message);
						}
					}
				}
				obj = intAry;
			}
			else if (typeof(T) == typeof(float))
			{
				value = string.IsNullOrEmpty(value) ? "0" : value;
				obj = float.Parse(value);
			}
			else if (typeof(T) == typeof(float[]))
			{
				float[] floatAry;
				if (value == "")
				{
					floatAry = new float[0];
				}
				else
				{
					sss = value.Split(new string[]{","},System.StringSplitOptions.None);
					floatAry = new float[sss.Length];
					for(i = 0; i < sss.Length; i++)
					{
						floatAry[i] = float.Parse(sss[i]);
					}
				}
				obj = floatAry;
			}
			else if (typeof(T) == typeof(string[]))
			{
				string[] stringAry;
				if (value == "")
				{
					stringAry = new string[0];
				}
				else
				{
					sss = value.Split(new string[]{","},System.StringSplitOptions.None);
					stringAry = new string[sss.Length];
					for(i = 0; i < sss.Length; i++)
					{
						stringAry[i] = sss[i].ToString();
					}
				}
				obj = stringAry;
			}
			else if (typeof(T) is System.Boolean)
			{
				if (value.ToLower() == "false" || value.ToLower() == "true")
				{
					obj = value.ToLower() == "false" ? false : true;
				}
				obj = System.Boolean.Parse(value);
			}
			else
				obj = System.Convert.ChangeType(value,typeof(T));
			
			return (T)obj;
		}
		
		static bool chkPowNum(string value, out double val)
		{
			val = 0;
			int ix = value.IndexOf("E");
			if (ix > 0)
			{
				string s0 = value.Substring(0,ix);
				string s1 = value.Substring(ix+2);
				int p = int.Parse(s1);
				float a = float.Parse(s0);
				val = a*System.Math.Pow(10, p);
				return true;
			}
			return false;
		}

	}
}
