using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAchievementListInterface : MonoBehaviour
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
			ShowAchievements(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowAchievements(bool loadingSuccess)
		{
			PreDraw();
			DrawAchievements(loadingSuccess);
			PostDraw(loadingSuccess);
		}

		private void PreDraw()
		{
			SUGARManager.Account.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
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

		protected abstract void DrawAchievements(bool loadingSuccess);

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
						_errorText.text = Localization.Get("ACHIEVEMENT_LOAD_ERROR");
					}
				}
			}
			else if (SUGARManager.Achievement.Progress.Count == 0)
			{
				if (_errorText)
				{
					_errorText.text = Localization.Get("NO_ACHIEVEMENT_ERROR");
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
	}
}
