﻿using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Unity.Utilities;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	[RequireComponent(typeof(AccountUnityClient))]
	[RequireComponent(typeof(AchievementUnityClient))]
	[RequireComponent(typeof(LeaderboardUnityClient))]
	[RequireComponent(typeof(LeaderboardListUnityClient))]
	[RequireComponent(typeof(ResponseHandler))]
	[RequireComponent(typeof(Logging))]
	public class SUGARUnityManager : MonoBehaviour
	{
		[SerializeField]
		private string _baseAddress;
		[SerializeField]
		private int _gameId;
		private Canvas _canvas;
		[SerializeField]
		private bool _useAchievements = true;
		[SerializeField]
		private bool _useLeaderboards = true;

		private void Awake()
		{
			if (SUGARManager.Register(this))
			{
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
			SUGARManager.Client = new SUGARClient(_baseAddress);
			SUGARManager.GameId = _gameId;
			SUGARManager.account = GetComponent<AccountUnityClient>();
			SUGARManager.achievement = _useAchievements ? GetComponent<AchievementUnityClient>() : null;
			SUGARManager.leaderboard = _useLeaderboards ? GetComponent<LeaderboardUnityClient>() : null;
			SUGARManager.gameLeaderboard = _useLeaderboards ? GetComponent<LeaderboardListUnityClient>() : null;
			_canvas = GetComponentInChildren<Canvas>();
			GetComponent<AccountUnityClient>().CreateInterface(_canvas);
			if (_useLeaderboards)
			{
				GetComponent<LeaderboardListUnityClient>().CreateInterface(_canvas);
				GetComponent<LeaderboardUnityClient>().CreateInterface(_canvas);
			}
			if (_useAchievements)
			{
				GetComponent<AchievementUnityClient>().CreateInterface(_canvas);
			}
		}
	}
}