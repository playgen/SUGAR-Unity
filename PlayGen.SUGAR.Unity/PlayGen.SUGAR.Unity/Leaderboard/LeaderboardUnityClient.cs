using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : BaseUnityClient<BaseLeaderboardInterface>
	{
		[SerializeField]
		private int _positionCount;
		[SerializeField]
		private LeaderboardFilterType _currentFilter = LeaderboardFilterType.Top;
		private LeaderboardResponse _currentLeaderboard;
		private List<LeaderboardStandingsResponse> _currentStandings = new List<LeaderboardStandingsResponse>();

		public LeaderboardFilterType CurrentFilter => _currentFilter;
		public LeaderboardResponse CurrentLeaderboard => _currentLeaderboard;
		public List<LeaderboardStandingsResponse> CurrentStandings => _currentStandings;

		public int PositionCount => _positionCount;

		public void Display(string token, LeaderboardFilterType filter, int pageNumber = 0)
		{
				_currentFilter = filter;
				SUGARManager.unity.StartSpinner();
				GetLeaderboard(token, success =>
				{
					if (success)
					{
						GetLeaderboardStandings(pageNumber, result =>
						{
							SUGARManager.unity.StopSpinner();
							if (_interface)
							{
								_interface.Display(result);
							}
						});
					}
					else
					{
						SUGARManager.unity.StopSpinner();
						if (_interface)
						{
							_interface.Display(false);
						}
					}
				});
		}

		private void GetLeaderboard(string token, Action<bool> success)
		{
			_currentLeaderboard = null;
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
				response =>
				{
					_currentLeaderboard = response;
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
			else
			{
				success(false);
			}
		}

		public void GetLeaderboardStandings(int pageNumber, Action<bool> success, Action<List<LeaderboardStandingsResponse>> result = null)
		{
			if (result == null)
			{
				_currentStandings.Clear();
			}
			if (SUGARManager.CurrentUser != null && _currentLeaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = _currentLeaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = _currentFilter,
					PageLimit = _positionCount,
					PageOffset = pageNumber
				};

				SUGARManager.client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					if (result == null)
					{
						_currentStandings = response.ToList();
					}
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard standings. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
			else
			{
				success(false);
			}
		}

		public void SetPositionCount(int count)
		{
			_positionCount = count;
		}
	}
}