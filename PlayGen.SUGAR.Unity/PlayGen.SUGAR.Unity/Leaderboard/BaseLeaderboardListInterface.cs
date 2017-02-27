using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardListInterface : MonoBehaviour
	{
		protected ActorType _actorType;
		[SerializeField]
		protected Text _leaderboardType;
		[SerializeField]
		protected Button _userButton;
		[SerializeField]
		protected Button _groupButton;
		[SerializeField]
		protected Button _combinedButton;
		[SerializeField]
		protected Text _errorText;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Button _signinButton;

		protected List<List<LeaderboardResponse>> _leaderboards;

		protected virtual void Awake()
		{
			if (_userButton)
			{
				_userButton.onClick.AddListener(delegate { UpdateFilter(1); });
			}
			if (_groupButton)
			{
				_groupButton.onClick.AddListener(delegate { UpdateFilter(2); });
			}
			if (_combinedButton)
			{
				_combinedButton.onClick.AddListener(delegate { UpdateFilter(0); });
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

		internal void Display(List<List<LeaderboardResponse>> leaderboards, ActorType filter, bool loadingSuccess = true)
		{
			_leaderboards = leaderboards;
			PreDisplay();
			_actorType = filter;
			ShowLeaderboards(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowLeaderboards(bool loadingSuccess)
		{
			PreDraw();
			DrawLeaderboards(loadingSuccess);
			PostDraw(loadingSuccess);
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
			if (_userButton)
			{
				_userButton.interactable = true;
			}
			if (_groupButton)
			{
				_groupButton.interactable = true;
			}
			if (_combinedButton)
			{
				_combinedButton.interactable = true;
			}
		}

		protected abstract void DrawLeaderboards(bool loadingSuccess);

		private void PostDraw(bool loadingSuccess)
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
						_errorText.text = Localization.Get("LEADERBOARD_LIST_LOAD_ERROR");
					}
				}
				if (_userButton)
				{
					_userButton.interactable = false;
				}
				if (_groupButton)
				{
					_groupButton.interactable = false;
				}
				if (_combinedButton)
				{
					_combinedButton.interactable = false;
				}
			}
			else if (_leaderboards[(int)_actorType].Count == 0)
			{
				if (_errorText)
				{
					_errorText.text = Localization.Get("NO_LEADERBOARD_LIST_ERROR");
				}
			}
		}

		private void UpdateFilter(int filter)
		{
			Display(_leaderboards, (ActorType)filter);
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
	}
}