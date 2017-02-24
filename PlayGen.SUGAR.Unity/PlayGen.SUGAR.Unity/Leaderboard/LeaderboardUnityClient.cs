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
		private BaseLeaderboardInterface _leaderboardInterface;

		[SerializeField]
		private int _positionCount;

		public int PositionCount => _positionCount;

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
					_leaderboardInterface = newInterface.GetComponent<BaseLeaderboardInterface>();
				}
				_leaderboardInterface.gameObject.SetActive(false);
			}
		}

		public void Display(string token, LeaderboardFilterType filter = LeaderboardFilterType.Top)
		{
			if (_leaderboardInterface)
			{
				SUGARManager.unity.StartSpinner();
				GetLeaderboard(token, (leaderboard, success) =>
				{
					if (success)
					{
						GetLeaderboardStandings(leaderboard, filter, 0, result =>
						{
							SUGARManager.unity.StopSpinner();
							var standings = result.ToList();
							_leaderboardInterface.Display(leaderboard, filter, standings);
						});
					}
					else
					{
						SUGARManager.unity.StopSpinner();
						_leaderboardInterface.Display(leaderboard, filter, Enumerable.Empty<LeaderboardStandingsResponse>(), false);
					}
				});
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_leaderboardInterface.gameObject);
			}
		}

		private void GetLeaderboard(string token, Action<LeaderboardResponse, bool> success)
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
				response =>
				{
					success(response, true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard. " + exception.Message;
					Debug.LogError(error);
					success(null, false);
				});
			}
			else
			{
				success(null, false);
			}
		}

		internal void GetLeaderboardStandings(LeaderboardResponse leaderboard, LeaderboardFilterType filter, int pageNumber, Action<IEnumerable<LeaderboardStandingsResponse>> result)
		{
			if (SUGARManager.CurrentUser != null && leaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = leaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = filter,
					PageLimit = _positionCount,
					PageOffset = pageNumber
				};

				SUGARManager.client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
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

		public void SetPositionCount(int count)
		{
			_positionCount = count;
		}
	}
}