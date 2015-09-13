using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
namespace GameEngine
{
	public class NetUtils
	{
		public static string accessToken;
		public static string TokenIDKey = "tokenid";
		public  static WWW pack2Get(string path, JsonData jsdata)
		{
			if (!string.IsNullOrEmpty(accessToken) && !jsdata.Keys.Contains(TokenIDKey))
				jsdata[TokenIDKey] = accessToken;
			string str = jsdata.ToJson ();
			int cid = jsdata.Keys.Contains ("cmdid") ? int.Parse (jsdata ["cmdid"].ToString ()) : -1;
			return pack2Get(path, str, true,cid);
		}

		public static WWW pack2Get(string url){ return pack2Get (url, null);} 
		public static WWW pack2Get(string url, string paramStr){ return pack2Get (url, paramStr, true);}
		public static WWW pack2Get(string url, string paramStr, bool base64){ return pack2Get (url, paramStr, base64, -1);}
		public static WWW pack2Get(string url, string paramStr, bool base64, int cmdID)
		{
			WWW www = null;
			if (string.IsNullOrEmpty(paramStr))
			{
				www = new WWW (url);
			}
			else
			{
				string jsparams = paramStr;
				if (base64)
				{
					byte[] bytes = System.Text.Encoding.UTF8.GetBytes (jsparams);
					jsparams = System.Convert.ToBase64String (bytes);
				}
				string getchar = url.IndexOf("?") > 0 ? "&" : "?";
				if (cmdID != -1)
					url = url+getchar+"cmd="+cmdID+"&data="+jsparams;
				else
					url = url+getchar+"data="+jsparams;
				www = new WWW(url);
			}
			return www;
		}
		
		public static WWW pack2Post(string path, JsonData param)
		{
			string str = param.ToJson ();
			return pack2Post (path,str, true);
		}
		
		public static WWW pack2Post(string url, string paramStr, bool base64 = true)
		{
			Dictionary<string,string> headerDict = new Dictionary<string,string> ();
			if (!string.IsNullOrEmpty (accessToken))
				headerDict [TokenIDKey] = accessToken;
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes (paramStr);
			byte[] bys;
			if (base64)
			{
				string jsparams = System.Convert.ToBase64String (bytes);
				bys = System.Text.Encoding.UTF8.GetBytes (jsparams);
			}
			else
			{
				bys = bytes;
			}
			bys = bys.Length <= 0 ? new byte[]{0} : bys;
			WWW www = new WWW (url, bys, headerDict);
			return www;
		}
		
		public static LitJson.JsonData unpackFromJson(WWW www,bool base64 = false)
		{
			if (!www.isDone)
				return null;
			if (string.IsNullOrEmpty (www.text))
				return null;
			string jsonStr = www.text;
			if (base64) 
			{
				byte[] bytes = System.Convert.FromBase64String (jsonStr);
				jsonStr = System.Text.Encoding.UTF8.GetString (bytes);
			}
			return LitJson.JsonMapper.ToObject(jsonStr);
		}
	}
}
