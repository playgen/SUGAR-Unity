using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : MonoBehaviour
	{
		private string _leaderboardToken;

		private LeaderboardResponse _leaderboard;

		private int _pageNumber;

		private LeaderboardFilterType _filter;

		[SerializeField]
		private LeaderboardUserInterface _leaderboardInterface;

		public void Display(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			SetLeaderboard(token, filter);
		}

		internal void SetLeaderboard(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			_leaderboardToken = token;
			_pageNumber = 0;
			_filter = filter;
			GetLeaderboard();
			GetLeaderboardStandings(result =>
			{
				var standings = result.ToList();
				if (standings.Count != 0)
				{
					_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, standings, _pageNumber);
				}
			});
		}

		internal void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			GetLeaderboardStandings(result =>
			{
				var standings = result.ToList();
				if (standings.Count != 0)
				{
					_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, standings, _pageNumber);
				}
			});
		}

		internal void UpdateFilter(int filter)
		{
			_pageNumber = 0;
			_filter = (LeaderboardFilterType)filter;
			GetLeaderboardStandings(result =>
			{
				var standings = result.ToList();
				if (standings.Count != 0)
				{
					_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, standings, _pageNumber);
				}
			});
		}

		private void GetLeaderboard()
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Client.Leaderboard.GetAsync(_leaderboardToken, SUGARManager.GameId,
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

		private void GetLeaderboardStandings(Action<IEnumerable<LeaderboardStandingsResponse>> result)
		{
			if (SUGARManager.CurrentUser != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = _leaderboardToken,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = _filter,
					PageLimit = _leaderboardInterface.GetPossiblePositionCount() + 1,
					PageOffset = _pageNumber
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