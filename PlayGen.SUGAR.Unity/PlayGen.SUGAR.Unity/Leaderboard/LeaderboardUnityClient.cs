using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to leaderboard standings.
	/// </summary>
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : BaseUnityClient<BaseLeaderboardInterface>
	{
		[Tooltip("Number of results that should be gathered per call.")]
		[SerializeField]
		private int _positionCount;

		[Tooltip("Current filter to use for gathering leaderboard standings.")]
		[SerializeField]
		private LeaderboardFilterType _currentFilter = LeaderboardFilterType.Top;

		private LeaderboardResponse _currentLeaderboard;
		private List<LeaderboardStandingsResponse> _currentStandings = new List<LeaderboardStandingsResponse>();

		/// <summary>
		/// Current filter to use for gathering leaderboard standings.
		/// </summary>
		public LeaderboardFilterType CurrentFilter => _currentFilter;

		/// <summary>
		/// Current leaderboard to use for gathering leaderboard standings from.
		/// </summary>
		public LeaderboardResponse CurrentLeaderboard => _currentLeaderboard;

		/// <summary>
		/// Last set of standings gathered.
		/// </summary>
		public List<LeaderboardStandingsResponse> CurrentStandings => _currentStandings;

		/// <summary>
		/// Number of results that should be gathered per call.
		/// </summary>
		public int PositionCount => _positionCount;

		/// <summary>
		/// Gathers information on the leaderboard with the token provided and gets current standings based on the filter and page number provided, with the UI object displayed if provided.
		/// </summary>
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

		/// <summary>
		/// Get standings for the current leaderboard. A request for results to be returned means that the standings gotten will not be stored. Otherwise, they will be saved into CurrentStandings.
		/// </summary>
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
                        if (_currentLeaderboard.LeaderboardType == LeaderboardType.Earliest || _currentLeaderboard.LeaderboardType == LeaderboardType.Latest)
                        foreach (var r in response)
                        {
                                r.Value = DateTime.Parse(r.Value).ToString();
                        }
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

		/// <summary>
		/// Set the number of results to get at most per call.
		/// </summary>
		public void SetPositionCount(int count)
		{
			_positionCount = count;
		}
	}
}