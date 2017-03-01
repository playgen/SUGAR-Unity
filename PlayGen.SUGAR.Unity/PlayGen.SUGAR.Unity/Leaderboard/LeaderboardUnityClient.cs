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

		public LeaderboardFilterType CurrentFilter = LeaderboardFilterType.Top;

		public LeaderboardResponse CurrentLeaderboard;

		public List<LeaderboardStandingsResponse> CurrentStandings = new List<LeaderboardStandingsResponse>();

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

		public void Display(string token, LeaderboardFilterType filter, int pageNumber = 0)
		{
			if (_leaderboardInterface)
			{
				CurrentFilter = filter;
				SUGARManager.unity.StartSpinner();
				GetLeaderboard(token, success =>
				{
					if (success)
					{
						GetLeaderboardStandings(pageNumber, result =>
						{
							SUGARManager.unity.StopSpinner();
							_leaderboardInterface.Display(result);
						});
					}
					else
					{
						SUGARManager.unity.StopSpinner();
						_leaderboardInterface.Display(false);
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

		private void GetLeaderboard(string token, Action<bool> success)
		{
			CurrentLeaderboard = null;
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Leaderboard.GetAsync(token, SUGARManager.GameId,
				response =>
				{
					CurrentLeaderboard = response;
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

		public void GetLeaderboardStandings(int pageNumber, Action<bool> success, Action<List<LeaderboardStandingsResponse>> result = null)
		{
			if (result == null)
			{
				CurrentStandings.Clear();
			}
			if (SUGARManager.CurrentUser != null && CurrentLeaderboard != null)
			{
				var request = new LeaderboardStandingsRequest
				{
					LeaderboardToken = CurrentLeaderboard.Token,
					GameId = SUGARManager.GameId,
					ActorId = SUGARManager.CurrentUser.Id,
					LeaderboardFilterType = CurrentFilter,
					PageLimit = _positionCount,
					PageOffset = pageNumber
				};

				SUGARManager.client.Leaderboard.CreateGetLeaderboardStandingsAsync(request,
				response =>
				{
					if (result == null)
					{
						CurrentStandings = response.ToList();
					}
					success(true);
				},
				exception =>
				{
					string error = "Failed to get leaderboard standings. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
			else
			{
				success(false);
			}
		}

		public void SetPositionCount(int count)
		{
			_positionCount = count;
		}
	}
}