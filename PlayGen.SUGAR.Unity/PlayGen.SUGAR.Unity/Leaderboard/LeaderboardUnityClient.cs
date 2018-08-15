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
	/// Use this to get the current standings for a leaderboard
	/// </summary>
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : BaseUnityClient<BaseLeaderboardInterface>
	{
		[Tooltip("Number of results that should be gathered per call.")]
		[SerializeField]
		private int _positionCount;

		private LeaderboardFilterType _currentFilter;

		/// <value>
		/// Current filter to use for gathering leaderboard standings.
		/// </value>
		public LeaderboardFilterType CurrentFilter => _currentFilter;

		private bool _multiplePerActor;

		/// <value>
		/// Current setting for whether actors can appear on leaderboards multiple times.
		/// </value>
		public bool MultiplePerActor => _multiplePerActor;

		/// <value>
		/// Current leaderboard to use for gathering leaderboard standings from.
		/// </value>
		public LeaderboardResponse CurrentLeaderboard { get; private set; }

		/// <value>
		/// Last set of standings gathered.
		/// </value>
		public List<LeaderboardStandingsResponse> CurrentStandings { get; private set; } = new List<LeaderboardStandingsResponse>();

		/// <value>
		/// Number of results that should be gathered per call.
		/// </value>
		public int PositionCount => _positionCount;

		/// <summary>
		/// Gathers information for leaderboard and displays the interface if it has been provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appeard multiple times</param>
		/// <param name="pageNumber">**Optional** The page number to start from (default: 0)</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void Display(string token, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber = 0, bool globalLeaderboard = false)
		{
			_currentFilter = filter;
			_multiplePerActor = multiplePerActor;
			GetLeaderboard(token, globalLeaderboard,
			onComplete =>
			{
				if (onComplete)
				{
					GetLeaderboardStandings(pageNumber,
					result =>
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

		private void GetLeaderboard(string token, bool globalLeaderboard, Action<bool> onComplete)
		{
			CurrentLeaderboard = null;
			if (SUGARManager.UserSignedIn)
			{
				if (globalLeaderboard)
				{
					SUGARManager.client.Leaderboard.GetGlobalAsync(token,
					response =>
					{
						CurrentLeaderboard = response;
						onComplete(true);
					},
					exception =>
					{
						Debug.LogError($"Failed to get leaderboard. {exception}");
						onComplete(false);
					});
				}
				else
				{
					SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
					response =>
					{
						CurrentLeaderboard = response;
						onComplete(true);
					},
					exception =>
					{
						Debug.LogError($"Failed to get leaderboard. {exception}");
						onComplete(false);
					});
				}
			}
			else
			{
				onComplete(false);
			}
		}

		/// <summary>
		/// Get standings for the current leaderboard. 
		/// </summary>
		/// <param name="pageNumber">The page number to retrieve</param>
		/// <param name="onComplete">Whether the standings were retrieved successfully</param>
		/// <param name="result">**Optional** the results for the leaderboard standings, null value will save results to CurrentStandings (default: null)</param>
		public void GetLeaderboardStandings(int pageNumber, Action<bool> onComplete, Action<List<LeaderboardStandingsResponse>> result = null)
		{
			SUGARManager.unity.StartSpinner();
			if (result == null)
			{
				CurrentStandings.Clear();
			}
			var actor = CurrentLeaderboard == null ? null : CurrentLeaderboard.ActorType == ActorType.Group || _currentFilter == LeaderboardFilterType.GroupMembers || _currentFilter == LeaderboardFilterType.Alliances ? SUGARManager.CurrentGroup : SUGARManager.CurrentUser;
			if (actor != null && CurrentLeaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = CurrentLeaderboard.Token,
					GameId = CurrentLeaderboard.GameId,
					ActorId = actor.Id,
					LeaderboardFilterType = _currentFilter,
					PageLimit = _positionCount,
					PageOffset = pageNumber,
					MultiplePerActor = _currentFilter != LeaderboardFilterType.Near ? _multiplePerActor : false
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
					}
					else
					{
						result(response.ToList());
					}
					onComplete(true);
				},
				exception =>
				{
					SUGARManager.unity.StopSpinner();
					Debug.LogError($"Failed to get leaderboard standings. {exception}");
					result?.Invoke(new List<LeaderboardStandingsResponse>());
					onComplete(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				result?.Invoke(new List<LeaderboardStandingsResponse>());
				onComplete(false);
			}
		}

		/// <summary>
		/// Set the maximum number of results to get per call.
		/// </summary>
		/// <param name="count">The Maximum number of results</param>
		public void SetPositionCount(int count)
		{
			_positionCount = count;
		}

		internal void ResetClient()
		{
			CurrentLeaderboard = null;
			CurrentStandings.Clear();
		}
	}
}