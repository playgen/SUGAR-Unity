using System;
using System.Collections;
using System.Linq;

using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.SceneManagement;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to accounts
	/// </summary>
	[DisallowMultipleComponent]
	public class AccountUnityClient : MonoBehaviour
	{
		/// <summary>
		/// Landscape UI object for this unity client. Can be left null if not required.
		/// </summary>
		[Tooltip("Landscape UI object for this unity client. Can be left null if not required.")]
		[SerializeField]
		protected BaseAccountInterface _landscapeInterface;

		/// <summary>
		/// Portrait UI object for this unity client. Can be left null if not required.
		/// </summary>
		[Tooltip("Portrait UI object for this unity client. Can be left null if not required.")]
		[SerializeField]
		protected BaseAccountInterface _portraitInterface;

		protected BaseAccountInterface _interface => Screen.width > Screen.height ? _landscapeInterface ?? _portraitInterface : _portraitInterface ?? _landscapeInterface;

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
		public bool HasInterface => _interface;

		/// <summary>
		/// Is there a UI object and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		internal void CreateInterface(Canvas canvas)
		{
			if (_landscapeInterface)
			{
				var inScene = _landscapeInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_landscapeInterface.gameObject, canvas.transform, false);
					newInterface.name = _landscapeInterface.name;
					_landscapeInterface = newInterface.GetComponent<BaseAccountInterface>();
				}
				_landscapeInterface.gameObject.SetActive(false);
			}
			if (_portraitInterface)
			{
				var inScene = _portraitInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_portraitInterface.gameObject, canvas.transform, false);
					newInterface.name = _portraitInterface.name;
					_portraitInterface = newInterface.GetComponent<BaseAccountInterface>();
				}
				_portraitInterface.gameObject.SetActive(false);
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

		protected virtual void Update()
		{
			if (_landscapeInterface && _landscapeInterface != _interface && _landscapeInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_landscapeInterface.gameObject);
				SUGARManager.unity.EnableObject(_interface.gameObject);
				_interface.SetText(_landscapeInterface.GetText());
			}
			if (_portraitInterface && _portraitInterface != _interface && _portraitInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_portraitInterface.gameObject);
				SUGARManager.unity.EnableObject(_interface.gameObject);
				_interface.SetText(_portraitInterface.GetText());
			}
		}

		/// <summary>
		/// Hide the UI object if it is currently active.
		/// </summary>
		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_interface.gameObject);
			}
		}

		private IEnumerator CheckConfigLoad()
		{
			var wait = new WaitForFixedUpdate();
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
				options = CommandLineUtility.ParseArgs(Environment.GetCommandLineArgs());
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
					_interface.Display();
				}
			}
			_allowAutoLogin = false;
			if (options != null) SUGARManager.ClassId = options.ClassId;
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
					SUGARManager.userGroup.GetGroups(groups => SUGARManager.CurrentGroup = SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor);
					_signInCallback(true);
				}
				Hide();
			},
			exception =>
			{
				Debug.LogError(exception);
				if (HasInterface)
				{
					_interface.SetStatus(Localization.GetAndFormat("LOGIN_ERROR", false, exception.Message));
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
				SUGARManager.unity.StopSpinner();
				if (SUGARManager.unity.GameValidityCheck())
				{
					LoginUser(response.User.Name, pass);
					SUGARManager.userGroup.GetGroups(groups => SUGARManager.CurrentGroup = SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor);
					_signInCallback(true);
				}
				Hide();
			},
			exception =>
			{
				if (HasInterface)
				{
					_interface.SetStatus(Localization.GetAndFormat("REGISTER_ERROR", false, exception.Message));
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
				SourceToken = source
			};
		}
	}
}