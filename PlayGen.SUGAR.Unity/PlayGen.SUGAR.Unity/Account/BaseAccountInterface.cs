using PlayGen.SUGAR.Client;
using UnityEngine;
using UnityEngine.EventSystems;
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
		/// Toggle used to remember the current user login details for easy log in
		/// </summary>
		[Tooltip("Toggle used to remember the current user login details for easy log in")]
		[SerializeField]
		protected Toggle _rememberMeToggle;

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
				_loginButton?.onClick.AddListener(delegate
				{
					SUGARManager.account.LoginUser(_name.text, _password.text, _rememberMeToggle.isOn);
				});
				_registerButton?.onClick.AddListener(delegate { SUGARManager.account.RegisterUser(_name.text, _password.text, _rememberMeToggle.isOn); });
			}
			_closeButton?.onClick.AddListener(delegate { SUGARManager.unity.DisableObject(gameObject); });
		}

		internal void Display()
		{ 
			//_name.text = string.Empty;
			//_password.text = string.Empty;
			if (_errorText)
			{
				_errorText.text = string.Empty;
			}
			SUGARManager.Unity.EnableObject(gameObject);
		}

		internal void RegisterButtonDisplay(bool display)
		{
			_registerButton?.gameObject.SetActive(display);
		}

		public bool RememberLogin()
		{
			return _rememberMeToggle != null && _rememberMeToggle.isOn;
		}

		internal void SetStatus(string text)
		{
			if (_errorText)
			{
				_errorText.text = text;
			}
		}

		internal string[] GetText()
		{
			return new[] { _name.text, _password.text, _errorText.text };
		}

		internal void ResetText()
		{
			_name.text = string.Empty;
			_password.text = string.Empty;
			_errorText.text = string.Empty;
		}

		internal void SetText(string[] text)
		{
			_name.text = text[0];
			_password.text = text[1];
			_errorText.text = text[2];
		}

		internal void SetTokenText(string username)
		{
			_name.text = username;
			_password.placeholder.enabled = true;
		}

		internal void SetPasswordPlaceholder(string username)
		{
			if (_password.placeholder.enabled)
			{
				if (_password.placeholder.enabled && (_password.isFocused || _name.text != username || _name.text == ""))
				{
					_password.placeholder.enabled = false;
				}
			}
		}

		internal bool UseToken(string username)
		{
			return _name.text == username && _password.placeholder.enabled;
		}
	}
}