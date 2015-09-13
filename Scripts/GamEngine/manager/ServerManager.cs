using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace GameEngine
{
	public class ServerManager : MonoBehaviour {
		public string serverRequestURL = "http://54.169.102.54/web_app?act=interface.call";
		public string serverAssetURL="http://54.169.102.54/web_app/res/";
		public delegate void NetCallbackDelegate(LitJson.JsonData data,object ver = null);
		public event NetCallbackDelegate OnNetResponse;

		[DllImport("__Internal")]
		private static extern string advertisingIdentifier();
		static ServerManager __Instance;
		public static ServerManager Instance
		{
			get
			{
				if (__Instance == null)
					__Instance = FindObjectOfType<ServerManager>();
				return __Instance;
			}
		}

		System.TimeSpan _svrTimeOffset = new System.TimeSpan();
		public System.TimeSpan svrTimeOffset
		{
			get {return _svrTimeOffset;}
		}

		public string platform
		{
			get
			{
				return SystemInfo.operatingSystem;
			}
		}

		public string Udid
		{
			get 
			{
				string uuid = SystemInfo.deviceUniqueIdentifier;
				#if UNITY_IPHONE
				if( Application.platform == RuntimePlatform.IPhonePlayer )
				{
					if (int.Parse(iPhoneSettings.systemVersion.Split(new string[]{"."},System.StringSplitOptions.None)[0]) > 6)
					{
						//ios7 use IDFA//
						uuid = advertisingIdentifier();
					}
					else
					{
						//Mac//
						NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
						foreach (NetworkInterface adaper in nice)
						{
							if (adaper.Description == "en0")
							{
								uuid = adaper.GetPhysicalAddress().ToString();
								break;
							}
							else
							{
								uuid = adaper.GetPhysicalAddress().ToString();
								
								if (uuid != "")
								{
									break;
								}
							}
						}
					}
				}
				#endif
				return uuid;
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void CheckResourceVersion(string appID, NetCallbackDelegate onResult)
		{
			string verstr = PlayerPrefs.GetString ("ClientAssetVerNo", "1.0.0");
			StartCoroutine (requestServer(appID,verstr,onResult));
		}

		IEnumerator requestServer(string appID,string verstr, NetCallbackDelegate onResult)
		{
			LitJson.JsonData j = new LitJson.JsonData ();
			j ["appid"] = appID;
			j ["cmdid"] = 1;
			j ["udid"] = Udid;
			WWW www = NetUtils.pack2Get (serverRequestURL, j);
			yield return www;
			LitJson.JsonData ret = NetUtils.unpackFromJson (www);
			if (ret != null) {
				long clientNo = MakeVerNo (verstr);
				long serNo = MakeVerNo (ret["data"] ["version"].ToString ());
				if (serNo >= clientNo && ret["data"] ["filelist"].Count > 0) {
					string asseturl = serverAssetURL + ret["data"] ["filelist"] [0];
					www = NetUtils.pack2Get (asseturl);
					yield return www;
					LitJson.JsonData dat = NetUtils.unpackFromJson (www);
					onResult (dat, serNo);
				}
				else
				{
					onResult (null);
				}
			}
			else
			{	
				onResult (null);
			}
		}

		IEnumerator requestServer2(WWW www, NetCallbackDelegate onResult)
		{
			yield return www;
			LitJson.JsonData ret = NetUtils.unpackFromJson (www);
			onResult (ret);
		}

		public void call(int commandID, LitJson.JsonData args, NetCallbackDelegate onResult)
		{
			string argsStr = args.ToJson ();
			LitJson.JsonData paramm = new LitJson.JsonData ();
			paramm ["platform"] = platform;
			paramm ["udid"] = Udid;
			paramm ["cmd"] = commandID;
			paramm ["data"] = argsStr;
			WWW www = NetUtils.pack2Post (serverRequestURL, paramm);
			StartCoroutine (requestServer2(www, onResult));
		}

		public string unMakeVerNo(long ver)
		{
			string str;
			int a = (int)(ver / Mathf.Pow (1000, 2));
			int b = (int) (ver / Mathf.Pow (1000, 1) % 1000);
			int c = (int)(ver % Mathf.Pow (1000, 2)%1000);
			str = a+"."+b+"."+c;
			return str;
		}

		//1.1.3; 10103  1001003
		public long MakeVerNo(string str)
		{
			int[] intary = GameUtils.SplitString (str,".").ToArray();

			long val = 0;
			for(int i = 0; i < intary.Length; i++)
			{
				val += intary[i]*(int)Mathf.Pow(1000,intary.Length-i-1);
			}
			return val;
		}
	}
}
