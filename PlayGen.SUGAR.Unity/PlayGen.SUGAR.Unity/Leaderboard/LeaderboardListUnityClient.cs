using System;

using UnityEngine;
using System.Collections.Generic;

using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;
using System.Linq;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to leaderboards for an application.
	/// </summary>
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : BaseUnityClient<BaseLeaderboardListInterface>
	{
		private readonly Dictionary<ActorType, List<LeaderboardResponse>> _leaderboards = new Dictionary<ActorType, List<LeaderboardResponse>>();

		[Tooltip("Currently used ActorType filter.")]
		[SerializeField]
		private ActorType _currentActorType = ActorType.User;

		/// <summary>
		/// List of leaderboards for this application for each ActorType filter.
		/// </summary>
		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards => _leaderboards;

		/// <summary>
		/// Currently used ActorType filter.
		/// </summary>
		public ActorType CurrentActorType => _currentActorType;

		/// <summary>
		/// Gathers leaderboards for this application and displays list for current ActorType if UI object if provided.
		/// </summary>
		public void DisplayList(ActorType filter = ActorType.User)
		{
			SetFilter(filter);
			SUGARManager.unity.StartSpinner();
			GetLeaderboards(success =>
			{
				SUGARManager.unity.StopSpinner();
				if (_interface)
				{
					_interface.Display(success);
				}
			});
		}

		/// <summary>
		/// Set the ActorType filter to use.
		/// </summary>
		public void SetFilter(ActorType filter)
		{
			_currentActorType = filter;
		}

		private void GetLeaderboards(Action<bool> success)
		{
			_leaderboards.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(SUGARManager.GameId,
				response =>
				{
					var result = response.Items.ToList();
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						var at = actorType;
						_leaderboards.Add(actorType, result.Where(lb => lb.ActorType == at).ToList());
					}
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard list. " + exception.Message;
					Debug.LogError(error);
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						_leaderboards.Add(actorType, new List<LeaderboardResponse>());
					}
					success(false);
				});
			}
			else
			{
				foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
				{
					_leaderboards.Add(actorType, new List<LeaderboardResponse>());
				}
				success(false);
			}
		}
	}
}
