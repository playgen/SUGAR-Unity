﻿using System;

using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this to get a list of leaderboards for this game
	/// </summary>
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : BaseUnityClient<BaseLeaderboardListInterface>
	{
		private ActorType _currentActorType = ActorType.User;

		/// <value>
		/// Each ActorType and list of leaderboard responses for this application.
		/// </value>
		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards { get; } = new Dictionary<ActorType, List<LeaderboardResponse>>();

		/// <value>
		/// Currently used ActorType filter.
		/// </value>
		public ActorType CurrentActorType => _currentActorType;

		/// <summary>
		/// Gathers all leaderboards not attached to a game and displays list for current ActorType if interface if provided.
		/// </summary>
		/// <param name="filter">**Optional** The filter type to use (default: ActorType.User)</param>
		public void DisplayGlobalList(ActorType filter = ActorType.User)
		{
			SetFilter(filter);
			SUGARManager.unity.StartSpinner();
			GetGlobalLeaderboards(onComplete =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(onComplete);
			});
		}

		/// <summary>
		/// Gathers leaderboards for this application and displays list for current ActorType if interface if provided.
		/// </summary>
		/// <param name="filter">**Optional** The filter type to use (default: ActorType.User)</param>
		public void DisplayGameList(ActorType filter = ActorType.User)
		{
			SetFilter(filter);
			SUGARManager.unity.StartSpinner();
			GetLeaderboards(onComplete =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(onComplete);
			});
		}

		/// <summary>
		/// Set the ActorType filter to use.
		/// </summary>
		/// <param name="filter">The new ActorType to filter by</param>
		internal void SetFilter(ActorType filter)
		{
			_currentActorType = filter;
		}

		private void GetGlobalLeaderboards(Action<bool> onComplete)
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
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get leaderboard list. {exception}");
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, new List<LeaderboardResponse>());
					}
					onComplete(false);
				});
			}
			else
			{
				foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					Leaderboards.Add(actorType, new List<LeaderboardResponse>());
				}
				onComplete(false);
			}
		}

		private void GetLeaderboards(Action<bool> onComplete)
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
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get leaderboard list. {exception}");
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						Leaderboards.Add(actorType, new List<LeaderboardResponse>());
					}
					onComplete(false);
				});
			}
			else
			{
				foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					Leaderboards.Add(actorType, new List<LeaderboardResponse>());
				}
				onComplete(false);
			}
		}

		internal void ResetClient()
		{
			Leaderboards.Clear();
		}
	}
}
