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
		protected Text _statusText;

		protected virtual void Awake()
		{
			if (!_name || !_password)
			{
				Debug.LogError("You must provide input fields for username and password.");
			}
			else {
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
				_closeButton.onClick.AddListener(Hide);
			}
		}

		protected virtual void OnDisable()
		{
			_name.text = "";
			_password.text = "";
			if (_statusText)
			{
				_statusText.text = "";
			}
		}

		internal void Show()
		{
			SUGARManager.unity.EnableObject(gameObject);
			PostShow();
		}

		protected abstract void PostShow();

		internal void Hide()
		{
			SUGARManager.unity.DisableObject(gameObject);
			PostHide();
		}

		protected abstract void PostHide();

		internal void RegisterButtonDisplay(bool display)
		{
			if (_registerButton)
			{
				_registerButton.gameObject.SetActive(display);
			}
		}

		internal void SetStatus(string text)
		{
			if (_statusText)
			{
				_statusText.text = text;
			}
		}
	}
}
