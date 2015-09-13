using UnityEngine;
using System.Collections;
using LitJson;
using GoogleMobileAds;
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;

namespace GameEngine
{
	public class AdManager : MonoBehaviour 
	{
		public string bannerID = "ca-app-pub-5329380353260701/1640831876";
		public string screenAdID = "ca-app-pub-5329380353260701/5452995478";

		public bool bannerOnTop;
		BannerView bview;
		InterstitialAd screenAd;
		// Use this for initialization
		void Start () 
		{
			DontDestroyOnLoad (this.gameObject);
			Init ();
		}

		static AdManager __Instance;
		public static AdManager Instance
		{
			get
			{
				if (__Instance == null)
					__Instance = FindObjectOfType<AdManager>();
				return __Instance;
			}
		}

		// Update is called once per frame
		void Update () {
		
		}

		public void Init()
		{
			if (!string.IsNullOrEmpty(bannerID))
			{
				bview = new BannerView (bannerID, GoogleMobileAds.Api.AdSize.Banner, bannerOnTop ? GoogleMobileAds.Api.AdPosition.Top : GoogleMobileAds.Api.AdPosition.Bottom);
				bview.LoadAd (new AdRequest.Builder().Build());
				bview.AdOpened+= HandleAdOpened;
			}

			loadScreenAd ();
		}

		void HandleAdOpened (object sender, System.EventArgs e)
		{
			if (isCloseAd)
				bview.Hide ();
		}

		public void showScreenAd()
		{
			if (screenAd != null && screenAd.IsLoaded())
			{
				screenAd.Show ();
				loadScreenAd ();
			}
		}

		void loadScreenAd()
		{
			if (screenAd != null)
				screenAd.Destroy ();
			if (!string.IsNullOrEmpty(screenAdID))
			{
				screenAd = new InterstitialAd (screenAdID);
				screenAd.LoadAd (new AdRequest.Builder ().Build ());
				screenAd.AdLoaded += (object sender, System.EventArgs e) => Debug.Log ("Screend Ad Loaded.....");
			}
		}

		public void showAds()
		{
			isCloseAd = false;
			if(bview != null)
				bview.Show();
		}

		bool isCloseAd = false;
		public void closeAds()
		{
			isCloseAd = true;
			if(bview != null)
				bview.Hide ();
		}
	}
}
