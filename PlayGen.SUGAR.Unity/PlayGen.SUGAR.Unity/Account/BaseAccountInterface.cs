using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the interface related to Account functionality.
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
		/// Button used to trigger attempting to sign in. Can be left null.
		/// </summary>
		[Tooltip("Button used to trigger attempting to sign in. Can be left null.")]
		[SerializeField]
		protected Button _loginButton;

		/// <summary>
		/// Button used to trigger attempting to register a new account. Can be left null.
		/// </summary>
		[Tooltip("Button used to trigger attempting to register a new account. Can be left null.")]
		[SerializeField]
		protected Button _registerButton;

		/// <summary>
		/// Toggle used to enable/disable remembering the next signed in user's details for future sessions. Can be left null.
		/// </summary>
		[Tooltip("Toggle used to enable/disable remembering the next signed in user's details for future sessions. Can be left null.")]
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
				_loginButton?.onClick.AddListener(() => SUGARManager.account.LoginUser(_name.text, _password.text));
				_registerButton?.onClick.AddListener(() => SUGARManager.account.RegisterUser(_name.text, _password.text));
			}
			_closeButton?.onClick.AddListener(() => SUGARManager.unity.DisableObject(gameObject));
		}

		internal void Display()
		{
			ResetText();
			SUGARManager.Unity.EnableObject(gameObject);
		}

		internal void RegisterButtonDisplay(bool display)
		{
			_registerButton?.gameObject.SetActive(display);
		}

		internal bool RememberLogin()
		{
			return gameObject.activeInHierarchy && (_rememberMeToggle?.isOn ?? false);
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
			return new[] { _name.text, _password.text, _errorText?.text ?? string.Empty };
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
			if ((_password.placeholder?.enabled ?? false) && (_password.isFocused || _name.text != username || string.IsNullOrEmpty(_name.text)))
			{
				_password.placeholder.enabled = false;
			}
		}

		internal bool UseToken(string username)
		{
			return _name.text == username && (_password.placeholder?.enabled ?? false);
		}
	}
}