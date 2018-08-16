using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the interface related to Account functionality.
	/// </summary>
	public abstract class BaseAccountInterface : MonoBehaviour
	{
		/// <value>
		/// Input field used for providing usernames. Required.
		/// </value>
		[Tooltip("Input field used for providing usernames. Required.")]
		[SerializeField]
		protected InputField _name;

		/// <value>
		/// Input field used for providing passwords. Required.
		/// </value>
		[Tooltip("Input field used for providing passwords. Required.")]
		[SerializeField]
		protected InputField _password;

		/// <value>
		/// Button used to trigger attempting to sign in. Can be left null.
		/// </value>
		[Tooltip("Button used to trigger attempting to sign in. Can be left null.")]
		[SerializeField]
		protected Button _loginButton;

		/// <value>
		/// Button used to trigger attempting to register a new account. Can be left null.
		/// </value>
		[Tooltip("Button used to trigger attempting to register a new account. Can be left null.")]
		[SerializeField]
		protected Button _registerButton;

		/// <value>
		/// Toggle used to enable/disable remembering the next signed in user's details for future sessions. Can be left null.
		/// </value>
		[Tooltip("Toggle used to enable/disable remembering the next signed in user's details for future sessions. Can be left null.")]
		[SerializeField]
		protected Toggle _rememberMeToggle;

		/// <value>
		/// Button used to disable this object. Can be left null.
		/// </value>
		[Tooltip("Button used to disable this object. Can be left null.")]
		[SerializeField]
		protected Button _closeButton;

		/// <value>
		/// Text object which displays errors if/when they occur. Can be left null.
		/// </value>
		[Tooltip("Text object which displays errors if/when they occur. Can be left null.")]
		[SerializeField]
		protected Text _errorText;

		internal bool RememberLogin => gameObject.activeInHierarchy && _rememberMeToggle != null && _rememberMeToggle.isOn;

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
				if (_loginButton != null)
				{
					_loginButton.onClick.AddListener(() => SUGARManager.account.LoginUser(_name.text, _password.text));
				}

				if (_registerButton != null)
				{
					_registerButton.onClick.AddListener(() =>SUGARManager.account.RegisterUser(_name.text, _password.text));
				}
			}

			if (_closeButton != null)
			{
				_closeButton.onClick.AddListener(() => SUGARManager.unity.DisableObject(gameObject));
			}
		}

		internal void Display()
		{
			ResetText();
			SUGARManager.Unity.EnableObject(gameObject);
		}

		internal void RegisterButtonDisplay(bool display)
		{
			if (_registerButton != null)
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

		internal void ResetText()
		{
			_name.text = string.Empty;
			_password.text = string.Empty;
			if (_errorText)
			{
				_errorText.text = string.Empty;
			}
		}

		internal string[] GetText()
		{
			return new[] { _name.text, _password.text, _errorText != null ? _errorText.text : string.Empty };
		}

		internal void SetText(string[] text)
		{
			_name.text = text[0];
			_password.text = text[1];
			if (_errorText)
			{
				_errorText.text = text[2];
			}
		}

		internal void SetTokenText(string username)
		{
			_name.text = username;
			if (_password.placeholder)
			{
				_password.placeholder.enabled = true;
			}
		}

		internal void SetPasswordPlaceholder(string username)
		{
			if (_password.placeholder != null && _password.placeholder.enabled && (_password.isFocused || _name.text != username || string.IsNullOrEmpty(_name.text)))
			{
				_password.placeholder.enabled = false;
			}
		}

		internal bool ShouldUseToken(string username)
		{
			return _name.text == username && _password.placeholder != null && _password.placeholder.enabled;
		}
	}
}