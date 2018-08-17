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
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">**Optional** The page number to start from (default: 0)</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void Display(string token, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber = 0, bool globalLeaderboard = false)
		{
			_currentFilter = filter;
			_multiplePerActor = multiplePerActor;
			CurrentLeaderboard = null;
			CurrentStandings.Clear();
			GetLeaderboardDetails(token,
			leaderboard =>
			{
				if (leaderboard != null)
				{
					CurrentLeaderboard = leaderboard;
					GetCurrentLeaderboardStandings(pageNumber,
					result =>
					{
						_interface?.Display(result);
					});
				}
				else
				{
					_interface?.Display(false);
				}
			}, globalLeaderboard);
		}

		/// <summary>
		/// Gathers information for the leaderboard that uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="leaderboard">Callback which returns the Leaderboard details</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardDetails(string token, Action<LeaderboardResponse> leaderboard, bool globalLeaderboard = false)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				if (globalLeaderboard)
				{
					SUGARManager.client.Leaderboard.GetGlobalAsync(token, 
					response =>
					{
						SUGARManager.unity.StopSpinner();
						leaderboard(response);
					},
					exception =>
					{
						SUGARManager.unity.StopSpinner();
						Debug.LogError($"Failed to get leaderboard. {exception}");
						leaderboard(null);
					});
				}
				else
				{
					SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
					response =>
					{
						SUGARManager.unity.StopSpinner();
						leaderboard(response);
					},
					exception =>
					{
						SUGARManager.unity.StopSpinner();
						Debug.LogError($"Failed to get leaderboard. {exception}");
						leaderboard(null);
					});
				}
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				leaderboard(null);
			}
		}

		/// <summary>
		/// Gathers the current standing in the CurrentLeaderboard for the Current User or Group (whichever is valid for the leaderboard).
		/// </summary>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetCurrentLeaderboardPosition(Action<LeaderboardStandingsResponse> standings)
		{
			GetLeaderboardPosition(CurrentLeaderboard, standings);
		}

		/// <summary>
		/// Gathers the current standing of the Current User or Group (whichever is valid for the leaderboard) for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardPosition(string token, Action<LeaderboardStandingsResponse> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, LeaderboardFilterType.Near, false, 0, 1, response => standings(response?.FirstOrDefault()), globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standing for the Actor provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardPosition(string token, ActorResponse actor, Action<LeaderboardStandingsResponse> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardPosition(token, actor.Id, standings, globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standing for the actor with the id provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardPosition(string token, int actorId, Action<LeaderboardStandingsResponse> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, actorId, LeaderboardFilterType.Near, false, 0, 1, response => standings(response?.FirstOrDefault()), globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standing of the Current User or Group (whichever is valid for the leaderboard) for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardPosition(LeaderboardResponse leaderboard, Action<LeaderboardStandingsResponse> standings)
		{
			GetLeaderboardStandings(leaderboard, LeaderboardFilterType.Near, false, 0, 1, response => standings(response?.FirstOrDefault()));
		}

		/// <summary>
		/// Gathers the current standing for the Actor provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardPosition(LeaderboardResponse leaderboard, ActorResponse actor, Action<LeaderboardStandingsResponse> standings)
		{
			GetLeaderboardPosition(leaderboard, actor.Id, standings);
		}

		/// <summary>
		/// Gathers the current standing for the Actor with the id provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardPosition(LeaderboardResponse leaderboard, int actorId, Action<LeaderboardStandingsResponse> standings)
		{
			GetLeaderboardStandings(leaderboard, actorId, LeaderboardFilterType.Near, false, 0, 1, response => standings(response?.FirstOrDefault()));
		}

		/// <summary>
		/// Gathers the current standings for the CurrentLeaderboard, using the LeaderboardTypeFilter, MultiplePerActor and PositionCount values already defined, and sets CurrentStandings to these values.
		/// </summary>
		/// <param name="pageNumber">The page number to retrieve</param>
		/// <param name="onComplete">Callback which returns if the updating of CurrentStandings was successful.</param>
		/// <remarks>
		/// Current User or Group is used for filters, depending on which is valid for the leaderboard.
		/// </remarks>
		public void GetCurrentLeaderboardStandings(int pageNumber, Action<bool> onComplete)
		{
			GetLeaderboardStandings(CurrentLeaderboard, _currentFilter, _multiplePerActor, pageNumber, _positionCount,
			standings => 
			{
				if (standings != null)
				{
					CurrentStandings = standings;
				}
				onComplete(standings != null);
			});
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the Current User or Group (whichever is valid for the leaderboard) using the filter and multiplePerActor settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, filter, multiplePerActor, 0, 0, standings, globalLeaderboard);
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the actor provided using the filter and multiplePerActor settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, ActorResponse actor, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, actor.Id, filter, multiplePerActor, standings, globalLeaderboard);
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the actor with the id provided using the filter and multiplePerActor settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, int actorId, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, actorId, filter, multiplePerActor, 0, 0, standings, globalLeaderboard);
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the Current User or Group (whichever is valid for the leaderboard) using the filter and multiplePerActor settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings)
		{
			GetLeaderboardStandings(leaderboard, filter, multiplePerActor, 0, 0, standings);
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the actor provided using the filter and multiplePerActor settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, ActorResponse actor, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings)
		{
			GetLeaderboardStandings(leaderboard, actor.Id, filter, multiplePerActor, standings);
		}

		/// <summary>
		/// Gathers all of the current standings in relation to the actor with the id provided using the filter and multiplePerActor settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, int actorId, LeaderboardFilterType filter, bool multiplePerActor, Action<List<LeaderboardStandingsResponse>> standings)
		{
			GetLeaderboardStandings(leaderboard, actorId, filter, multiplePerActor, 0, 0, standings);
		}

		/// <summary>
		/// Gathers the current standings in relation to the Current User or Group (whichever is valid for the leaderboard) using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardDetails(token,
			leaderboard =>
			{
				GetLeaderboardStandings(leaderboard, filter, multiplePerActor, pageNumber, positionCount, standings);
			}, globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standings in relation to the actor provided using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, ActorResponse actor, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardStandings(token, actor.Id, filter, multiplePerActor, pageNumber, positionCount, standings, globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standings in relation to the actor with the id provided using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard which uses the token provided.
		/// </summary>
		/// <param name="token">The unique identifier for the Leaderboard</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		/// <param name="globalLeaderboard">**Optional** Whether the leaderboard is global or in game scope. (default: false)</param>
		public void GetLeaderboardStandings(string token, int actorId, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings, bool globalLeaderboard = false)
		{
			GetLeaderboardDetails(token,
			leaderboard =>
			{
				GetLeaderboardStandings(leaderboard, actorId, filter, multiplePerActor, pageNumber, positionCount, standings);
			}, globalLeaderboard);
		}

		/// <summary>
		/// Gathers the current standings in relation to the Current User or Group (whichever is valid for the leaderboard) using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings)
		{
			var actor = leaderboard == null ? null : leaderboard.ActorType == ActorType.Group || filter == LeaderboardFilterType.GroupMembers || filter == LeaderboardFilterType.Alliances ? SUGARManager.CurrentGroup : SUGARManager.CurrentUser;
			GetLeaderboardStandings(leaderboard, actor, filter, multiplePerActor, pageNumber, positionCount, standings);
		}

		/// <summary>
		/// Gathers the current standings in relation to the actor with the id provided using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actor">The actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, ActorResponse actor, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings)
		{
			if (actor != null)
			{
				GetLeaderboardStandings(leaderboard, actor.Id, filter, multiplePerActor, pageNumber, positionCount, standings);
			}
			else
			{
				standings(null);
			}
		}

		/// <summary>
		/// Gathers the current standings in relation to the actor with the id provided using the filter, multiplePerActor, pageNumber and positionCount settings provided for the leaderboard provided.
		/// </summary>
		/// <param name="leaderboard">The leaderboard standings are being gathered for</param>
		/// <param name="actorId">The id of the actor the current standings are being gathered for</param>
		/// <param name="filter">The Filter type to order standings by</param>
		/// <param name="multiplePerActor">If the leaderboard allows for actors to appear multiple times</param>
		/// <param name="pageNumber">The page offset to gather positions from</param>
		/// <param name="positionCount">The amount of positions to gather standings for</param>
		/// <param name="standings">Callback which returns the Leaderboard standings</param>
		public void GetLeaderboardStandings(LeaderboardResponse leaderboard, int actorId, LeaderboardFilterType filter, bool multiplePerActor, int pageNumber, int positionCount, Action<List<LeaderboardStandingsResponse>> standings)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn && leaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = leaderboard.Token,
					GameId = leaderboard.GameId,
					ActorId = actorId,
					LeaderboardFilterType = filter,
					PageLimit = positionCount,
					PageOffset = pageNumber,
					MultiplePerActor = filter != LeaderboardFilterType.Near ? multiplePerActor : false
				};

				SUGARManager.client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					SUGARManager.unity.StopSpinner();
					var leaderboardStandingsResponses = response.ToList();
					foreach (var r in leaderboardStandingsResponses)
					{
						if (leaderboard.LeaderboardType == LeaderboardType.Earliest || leaderboard.LeaderboardType == LeaderboardType.Latest)
						{
							r.Value = DateTime.Parse(r.Value).ToString(Localization.SelectedLanguage);
						}
					}
					response = leaderboardStandingsResponses.Where(r => r.Ranking > 0);
					standings(response.ToList());
				},
				exception =>
				{
					SUGARManager.unity.StopSpinner();
					Debug.LogError($"Failed to get leaderboard standings. {exception}");
					standings(null);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				standings(null);
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