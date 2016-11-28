using System;
using System.Collections.Generic;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine;

namespace SUGAR.Unity
{
	public class LeaderboardUnityClient : MonoBehaviour
	{
		private string _leaderboardToken;

		private string _name;

		private int _pageNumber;

		private LeaderboardFilterType _filter;

		[SerializeField]
		private LeaderboardUserInterface _leaderboardInterface;

		public void SetLeaderboard(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			_leaderboardToken = token;
			_pageNumber = 0;
			_filter = filter;
			GetLeaderboardName();
			_leaderboardInterface.ShowLeaderboard(_name, _filter, GetLeaderboard(), _pageNumber);
		}

		public void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			_leaderboardInterface.ShowLeaderboard(_name, _filter, GetLeaderboard(), _pageNumber);
		}

		public void UpdateFilter(int filter)
		{
			_pageNumber = 0;
			_filter = (LeaderboardFilterType)filter;
			_leaderboardInterface.ShowLeaderboard(_name, _filter, GetLeaderboard(), _pageNumber);
		}

		private void GetLeaderboardName()
		{
			_name = SUGARManager.Client.Leaderboard.Get(_leaderboardToken, SUGARManager.GameId).Name;
		}

		private IEnumerable<LeaderboardStandingsResponse> GetLeaderboard()
		{
			var request = new LeaderboardStandingsRequest
			{
				LeaderboardToken = _leaderboardToken,
				GameId = SUGARManager.GameId,
				ActorId = SUGARManager.CurrentUser.Id,
				LeaderboardFilterType = _filter,
				PageLimit = 10,
				PageOffset = _pageNumber
			};
			var standings = SUGARManager.Client.Leaderboard.CreateGetLeaderboardStandings(request);
			return standings;
		}
	}
}