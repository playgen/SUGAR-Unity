using System;
using System.Collections;
using System.Linq;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Client;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for functionality related to signing in, registering and signing out.
	/// </summary>
	[DisallowMultipleComponent]
	public class AccountUnityClient : MonoBehaviour
	{
		[Tooltip("Landscape interface for this area of functionality. Can be left null if not required.")]
		[SerializeField]
		private BaseAccountInterface _landscapeInterface;

		[Tooltip("Portrait interface for this area of functionality. Can be left null if not required.")]
		[SerializeField]
		private BaseAccountInterface _portraitInterface;

		private BaseAccountInterface _interface => Screen.width > Screen.height ? _landscapeInterface ?? _portraitInterface : _portraitInterface ?? _landscapeInterface;

		[Tooltip("Should the application attempt to sign in users using information provided at start-up?")]
		[SerializeField]
		private bool _allowAutoLogin;

		[Tooltip("Should the application show a registration button on the interface?")]
		[SerializeField]
		private bool _allowRegister;

		[Tooltip("The default account source token used when signing in and registering via the interface")]
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

		private readonly ISavedPrefsHandler _savedPrefsHandler = new SavedPrefsHandler();

		internal string savedLoginToken;

		internal string savedUsername;

		/// <summary>
		/// Has an interface been provided for this Unity Client?
		/// </summary>
		public bool HasInterface => _interface;

		/// <summary>
		/// Is there an interface and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		private void Awake()
		{
			savedLoginToken = _savedPrefsHandler.Get<string>("token");
			savedUsername = _savedPrefsHandler.Get<string>("username");
		}

		internal void CreateInterface(Canvas canvas)
		{
			_landscapeInterface = SetInterface(_landscapeInterface, canvas);
			_landscapeInterface?.gameObject.SetActive(false);
			
			_portraitInterface = SetInterface(_portraitInterface, canvas);
			_portraitInterface?.gameObject.SetActive(false);
		}

		internal BaseAccountInterface SetInterface(BaseAccountInterface popupInterface, Canvas canvas, string extension = "")
		{
			if (!popupInterface)
			{
				return null;
			}
			var inScene = popupInterface.gameObject.scene == SceneManager.GetActiveScene() || popupInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScene)
			{
				var newInterface = Instantiate(popupInterface, canvas.transform, false);
				newInterface.name = popupInterface.name + extension;
				popupInterface = newInterface;
			}
			return popupInterface;
		}

		private void Update()
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
			if (IsActive)
			{
				_interface.RegisterButtonDisplay(_allowRegister && !_interface.UseToken(savedUsername));
				_interface.SetPasswordPlaceholder(savedUsername);
			}
		}

		/// <summary>
		/// Displays interface if provided and allowAutoLogin is false. Attempts automatic sign in using provided details if allowAutoLogin is true.
		/// Note: allowAutoLogin is made false after automatic sign in is first attempted.
		/// </summary>
		/// <param name="success">Callback which will result whether the user successfully signed in.</param>
		public virtual void DisplayLogInPanel(Action<bool> success)
		{
			_signInCallback = success;

			if (SUGARManager.UserSignedIn)
			{
				_signInCallback(true);
				return;
			}
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
		public virtual void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_interface.gameObject);
			}
		}

		/// <summary>
		/// Sign out the currently signed in user
		/// </summary>
		/// <param name="success">Callback which will result whether the user successfully signed out.</param>
		public virtual void Logout(Action<bool> success)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.unity.StartSpinner();
				DeletePrefs();
				SUGARManager.client.Session.LogoutAsync(
				() =>
				{
					SUGARManager.SetCurrentUser(null);
					SUGARManager.SetCurrentGroup(null);
					SUGARManager.unity.ResetClients();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					Debug.LogError(exception.Message);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				success(false);
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
				// Default remember me to false for auto logging in
				LoginUser(options.UserId, options.Password ?? string.Empty, options.AuthenticationSource);
			}
			else
			{
				if (HasInterface)
				{
					_interface.Display();
					if (!string.IsNullOrEmpty(savedLoginToken))
					{
						_interface.SetTokenText(savedUsername);
					}
				}
				else
				{
					_signInCallback(false);
				}
			}
			_allowAutoLogin = false;

			if (options != null)
			{
				SUGARManager.SetClassId(options.ClassId);
			}
		}

		internal void LoginUser(string user, string pass, string sourceToken = "")
		{
			if (SUGARManager.UserSignedIn)
			{
				_signInCallback(false);
				return;
			}
			if (!string.IsNullOrEmpty(savedLoginToken) && (_interface?.UseToken(savedUsername) ?? false))
			{
				LoginToken(savedLoginToken);
			}
			else
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
					PostSignIn(response);
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
		}

		internal void LoginToken(string token)
		{
			SUGARManager.unity.StartSpinner();

			SUGARManager.Client.Session.LoginAsync(token,
			response =>
			{
				PostSignIn(response);
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
			if (SUGARManager.UserSignedIn)
			{
				_signInCallback(false);
				return;
			}
			SUGARManager.unity.StartSpinner();
			var accountRequest = CreateAccountRequest(user, pass, _defaultSourceToken);
			SUGARManager.client.Session.CreateAndLoginAsync(SUGARManager.GameId, accountRequest,
			response =>
			{
				PostSignIn(response);
			},
			exception =>
			{
				if (HasInterface)
				{
					_interface.SetStatus(Localization.GetAndFormat("REGISTER_ERROR", false, exception.Message));
				}
				_signInCallback(false);
				SUGARManager.unity.StopSpinner();
			});
		}

		private void PostSignIn(AccountResponse response)
		{
			SUGARManager.unity.StopSpinner();
			if (SUGARManager.unity.GameValidityCheck())
			{
				SUGARManager.SetCurrentUser(response.User);
				SUGARManager.userGroup.GetGroups(groups => SUGARManager.SetCurrentGroup(SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor));
				_signInCallback(true);
			}
			if ((_interface?.RememberLogin() ?? false) && string.IsNullOrEmpty(savedLoginToken))
			{
				SavePrefs(response.LoginToken, response.User.Name);
			}
			else
			{
				DeletePrefs();
				// Clear text in the account panel
				if (HasInterface)
				{
					_interface.ResetText();
				}
			}
			Hide();
		}

		private void SavePrefs(string token, string username)
		{
			savedLoginToken = token;
			savedUsername = username;
			_savedPrefsHandler.Save("token", token);
			_savedPrefsHandler.Save("username",username);
		}

		private void DeletePrefs()
		{
			_savedPrefsHandler.Delete("token");
			_savedPrefsHandler.Delete("username");
			savedLoginToken = string.Empty;
			savedUsername = string.Empty;
		}

		private AccountRequest CreateAccountRequest(string user, string pass, string source)
		{
			return new AccountRequest
			{
				Name = user,
				Password = pass,
				SourceToken = source,
				IssueLoginToken = (_interface?.RememberLogin() ?? false)
			};
		}
	}
}