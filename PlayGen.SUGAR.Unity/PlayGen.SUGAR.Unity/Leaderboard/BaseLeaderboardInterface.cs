using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _leaderboardName;
		[SerializeField]
		protected Text _leaderboardType;
		protected LeaderboardFilterType _filter;
		[SerializeField]
		protected Button _topButton;
		[SerializeField]
		protected Button _nearButton;
		[SerializeField]
		protected Button _friendsButton;
		[SerializeField]
		protected Text _errorText;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Button _signinButton;

		protected LeaderboardResponse _currentLeaderboard;

		protected virtual void Awake()
		{
			if (_topButton)
			{
				_topButton.onClick.AddListener(delegate { UpdateFilter(0); });
			}
			if (_nearButton)
			{
				_nearButton.onClick.AddListener(delegate { UpdateFilter(1); });
			}
			if (_friendsButton)
			{
				_friendsButton.onClick.AddListener(delegate { UpdateFilter(2); });
			}
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(delegate { SUGARManager.unity.DisableObject(gameObject); });
			}
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		internal void Display(LeaderboardResponse leaderboard, LeaderboardFilterType filter, IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess = true)
		{
			_currentLeaderboard = leaderboard;
			PreDisplay();
			_filter = filter;
			ShowLeaderboard(standings, loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowLeaderboard(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess)
		{
			PreDraw();
			DrawLeaderboard(standings, loadingSuccess);
			PostDraw(standings, loadingSuccess);
		}

		private void PreDraw()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.Friend.Hide();
			SUGARManager.Group.Hide();
			SUGARManager.Unity.EnableObject(gameObject);
			if (_errorText)
			{
				_errorText.text = string.Empty;
			}
			if (_signinButton)
			{
				_signinButton.gameObject.SetActive(false);
			}
			if (_topButton)
			{
				_topButton.interactable = true;
			}
			if (_nearButton)
			{
				_nearButton.interactable = true;
			}
			if (_friendsButton)
			{
				_friendsButton.interactable = true;
			}
		}

		protected abstract void DrawLeaderboard(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess);

		private void PostDraw(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess)
		{
			if (!loadingSuccess)
			{
				if (SUGARManager.CurrentUser == null)
				{
					if (_errorText)
					{
						_errorText.text = Localization.Get("NO_USER_ERROR");
					}
					if (SUGARManager.Account.HasInterface && _signinButton)
					{
						_signinButton.gameObject.SetActive(true);
					}
				}
				else
				{
					if (_errorText)
					{
						_errorText.text = Localization.Get("LEADERBOARD_LOAD_ERROR");
					}
				}
				if (_topButton)
				{
					_topButton.interactable = false;
				}
				if (_nearButton)
				{
					_nearButton.interactable = false;
				}
				if (_friendsButton)
				{
					_friendsButton.interactable = false;
				}
			}
			else if (!standings.Any())
			{
				if (_errorText)
				{
					_errorText.text = Localization.Get("NO_LEADERBOARD_ERROR");
				}
			}
		}

		private void AttemptSignIn()
		{
			SUGARManager.account.DisplayPanel(success =>
			{
				if (success)
				{
					OnSignIn();
				}
			});
		}

		protected abstract void OnSignIn();

		private void UpdateFilter(int filter)
		{
			_filter = (LeaderboardFilterType)filter;
			GetStandings(_filter, 0, result =>
			{
				var standings = result.ToList();
				Display(_currentLeaderboard, _filter, standings);
			});
		}

		protected void GetStandings(LeaderboardFilterType filter, int pageNumber, Action<IEnumerable<LeaderboardStandingsResponse>> result)
		{
			SUGARManager.unity.StartSpinner();
			SUGARManager.Leaderboard.GetLeaderboardStandings(_currentLeaderboard, filter, pageNumber, response =>
			{
				SUGARManager.unity.StopSpinner();
				result(response.ToList());
			});
		}
	}
}
