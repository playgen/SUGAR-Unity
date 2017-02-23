using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class LeaderboardUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseLeaderboardUserInterface _leaderboardInterface;

		[SerializeField]
		private BaseLeaderboardPositionInterface _leaderboardPositionPrefab;

		[SerializeField]
		internal int PositionCount;

		internal BaseLeaderboardPositionInterface LeaderboardPositionPrefab => _leaderboardPositionPrefab;

		private LeaderboardResponse _leaderboard;

		internal LeaderboardResponse CurrentLeaderboard => _leaderboard;

		public bool IsActive => _leaderboardInterface && _leaderboardInterface.gameObject.activeInHierarchy;

		internal void CreateInterface(Canvas canvas)
		{
			if (_leaderboardInterface)
			{
				bool inScene = _leaderboardInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_leaderboardInterface.gameObject, canvas.transform, false);
					newInterface.name = _leaderboardInterface.name;
					_leaderboardInterface = newInterface.GetComponent<BaseLeaderboardUserInterface>();
				}
				_leaderboardInterface.gameObject.SetActive(false);
			}
		}

		public void Display(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			if (_leaderboardInterface)
			{
				GetLeaderboard(token, success =>
				{
					if (success)
					{
						GetLeaderboardStandings(filter, 0, result =>
						{
							var standings = result.ToList();
							_leaderboardInterface.Display(filter, standings);
						});
					}
					else
					{
						_leaderboardInterface.Display(filter, Enumerable.Empty<LeaderboardStandingsResponse>(), false);
					}
				});
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.Unity.DisableObject(_leaderboardInterface.gameObject);
			}
		}

		private void GetLeaderboard(string token, Action<bool> success)
		{
			_leaderboard = null;
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Client.Leaderboard.GetAsync(token, SUGARManager.GameId,
				response =>
				{
					_leaderboard = response;
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
			else
			{
				success(false);
			}
		}

		internal void GetLeaderboardStandings(LeaderboardFilterType filter, int pageNumber, Action<IEnumerable<LeaderboardStandingsResponse>> result)
		{
			if (SUGARManager.CurrentUser != null && _leaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = _leaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = filter,
					PageLimit = PositionCount,
					PageOffset = pageNumber
				};

				SUGARManager.Client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					result(response.ToList());
				},
				exception =>
				{
					string error = "Failed to get leaderboard standings. " + exception.Message;
					Debug.LogError(error);
					result(Enumerable.Empty<LeaderboardStandingsResponse>());
				});
			}
			else
			{
				result(Enumerable.Empty<LeaderboardStandingsResponse>());
			}
		}
	}
}