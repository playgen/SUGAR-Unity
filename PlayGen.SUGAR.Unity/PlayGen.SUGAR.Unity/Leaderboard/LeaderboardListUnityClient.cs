using System;

using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for functionality related to getting all leaderboards either related to the current game or for the system as a whole.
	/// </summary>
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : BaseUnityClient<BaseLeaderboardListInterface>
	{
		private ActorType _currentActorType = ActorType.User;

		/// <summary>
		/// Dictionary of leaderboards for this application for each ActorType filter.
		/// </summary>
		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards { get; } = new Dictionary<ActorType, List<LeaderboardResponse>>();

		/// <summary>
		/// Currently used ActorType filter.
		/// </summary>
		public ActorType CurrentActorType => _currentActorType;

		/// <summary>
		/// Gathers all leaderboards not attached to a game and displays list for current ActorType if interface if provided.
		/// </summary>
		public void DisplayGlobalList(ActorType filter = ActorType.User)
		{
			SetFilter(filter);
			SUGARManager.unity.StartSpinner();
			GetGlobalLeaderboards(success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		/// <summary>
		/// Gathers leaderboards for this application and displays list for current ActorType if interface if provided.
		/// </summary>
		public void DisplayGameList(ActorType filter = ActorType.User)
		{
			SetFilter(filter);
			SUGARManager.unity.StartSpinner();
			GetLeaderboards(success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		/// <summary>
		/// Set the ActorType filter to use.
		/// </summary>
		internal void SetFilter(ActorType filter)
		{
			_currentActorType = filter;
		}

		private void GetGlobalLeaderboards(Action<bool> success)
		{
			Leaderboards.Clear();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Leaderboard.GetGlobalAsync(
				response =>
				{
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, response.Where(lb => lb.ActorType == actorType).ToList());
					}
					success(true);
				},
				exception =>
				{
					var error = "Failed to get leaderboard list. " + exception.Message;
					Debug.LogError(error);
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, new List<LeaderboardResponse>());
					}
					success(false);
				});
			}
			else
			{
				foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					Leaderboards.Add(actorType, new List<LeaderboardResponse>());
				}
				success(false);
			}
		}

		private void GetLeaderboards(Action<bool> success)
		{
			Leaderboards.Clear();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Leaderboard.GetAsync(SUGARManager.GameId,
				response =>
				{
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, response.Where(lb => lb.ActorType == actorType).ToList());
					}
					success(true);
				},
				exception =>
				{
					var error = "Failed to get leaderboard list. " + exception.Message;
					Debug.LogError(error);
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, new List<LeaderboardResponse>());
					}
					success(false);
				});
			}
			else
			{
				foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					Leaderboards.Add(actorType, new List<LeaderboardResponse>());
				}
				success(false);
			}
		}

		internal void ResetClient()
		{
			Leaderboards.Clear();
		}
	}
}
