using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling UI objects
	/// </summary>
	public abstract class BaseInterface : MonoBehaviour
	{
		/// <summary>
		/// Text object which displays errors if/when they occur. Can be left null.
		/// </summary>
		[Tooltip("Text object which displays errors if/when they occur. Can be left null.")]
		[SerializeField]
		protected Text _errorText;

		/// <summary>
		/// Button used to disable this object. Can be left null.
		/// </summary>
		[Tooltip("Button used to disable this object. Can be left null.")]
		[SerializeField]
		protected Button _closeButton;

		/// <summary>
		/// Button used to display account interface (if available) if no user is signed in. Can be left null.
		/// </summary>
		[Tooltip("Button used to display account interface (if available) if no user is signed in. Can be left null.")]
		[SerializeField]
		protected Button _signinButton;

		/// <summary>
		/// Base Awake method adds onClick listeners for the close and signin buttons.
		/// </summary>
		protected virtual void Awake()
		{
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(() => SUGARManager.unity.DisableObject(gameObject));
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

		/// <summary>
		/// Functionality triggered before displaying the interface.
		/// </summary>
		protected abstract void PreDisplay();

		/// <summary>
		/// Used to display/redraw the UI on this object. Triggers methods in this order:
		/// HideInterfaces - abstract method used to enable/disable UI on this object and hide other UI objects.
		/// PreDraw - private method. Activates object using SUGARManager.Unity.EnableObject, resets error text and hides signin button.
		/// Draw - abstract method where creation and placement of the UI should be performed.
		/// ErrorDraw - where error text is determined and set, if required.
		/// </summary>
		/// <param name="loadingSuccess">Was the data successfully loaded?</param>
		protected void Show(bool loadingSuccess)
		{
			HideInterfaces();
			PreDraw();
			Draw();
			ErrorDraw(loadingSuccess);
		}

		/// <summary>
		/// Should be used to enable/disable UI on this object and hide other UI objects.
		/// </summary>
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

		/// <summary>
		/// Should be used to set, create and place UI on this object.
		/// </summary>
		protected abstract void Draw();

		/// <summary>
		/// Should be used to set error text and disable UI objects due to errors, if required. By default sets error text in case of no user being signed in or loading issues.
		/// </summary>
		/// <param name="loadingSuccess">Was the data successfully loaded?</param>
		protected virtual void ErrorDraw(bool loadingSuccess)
		{
			if (!loadingSuccess)
			{
				if (!SUGARManager.UserSignedIn)
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

		/// <summary>
		/// Get error string if there were issues loading what was required.
		/// </summary>
		protected abstract string LoadErrorText();

		/// <summary>
		/// Get error string if there were no results to display.
		/// </summary>
		protected abstract string NoResultsErrorText();

		private void AttemptSignIn()
		{
			SUGARManager.account.DisplayLogInPanel(onComplete =>
			{
				if (onComplete)
				{
					OnSignIn();
				}
			});
		}

		/// <summary>
		/// Triggered by successful sign-in via this interface. 
		/// </summary>
		protected abstract void OnSignIn();
	}
}
