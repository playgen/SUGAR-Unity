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
		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards = new Dictionary<ActorType, List<LeaderboardResponse>>();

		public ActorType CurrentActorType = ActorType.User;

		public void DisplayList(ActorType filter = ActorType.User)
		{
			CurrentActorType = filter;
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

		private void GetLeaderboards(Action<bool> success)
		{
			Leaderboards.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(SUGARManager.GameId,
				response =>
				{
					var result = response.ToList();
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						var at = actorType;
						Leaderboards.Add(actorType, result.Where(lb => lb.ActorType == at).ToList());
					}
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard list. " + exception.Message;
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
	}
}
