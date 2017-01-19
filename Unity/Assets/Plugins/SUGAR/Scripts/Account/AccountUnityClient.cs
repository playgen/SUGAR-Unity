﻿using System;
using System.Collections;
using PlayGen.SUGAR.Contracts.Shared;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

		private CommandLineOptions _options;

		private Action<bool> _signInCallback;

		internal bool HasInterface
		{
			get { return _loginUserInterface; }
		}

		public bool IsActive
		{
			get { return _loginUserInterface && _loginUserInterface.gameObject.activeInHierarchy; }
		}

		#if UNITY_EDITOR
		private string _autoLoginSourceToken;

		private bool _autoLoginSourcePassRequired;

		private string _autoLoginUsername;

		private string _autoLoginPassword;

		private string _autoLoginGroup;

		private bool _autoLoginAuto;

		private string _autoLoginCustomArgs;


		#endif

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
			while (SUGARManager.Client == null)
			{
				yield return wait;
			}
			SignIn();
		}

		private void SignIn()
		{
			if (_allowAutoLogin)
			{
				#if UNITY_EDITOR
				_autoLoginSourcePassRequired = !EditorPrefs.HasKey("AutoLoginSourcePassRequired") || EditorPrefs.GetBool("AutoLoginSourcePassRequired");
				_autoLoginAuto = !EditorPrefs.HasKey("AutoLoginAuto") || EditorPrefs.GetBool("AutoLoginAuto");
				_autoLoginUsername = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginUsername") : string.Empty;
				_autoLoginGroup = EditorPrefs.HasKey("AutoLoginGroup") ? EditorPrefs.GetString("AutoLoginGroup") : string.Empty;
				_autoLoginPassword = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginPassword") : string.Empty;
				_autoLoginSourceToken = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginSourceToken") : string.Empty;
				_autoLoginCustomArgs = EditorPrefs.HasKey("AutoLoginCustomArgs") ? EditorPrefs.GetString("AutoLoginCustomArgs") : string.Empty;
				if (_autoLoginSourcePassRequired)
				{
					_options = CommandLineUtility.ParseArgs(new[] { "-u" + _autoLoginUsername, "-p" + _autoLoginPassword, "-s" + _autoLoginSourceToken, _autoLoginGroup != string.Empty ? "-g" + _autoLoginGroup : string.Empty, _autoLoginAuto ? "-a" : string.Empty, _autoLoginCustomArgs != string.Empty ? "-c" + _autoLoginCustomArgs : string.Empty });
				}
				else
				{
					_options = CommandLineUtility.ParseArgs(new[] { "-u" + _autoLoginUsername, "-s" + _autoLoginSourceToken, _autoLoginGroup != string.Empty ? "-g" + _autoLoginGroup : string.Empty, _autoLoginAuto ? "-a" : string.Empty, _autoLoginCustomArgs != string.Empty ? "-c" + _autoLoginCustomArgs : string.Empty });
				}
				#else
				_options = CommandLineUtility.ParseArgs(System.Environment.GetCommandLineArgs());
				#endif
			}
			if (_options != null && _options.AuthenticationSource == null)
			{
				_options.AuthenticationSource = _defaultSourceToken;
			}
			if (_options != null && _allowAutoLogin && _options.AutoLogin)
			{
				LoginUser(_options.UserId, _options.AuthenticationSource, _options.Password ?? string.Empty);
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
			if (_options != null) SUGARManager.GroupId = _options.GroupId;
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