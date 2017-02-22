using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using PlayGen.Unity.Utilities.BestFit;

namespace PlayGen.SUGAR.Unity
{
	public class LoginUserInterface : MonoBehaviour
	{
		[SerializeField]
		private InputField _name;
		[SerializeField]
		private InputField _password;
		[SerializeField]
		private Button _loginButton;
		[SerializeField]
		private Button _registerButton;
		[SerializeField]
		private Button _closeButton;
		[SerializeField]
		private Text _statusText;

		internal event EventHandler<LoginEventArgs> Login;

		internal event EventHandler<LoginEventArgs> Register;

		private void Awake()
		{
			_loginButton.onClick.AddListener(InvokeLogin);
			if (_registerButton)
			{
				_registerButton.onClick.AddListener(InvokeRegister);
			}
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(Hide);
			}
		}

		private void OnEnable()
		{
			DoBestFit();
			BestFit.ResolutionChange += DoBestFit;
		}

		private void OnDisable()
		{
			BestFit.ResolutionChange -= DoBestFit;
			_name.text = "";
			_password.text = "";
			_statusText.text = "";
		}

		internal void Show()
		{
			SUGARManager.Unity.EnableObject(gameObject);
			_name.ActivateInputField();
		}

		internal void Hide()
		{
			SUGARManager.Unity.DisableObject(gameObject);
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
			_statusText.text = text;
		}

		private void InvokeLogin()
		{
			var args = new LoginEventArgs(_name.text, _password.text);
			if (Login != null) Login(this, args);
		}

		private void InvokeRegister()
		{
			var args = new LoginEventArgs(_name.text, _password.text);
			if (Register != null) Register(this, args);
		}

		internal void DoBestFit()
		{
			GetComponentsInChildren<Button>().Select(t => t.gameObject).BestFit();
		}
	}
}
