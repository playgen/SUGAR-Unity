using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseUserFriendInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _errorText;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Button _signinButton;

		protected virtual void Awake()
		{
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(delegate { SUGARManager.unity.DisableObject(gameObject); });
			}
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		internal void Display(bool loadingSuccess = true)
		{
			PreDisplay();
			ShowFriendsList(loadingSuccess);
		}

		internal void Reload(bool loadingSuccess = true)
		{
			ShowFriendsList(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowFriendsList(bool loadingSuccess)
		{
			PreDraw();
			DrawFriendsList(loadingSuccess);
			PostDraw(loadingSuccess);
		}

		private void PreDraw()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
			SUGARManager.Unity.EnableObject(gameObject);
			if (_errorText)
			{
				_errorText.text = string.Empty;
			}
			if (_signinButton)
			{
				_signinButton.gameObject.SetActive(false);
			}
		}

		protected abstract void DrawFriendsList(bool loadingSuccess);

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
						_errorText.text = Localization.Get("FRIENDS_LOAD_ERROR");
					}
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

		protected void GetFriends()
		{
			SUGARManager.userFriend.GetFriends(success =>
			{
				ShowFriendsList(success);
			});
		}

		protected void GetPendingSent()
		{
			SUGARManager.userFriend.GetPendingSent(success =>
			{
				ShowFriendsList(success);
			});
		}

		protected void GetPendingReceived()
		{
			SUGARManager.userFriend.GetPendingReceived(success =>
			{
				ShowFriendsList(success);
			});
		}

		protected void GetSearchResults(string search)
		{
			SUGARManager.userFriend.GetSearchResults(search, success =>
			{
				ShowFriendsList(success);
			});
		}
	}
}
