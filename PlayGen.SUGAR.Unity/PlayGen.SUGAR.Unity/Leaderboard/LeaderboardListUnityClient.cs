using System;

using UnityEngine;
using System.Collections.Generic;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardListUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseLeaderboardListInterface _leaderboardListInterface;

		public bool IsActive => _leaderboardListInterface && _leaderboardListInterface.gameObject.activeInHierarchy;

		public Dictionary<ActorType, List<LeaderboardResponse>> Leaderboards = new Dictionary<ActorType, List<LeaderboardResponse>>();

		public ActorType CurrentActorType = ActorType.User;

		internal void CreateInterface(Canvas canvas)
		{
			if (_leaderboardListInterface)
			{
				bool inScene = _leaderboardListInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_leaderboardListInterface.gameObject, canvas.transform, false);
					newInterface.name = _leaderboardListInterface.name;
					_leaderboardListInterface = newInterface.GetComponent<BaseLeaderboardListInterface>();
				}
				_leaderboardListInterface.gameObject.SetActive(false);
			}
		}

		public void DisplayList(ActorType filter = ActorType.User)
		{
			if (_leaderboardListInterface)
			{
				CurrentActorType = filter;
				SUGARManager.unity.StartSpinner();
				GetLeaderboards(success =>
				{
					SUGARManager.unity.StopSpinner();
					_leaderboardListInterface.Display(success);
				});
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_leaderboardListInterface.gameObject);
			}
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
