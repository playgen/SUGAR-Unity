﻿using System;

using UnityEngine;
using System.Collections.Generic;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseLeaderboardListUserInterface _leaderboardListInterface;

		private readonly List<List<LeaderboardResponse>> _leaderboards = new List<List<LeaderboardResponse>>();

		internal List<List<LeaderboardResponse>> leaderboards => _leaderboards;

		public bool IsActive => _leaderboardListInterface && _leaderboardListInterface.gameObject.activeInHierarchy;

		internal void CreateInterface(Canvas canvas)
		{
			if (_leaderboardListInterface)
			{
				bool inScene = _leaderboardListInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_leaderboardListInterface.gameObject, canvas.transform, false);
					newInterface.name = _leaderboardListInterface.name;
					_leaderboardListInterface = newInterface.GetComponent<BaseLeaderboardListUserInterface>();
				}
				_leaderboardListInterface.gameObject.SetActive(false);
			}
		}

		public void DisplayList(ActorType filter = ActorType.User)
		{
			if (_leaderboardListInterface)
			{
				GetLeaderboards(success =>
				{
					_leaderboardListInterface.Display(filter, success);
				});
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_leaderboardListInterface.gameObject);
			}
		}

		private void GetLeaderboards(Action<bool> success)
		{
			_leaderboards.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(SUGARManager.GameId,
				response =>
				{
					var result = response.ToList();
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						var at = actorType;
						_leaderboards.Add(result.Where(lb => lb.ActorType == at).ToList());
					}
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard list. " + exception.Message;
					Debug.LogError(error);
					for (int i = 0; i < Enum.GetValues(typeof(ActorType)).Length; i++)
					{
						_leaderboards.Add(new List<LeaderboardResponse>());
					}
					success(false);
				});
			}
			else
			{
				for (int i = 0; i < Enum.GetValues(typeof(ActorType)).Length; i++)
				{
					_leaderboards.Add(new List<LeaderboardResponse>());
				}
				success(false);
			}
		}
	}
}
