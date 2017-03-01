using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAccountInterface : MonoBehaviour
	{
		[SerializeField]
		protected InputField _name;
		[SerializeField]
		protected InputField _password;
		[SerializeField]
		protected Button _loginButton;
		[SerializeField]
		protected Button _registerButton;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Text _errorText;

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