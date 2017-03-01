using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseInterface : MonoBehaviour
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
			Show(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void Show(bool loadingSuccess)
		{
			HideInterfaces();
			PreDraw();
			Draw(loadingSuccess);
			ErrorDraw(loadingSuccess);
		}

		protected abstract void HideInterfaces();

		private void PreDraw()
		{
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

		protected abstract void Draw(bool loadingSuccess);

		protected virtual void ErrorDraw(bool loadingSuccess)
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
						_errorText.text = LoadErrorText();
					}
				}
			}
		}

		protected abstract string LoadErrorText();

		protected abstract string NoResultsErrorText();

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
