﻿using System;
using System.Collections;
using PlayGen.SUGAR.Contracts;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to accounts
	/// </summary>
	[DisallowMultipleComponent]
	public class AccountUnityClient : MonoBehaviour
	{
		/// <summary>
		/// UI object for this unity client. Can be left null if no UI is required.
		/// </summary>
		[Tooltip("UI object for this unity client. Can be left null if no UI is required.")]
		[SerializeField]
		private BaseAccountInterface _accountInterface;

		[Tooltip("Should the application attempt to sign in users using information provided at start-up?")]
		[SerializeField]
		private bool _allowAutoLogin;

		[Tooltip("Should the application show a registration button on the UI object?")]
		[SerializeField]
		private bool _allowRegister;

		[Tooltip("The default account source token to be used when signing in and registering via the UI object")]
		[SerializeField]
		private string _defaultSourceToken = "SUGAR";

		private Action<bool> _signInCallback;

		internal CommandLineOptions options;

		internal string autoLoginSourceToken;

		internal bool autoLoginSourcePassRequired;

		internal string autoLoginUsername;

		internal string autoLoginPassword;

		internal string autoLoginGroup;

		internal bool autoLoginAuto;

		internal string autoLoginCustomArgs;

		/// <summary>
		/// Has a UI object been provided for this Unity Client?
		/// </summary>
		public bool HasInterface => _accountInterface;

		/// <summary>
		/// Is there a UI object and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _accountInterface.gameObject.activeInHierarchy;

		internal void CreateInterface(Canvas canvas)
		{
			if (HasInterface)
			{
				bool inScene = _accountInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_accountInterface.gameObject, canvas.transform, false);
					newInterface.name = _accountInterface.name;
					_accountInterface = newInterface.GetComponent<BaseAccountInterface>();
				}
				_accountInterface.RegisterButtonDisplay(_allowRegister);
				_accountInterface.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Displays UI object if provided and allowAutoLogin is false. Attempts automatic sign in using provided details if allowAutoLogin is true.
		/// Note: allowAutoLogin is made false after automatic sign in is first attempted.
		/// </summary>
		public void DisplayPanel(Action<bool> success)
		{
			_signInCallback = success;

			if (SUGARManager.client != null && ((Application.isEditor && options != null) || !Application.isEditor))
			{
				SignIn();
			}
			else
			{
				StartCoroutine(CheckConfigLoad());
			}
		}

		/// <summary>
		/// Hide the UI object if it is currently active.
		/// </summary>
		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_accountInterface.gameObject);
            }
        }

		private IEnumerator CheckConfigLoad()
		{
			WaitForFixedUpdate wait = new WaitForFixedUpdate();
			while (SUGARManager.client == null || (Application.isEditor && options == null))
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
				LoginUser(options.UserId, options.Password ?? string.Empty, options.AuthenticationSource);
			}
			else
			{
				if (HasInterface)
				{
					_accountInterface.Display();
				}
			}
			_allowAutoLogin = false;
			if (options != null) SUGARManager.GroupId = options.GroupId;
		}

		internal void LoginUser(string user, string pass, string sourceToken = "")
		{
			if (string.IsNullOrEmpty(sourceToken))
			{
				sourceToken = _defaultSourceToken;
			}
			SUGARManager.unity.StartSpinner();
			var accountRequest = CreateAccountRequest(user, pass, sourceToken);
			SUGARManager.client.Session.LoginAsync(SUGARManager.GameId, accountRequest,
			response =>
			{
				SUGARManager.unity.StopSpinner();
				if (SUGARManager.unity.GameValidityCheck())
				{
					SUGARManager.CurrentUser = response.User;
					_signInCallback(true);
				}
				Hide();
            },
			exception =>
			{
				Debug.LogError(exception);
				if (HasInterface)
				{
					_accountInterface.SetStatus(Localization.GetAndFormat("LOGIN_ERROR", false, exception.Message));
				}
				_signInCallback(false);
                SUGARManager.unity.StopSpinner();
            });
		}

		internal void RegisterUser(string user, string pass)
		{
			SUGARManager.unity.StartSpinner();
			var accountRequest = CreateAccountRequest(user, pass, _defaultSourceToken);
			SUGARManager.client.Account.CreateAsync(SUGARManager.GameId, accountRequest,
			response =>
			{
				if (HasInterface)
				{
					LoginUser(response.User.Name, pass);
				}
				SUGARManager.unity.StopSpinner();
			},
			exception =>
			{
				if (HasInterface)
				{
					_accountInterface.SetStatus(Localization.GetAndFormat("REGISTER_ERROR", false, exception.Message));
				}
				SUGARManager.unity.StopSpinner();
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
	}
}