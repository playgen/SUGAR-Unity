﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : MonoBehaviour
	{
		private LeaderboardResponse _leaderboard;

		internal LeaderboardResponse CurrentLeaderboard
		{
			get { return _leaderboard; }
		}

		[SerializeField]
		private LeaderboardUserInterface _leaderboardInterface;

		internal void CreateInterface(Canvas canvas)
		{
			bool inScene = _leaderboardInterface.gameObject.scene == SceneManager.GetActiveScene();
			if (!inScene)
			{
				var newInterface = Instantiate(_leaderboardInterface, canvas.transform, false);
				newInterface.name = _leaderboardInterface.name;
				_leaderboardInterface = newInterface;
			}
			_leaderboardInterface.gameObject.SetActive(false);
		}

		public void Display(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			GetLeaderboard(token);
			GetLeaderboardStandings(filter, 0, result =>
			{
				if (result != null)
				{
					var standings = result.ToList();
					_leaderboardInterface.Display(filter, standings);
				}
			});
		}

		private void GetLeaderboard(string token)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Client.Leaderboard.GetAsync(token, SUGARManager.GameId,
				response =>
				{
					_leaderboard = response;
				},
				exception =>
				{
					string error = "Failed to get leaderboard. " + exception.Message;
					Debug.LogError(error);
				});
			}
		}

		internal void GetLeaderboardStandings(LeaderboardFilterType filter, int pageNumber, Action<IEnumerable<LeaderboardStandingsResponse>> result)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = _leaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = filter,
					PageLimit = _leaderboardInterface.GetPossiblePositionCount() + 1,
					PageOffset = pageNumber
				};

				SUGARManager.Client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					result(response.ToList());
				},
				exception =>
				{
					string error = "Failed to get leaderboard standings. " + exception.Message;
					Debug.LogError(error);
					result(Enumerable.Empty<LeaderboardStandingsResponse>());
				});
			}
			result(Enumerable.Empty<LeaderboardStandingsResponse>());
		}
	}
}