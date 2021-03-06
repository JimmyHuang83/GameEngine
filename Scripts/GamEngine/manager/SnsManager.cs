﻿using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.InteropServices;

namespace GameEngine
{
	public class SnsManager : MonoBehaviour 
	{
		public string URL_Appstore;
		public void gotoAppStore()
		{
			Application.OpenURL (URL_Appstore);
		}

		static SnsManager __Instance;
		public static SnsManager Instance
		{
			get
			{
				if (__Instance == null)
					__Instance = FindObjectOfType<SnsManager>();
				return __Instance;
			}
		}
		void Start()
		{
			DontDestroyOnLoad (this.gameObject);
			Social.localUser.Authenticate(HandleAuthenticated);
		}
		
		private void HandleAuthenticated(bool success)
		{
			Debug.Log("*** HandleAuthenticated: success = " + success);
			if(success)
			{
				//下面三行看个人需要，需要什么信息就取什么信息，这里注释掉是因为担心有的朋友没有在iTunesConnect里设置排行、成就之类的东西，运行起来可能会报错
				//Social.localUser.LoadFriends(HandleFriendsLoaded);
				//Social.LoadAchievements(HandleAchievementsLoaded);
				//Social.LoadAchievementDescriptions(HandleAchievementDescriptionsLoaded);
				Debug.Log("HandleAuthenticated success!!!");
			}
		}
		
		private void HandleFriendsLoaded(bool success)
		{
			Debug.Log("*** HandleFriendsLoaded: success = " + success);
			foreach(IUserProfile friend in Social.localUser.friends)
			{
				Debug.Log("* friend = " + friend.ToString());
			}
		}
		
		private void HandleAchievementsLoaded(IAchievement[] achievements)
		{
			Debug.Log("* HandleAchievementsLoaded");
			foreach(IAchievement achievement in achievements)
			{
				Debug.Log("* achievement = " + achievement.ToString());
			}
		}
		
		private void HandleAchievementDescriptionsLoaded(IAchievementDescription[] achievementDescriptions)
		{
			Debug.Log("*** HandleAchievementDescriptionsLoaded");
			foreach(IAchievementDescription achievementDescription in achievementDescriptions)
			{
				Debug.Log("* achievementDescription = " + achievementDescription.ToString());
			}
		}
		
		// achievements
		
		public void ReportProgress(string achievementId, double progress)
		{
			if (Social.localUser.authenticated) {
				Social.ReportProgress(achievementId, progress, HandleProgressReported);
			}
		}
		
		private void HandleProgressReported(bool success)
		{
			Debug.Log("*** HandleProgressReported: success = " + success);
		}
		
		public void ShowAchievements()
		{
			if (Social.localUser.authenticated) {
				Social.ShowAchievementsUI();
			}
		}
		
		// leaderboard
		
		public void ReportScore(string leaderboardId, long score)
		{
			#if UNITY_IPHONE
			if (Social.localUser.authenticated) {
				Social.ReportScore(score, leaderboardId, HandleScoreReported);
			}
			#endif
		}
		
		void HandleScoreReported(bool success)
		{
			Debug.Log("*** HandleScoreReported: success = " + success);
		}
		
		public void ShowLeaderboard()
		{
			if (Social.localUser.authenticated) {
				Social.ShowLeaderboardUI();
			}
		}

		public void TakeScreenShot(string jpgname)
		{
			Application.CaptureScreenshot (jpgname);
		}


		[DllImport("__Internal")]
		private static extern void UIActivityVC(string text,string imgname);
		public static void ShowUIActivityVC(string text,string imgname)
		{
			
			#if UNITY_IPHONE
			UIActivityVC(text, imgname);
			#endif
		}

		public void NativeShare()
		{
			ShowUIActivityVC ("hi my friends,tell you a nice game~ hit here:"+URL_Appstore, "theSirtetScreenShot.jpg");	
		}


	}
}
