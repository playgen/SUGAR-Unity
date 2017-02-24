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
				GetLeaderboards((result, success) =>
				{
					_leaderboardListInterface.Display(result, filter, success);
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

		private void GetLeaderboards(Action<List<List<LeaderboardResponse>>, bool> success)
		{
			var leaderboards = new List<List<LeaderboardResponse>>();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(SUGARManager.GameId,
				response =>
				{
					var result = response.ToList();
					foreach (var actorType in (ActorType[])Enum.GetValues(typeof(ActorType)))
					{
						var at = actorType;
						leaderboards.Add(result.Where(lb => lb.ActorType == at).ToList());
					}
					success(leaderboards, true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard list. " + exception.Message;
					Debug.LogError(error);
					for (int i = 0; i < Enum.GetValues(typeof(ActorType)).Length; i++)
					{
						leaderboards.Add(new List<LeaderboardResponse>());
					}
					success(leaderboards, false);
				});
			}
			else
			{
				for (int i = 0; i < Enum.GetValues(typeof(ActorType)).Length; i++)
				{
					leaderboards.Add(new List<LeaderboardResponse>());
				}
				success(leaderboards, false);
			}
		}
	}
}
