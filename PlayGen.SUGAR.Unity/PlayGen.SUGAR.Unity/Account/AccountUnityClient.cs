using System;
using System.Collections;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class AccountUnityClient : MonoBehaviour
	{
		[SerializeField]
		private LoginUserInterface _loginUserInterface;

		[SerializeField]
		private bool _allowAutoLogin;

		[SerializeField]
		private bool _allowRegister;

		[SerializeField]
		private string _defaultSourceToken = "SUGAR";

		internal CommandLineOptions options;

		private Action<bool> _signInCallback;

		internal bool HasInterface
		{
			get { return _loginUserInterface; }
		}

		public bool IsActive
		{
			get { return _loginUserInterface && _loginUserInterface.gameObject.activeInHierarchy; }
		}

		internal string autoLoginSourceToken;

		internal bool autoLoginSourcePassRequired;

		internal string autoLoginUsername;

		internal string autoLoginPassword;

		internal string autoLoginGroup;

		internal bool autoLoginAuto;

		internal string autoLoginCustomArgs;

		internal void CreateInterface(Canvas canvas)
		{
			if (_loginUserInterface)
			{
				bool inScene = _loginUserInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_loginUserInterface.gameObject, canvas.transform, false);
					newInterface.name = _loginUserInterface.name;
					_loginUserInterface = newInterface.GetComponent<LoginUserInterface>();
				}
				_loginUserInterface.RegisterButtonDisplay(_allowRegister);
				_loginUserInterface.gameObject.SetActive(false);
			}
		}

		public void DisplayPanel(Action<bool> success)
		{
			_signInCallback = success;

			if (SUGARManager.Client != null)
			{
				SignIn();
			}
			else
			{
				StartCoroutine(CheckConfigLoad());
			}
		}

		public void Hide()
		{
			if (_loginUserInterface && _loginUserInterface.gameObject.activeSelf)
			{
				_loginUserInterface.Hide();
				_loginUserInterface.Login -= LoginUserInterfaceOnLogin;
				_loginUserInterface.Register -= LoginUserInterfaceOnRegister;
			}
		}

		private IEnumerator CheckConfigLoad()
		{
			WaitForFixedUpdate wait = new WaitForFixedUpdate();
			while (SUGARManager.Client == null || (Application.isEditor && options == null))
			{
				yield return wait;
			}
			SignIn();
		}

		private void SignIn()
		{
			if (!Application.isEditor)
			{
				if (_allowAutoLogin)
				{
					options = CommandLineUtility.ParseArgs(Environment.GetCommandLineArgs());
				}
			}
			if (options != null && options.AuthenticationSource == null)
			{
				options.AuthenticationSource = _defaultSourceToken;
			}
			if (options != null && _allowAutoLogin && options.AutoLogin)
			{
				LoginUser(options.UserId, options.AuthenticationSource, options.Password ?? string.Empty);
			}
			else
			{
				if (_loginUserInterface)
				{
					_loginUserInterface.Login += LoginUserInterfaceOnLogin;
					_loginUserInterface.Register += LoginUserInterfaceOnRegister;
					_loginUserInterface.Show();
				}
			}
			_allowAutoLogin = false;
			if (options != null) SUGARManager.GroupId = options.GroupId;
		}

		private void LoginUser(string user, string sourceToken, string pass)
		{
			var accountRequest = CreateAccountRequest(user, pass, sourceToken);
			SUGARManager.Client.Session.LoginAsync(SUGARManager.GameId, accountRequest,
			response =>
			{
				SUGARManager.CurrentUser = response.User;
				_signInCallback(true);
				Hide();
			},
			exception =>
			{
				Debug.LogError(exception);
				if (_loginUserInterface)
				{
					_loginUserInterface.SetStatus("Login Error: " + exception.Message);
				}
				_signInCallback(false);
			});
		}

		private void RegisterUser(string user, string sourceToken, string pass)
		{
			var accountRequest = CreateAccountRequest(user, pass, sourceToken);
			SUGARManager.Client.Account.CreateAsync(SUGARManager.GameId, accountRequest,
			response =>
			{
				if (_loginUserInterface)
				{
					LoginUser(response.User.Name, sourceToken, pass);
				}
			},
			exception =>
			{
				if (_loginUserInterface)
				{
					_loginUserInterface.SetStatus("Registration Error: " + exception.Message);
				}
			});
		}

		private AccountRequest CreateAccountRequest(string user, string pass, string source)
		{
			return new AccountRequest
			{
				Name = user,
				Password = pass,
				SourceToken = source,
			};
		}

		private void LoginUserInterfaceOnLogin(object sender, LoginEventArgs loginEventArgs)
		{
			LoginUser(loginEventArgs.Username, _defaultSourceToken, loginEventArgs.Password);
		}

		private void LoginUserInterfaceOnRegister(object sender, LoginEventArgs loginEventArgs)
		{
			RegisterUser(loginEventArgs.Username, _defaultSourceToken, loginEventArgs.Password);
		}
	}
}