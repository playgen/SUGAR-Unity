using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.SceneManagement;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Client;
using UnityEngine.XR.WSA.Input;

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

		private readonly ISavedPrefsHandler _savedPrefsHandler = new SavedPrefsHandler();

		internal string savedLoginToken;
		internal string savedUsername;

		/// <summary>
		/// Has a UI object been provided for this Unity Client?
		/// </summary>
		public bool HasInterface => _interface;

		/// <summary>
		/// Is there a UI object and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		void Awake()
		{
			// Saving prefs to local variables so that we dont have to rely on unity to update player prefs efficiently
			// PlayerPrefs dont appear to return updated values unless a new scene is loaded
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

		protected BaseAccountInterface SetInterface(BaseAccountInterface popupInterface, Canvas canvas, string extension = "")
		{
			if (!popupInterface)
				return null;
			var inScene = popupInterface.gameObject.scene == SceneManager.GetActiveScene() || popupInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScene)
			{
				var newInterface = Instantiate(popupInterface.gameObject, canvas.transform, false);
				newInterface.name = popupInterface.name + extension;
				popupInterface = newInterface.GetComponent<BaseAccountInterface>();
			}
			
			return popupInterface;
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
			_interface.RegisterButtonDisplay(_allowRegister && !_interface.UseToken(savedUsername));
			_interface.SetPasswordPlaceholder(savedUsername);
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

		public void Logout(Action<bool> success)
		{
			if (SUGARManager.CurrentUser != null)
			{
				DeletePrefs();
				SUGARManager.client.Session.LogoutAsync(
				() =>
				{
					SUGARManager.CurrentUser = null;
					SUGARManager.CurrentGroup = null;
					success(true);
				},
				exception =>
				{
					Debug.LogError(exception.Message);
					success(false);
				});
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
				LoginUser(options.UserId, options.Password ?? string.Empty, false, options.AuthenticationSource);
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
			
			if (options != null) SUGARManager.ClassId = options.ClassId;
		}

		internal void LoginUser(string user, string pass, bool remember = false, string sourceToken = "")
		{
			
			if (!string.IsNullOrEmpty(savedLoginToken) && _interface.UseToken(savedUsername))
			{
				LoginToken(savedLoginToken, remember);
			}
			else
			{
				if (string.IsNullOrEmpty(sourceToken))
				{
					sourceToken = _defaultSourceToken;
				}
				SUGARManager.unity.StartSpinner();
				var accountRequest = CreateAccountRequest(user, pass, sourceToken, _interface.RememberLogin());

				SUGARManager.client.Session.LoginAsync(SUGARManager.GameId, accountRequest,
					response =>
					{
						SUGARManager.unity.StopSpinner();
						if (SUGARManager.unity.GameValidityCheck())
						{
							SUGARManager.CurrentUser = response.User;
							SUGARManager.userGroup.GetGroups(groups => SUGARManager.CurrentGroup =
								SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor);
							_signInCallback(true);
						}
						Hide();

						if (remember)
						{
							SavePrefs(response.LoginToken, response.User.Name);
						}
						else
						{
							DeletePrefs();
							// Clear text in the account panel
							_interface.ResetText();
						}
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
		internal void LoginToken(string token, bool remember)
		{
			SUGARManager.unity.StartSpinner();

			SUGARManager.Client.Session.LoginAsync(token, response =>
			{
				SUGARManager.unity.StopSpinner();
				if (SUGARManager.unity.GameValidityCheck())
				{
					SUGARManager.CurrentUser = response.User;
					SUGARManager.userGroup.GetGroups(groups => SUGARManager.CurrentGroup =
						SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor);
					_signInCallback(true);
					if (!remember)
					{
						DeletePrefs();
						// Clear text in the account panel
						_interface.ResetText();
					}
				}
				Hide();
			}, exception =>
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

		internal void RegisterUser(string user, string pass, bool issueLoginToken)
		{
			SUGARManager.unity.StartSpinner();
			var accountRequest = CreateAccountRequest(user, pass, _defaultSourceToken, issueLoginToken);
			SUGARManager.client.Session.CreateAndLoginAsync(SUGARManager.GameId, accountRequest,
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
				if (HasInterface)
				{
					_interface.SetStatus(Localization.GetAndFormat("REGISTER_ERROR", false, exception.Message));
				}
				SUGARManager.unity.StopSpinner();
			});
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
			savedUsername = String.Empty;
		}

		private AccountRequest CreateAccountRequest(string user, string pass, string source, bool rememberLogin)
		{
			return new AccountRequest
			{
				Name = user,
				Password = pass,
				SourceToken = source,
				IssueLoginToken = rememberLogin
			};
		}
	}
}