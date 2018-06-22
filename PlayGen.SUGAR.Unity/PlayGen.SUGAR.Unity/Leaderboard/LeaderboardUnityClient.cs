using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using PlayGen.SUGAR.Common;

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

		/// <summary>
		/// Current filter to use for gathering leaderboard standings.
		/// </summary>
		public LeaderboardFilterType CurrentFilter => _currentFilter;

		/// <summary>
		/// Current leaderboard to use for gathering leaderboard standings from.
		/// </summary>
		public LeaderboardResponse CurrentLeaderboard { get; private set; }

		/// <summary>
		/// Last set of standings gathered.
		/// </summary>
		public List<LeaderboardStandingsResponse> CurrentStandings { get; private set; } = new List<LeaderboardStandingsResponse>();

		/// <summary>
		/// Number of results that should be gathered per call.
		/// </summary>
		public int PositionCount => _positionCount;

		/// <summary>
		/// Gathers information on the leaderboard with the token provided and gets current standings based on the filter and page number provided, with the UI object displayed if provided.
		/// </summary>
		public void Display(string token, LeaderboardFilterType filter, int pageNumber = 0, bool globalLeaderboard = false)
		{
			_currentFilter = filter;
			GetLeaderboard(token, globalLeaderboard, success =>
			{
				if (success)
				{
					GetLeaderboardStandings(pageNumber, result =>
					{
						_interface?.Display(result);
					});
				}
				else
				{
					_interface?.Display(false);
				}
			});
		}

		private void GetLeaderboard(string token, bool globalLeaderboard, Action<bool> success)
		{
			CurrentLeaderboard = null;
			if (SUGARManager.CurrentUser != null)
			{
				if (globalLeaderboard)
				{
					SUGARManager.client.Leaderboard.GetGlobalAsync(token,
					response =>
					{
						CurrentLeaderboard = response;
						success(true);
					},
					exception =>
					{
						var error = "Failed to get leaderboard. " + exception.Message;
						Debug.LogError(error);
						success(false);
					});
				}
				else
				{
					SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
					response =>
					{
						CurrentLeaderboard = response;
						success(true);
					},
					exception =>
					{
						var error = "Failed to get leaderboard. " + exception.Message;
						Debug.LogError(error);
						success(false);
					});
				}
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
			SUGARManager.unity.StartSpinner();
			if (result == null)
			{
				CurrentStandings.Clear();
			}
			var actor = CurrentLeaderboard == null ? null : CurrentLeaderboard.ActorType == ActorType.Group || _currentFilter == LeaderboardFilterType.GroupMembers ? SUGARManager.CurrentGroup : SUGARManager.CurrentUser;
			if (actor != null && CurrentLeaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = CurrentLeaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = actor.Id,
					LeaderboardFilterType = _currentFilter,
					PageLimit = _positionCount,
					PageOffset = pageNumber,
					MultiplePerActor = false
				};

				SUGARManager.client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					SUGARManager.unity.StopSpinner();
					var leaderboardStandingsResponses = response.ToList();
					foreach (var r in leaderboardStandingsResponses)
					{
						if (CurrentLeaderboard.LeaderboardType == LeaderboardType.Earliest || CurrentLeaderboard.LeaderboardType == LeaderboardType.Latest)
						{
							r.Value = DateTime.Parse(r.Value).ToString(Localization.SelectedLanguage);
						}
					}
					response = leaderboardStandingsResponses.Where(r => r.Ranking > 0).ToList();
					if (result == null)
					{
						CurrentStandings = response.ToList();
						success(true);
					}
					else
					{
						result(response.ToList());
					}
				},
				exception =>
				{
					SUGARManager.unity.StopSpinner();
					var error = "Failed to get leaderboard standings. " + exception.Message;
					Debug.LogError(error);
					if (result == null)
					{
						success(false);
					}
					else
					{
						result(new List<LeaderboardStandingsResponse>());
					}
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				if (result == null)
				{
					success(false);
				}
				else
				{
					result(new List<LeaderboardStandingsResponse>());
				}
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