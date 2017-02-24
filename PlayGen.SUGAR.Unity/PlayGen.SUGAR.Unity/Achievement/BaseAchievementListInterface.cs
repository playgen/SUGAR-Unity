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
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		internal void Display(bool loadingSuccess)
		{
			PreDisplay();
			ShowAchievements(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected abstract void ShowAchievements(bool loadingSuccess);

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
