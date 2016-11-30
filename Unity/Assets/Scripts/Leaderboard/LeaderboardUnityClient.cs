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
			_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, GetLeaderboardStandings(), _pageNumber);
		}

		internal void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, GetLeaderboardStandings(), _pageNumber);
		}

		internal void UpdateFilter(int filter)
		{
			_pageNumber = 0;
			_filter = (LeaderboardFilterType)filter;
			_leaderboardInterface.ShowLeaderboard(_leaderboard, _filter, GetLeaderboardStandings(), _pageNumber);
		}

		private void GetLeaderboard()
		{
			if (SUGARManager.CurrentUser != null)
			{
				_leaderboard = SUGARManager.Client.Leaderboard.Get(_leaderboardToken, SUGARManager.GameId);
			}
		}

		private IEnumerable<LeaderboardStandingsResponse> GetLeaderboardStandings()
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
				var standings = SUGARManager.Client.Leaderboard.CreateGetLeaderboardStandings(request).ToList();
				return standings;
			}
			return Enumerable.Empty<LeaderboardStandingsResponse>();
		}
	}
}