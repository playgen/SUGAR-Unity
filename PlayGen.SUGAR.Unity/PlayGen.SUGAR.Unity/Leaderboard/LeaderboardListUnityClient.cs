using System;

using UnityEngine;
using System.Collections.Generic;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : BaseUnityClient<BaseLeaderboardListInterface>
	{
		private readonly Dictionary<ActorType, List<LeaderboardResponse>> _leaderboards = new Dictionary<ActorType, List<LeaderboardResponse>>();
		[SerializeField]
		private ActorType _currentActorType = ActorType.User;

		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards => _leaderboards;
		public ActorType CurrentActorType => _currentActorType;

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
					var result = response.ToList();
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
