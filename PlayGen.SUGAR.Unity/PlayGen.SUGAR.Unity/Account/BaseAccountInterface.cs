using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to accounts
	/// </summary>
	public abstract class BaseAccountInterface : MonoBehaviour
	{
		/// <summary>
		/// Input field used for providing usernames. Required.
		/// </summary>
		[Tooltip("Input field used for providing usernames. Required.")]
		[SerializeField]
		protected InputField _name;

		/// <summary>
		/// Input field used for providing passwords. Required.
		/// </summary>
		[Tooltip("Input field used for providing passwords. Required.")]
		[SerializeField]
		protected InputField _password;

		/// <summary>
		/// Button used to trigger attempting signing in. Can be left null.
		/// </summary>
		[Tooltip("Button used to trigger signing in. Can be left null.")]
		[SerializeField]
		protected Button _loginButton;

		/// <summary>
		/// Button used to trigger attempting registration. Can be left null.
		/// </summary>
		[Tooltip("Button used to trigger registration. Can be left null.")]
		[SerializeField]
		protected Button _registerButton;

		/// <summary>
		/// Button used to disable this object. Can be left null.
		/// </summary>
		[Tooltip("Button used to disable this object. Can be left null.")]
		[SerializeField]
		protected Button _closeButton;

		/// <summary>
		/// Text object which displays errors if/when they occur. Can be left null.
		/// </summary>
		[Tooltip("Text object which displays errors if/when they occur. Can be left null.")]
		[SerializeField]
		protected Text _errorText;

		/// <summary>
		/// Base Awake method adds onClick listeners for the login, register and close buttons.
		/// </summary>
		protected virtual void Awake()
		{
			if (!_name || !_password)
			{
				Debug.LogError("You must provide input fields for username and password.");
			}
			else
			{
				if (_loginButton)
				{
					_loginButton.onClick.AddListener(delegate { SUGARManager.account.LoginUser(_name.text, _password.text); });
				}
				if (_registerButton)
				{
					_registerButton.onClick.AddListener(delegate { SUGARManager.account.RegisterUser(_name.text, _password.text); });
				}
			}
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(delegate { SUGARManager.unity.DisableObject(gameObject); });
			}
		}

		internal void Display()
		{
			_name.text = string.Empty;
			_password.text = string.Empty;
			if (_errorText)
			{
				_errorText.text = string.Empty;
			}
			SUGARManager.Unity.EnableObject(gameObject);
		}

		internal void RegisterButtonDisplay(bool display)
		{
			if (_registerButton)
			{
				_registerButton.gameObject.SetActive(display);
			}
		}

		internal void SetStatus(string text)
		{
			if (_errorText)
			{
				_errorText.text = text;
			}
		}
	}
}