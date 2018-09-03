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
	/// Use this to Sign In, Register, Logout and manage other account functionality
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

		/// <value>
		/// Has an interface been provided for this Unity Client in the current orientation
		/// </value>
		public bool HasInterface => _interface != null;

        /// <value>
        /// Whether there are login details that were saved by a previously using "remember me".
        /// </value>
	    public bool HasSavedLogin => !string.IsNullOrEmpty(savedLoginToken);

        /// <value>
        /// Is there an interface and if so is it currently active
        /// </value>
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
				_interface.RegisterButtonDisplay(_allowRegister && !_interface.ShouldUseToken(savedUsername));
				_interface.SetPasswordPlaceholder(savedUsername);
			}
		}

		/// <summary>
		/// Displays interface if provided and allowAutoLogin is false. Attempts automatic sign in using provided details if allowAutoLogin is true.
		/// </summary>
		/// <remarks>
		/// - allowAutoLogin is set to false after automatic sign in is first attempted.
		/// - If there is no interface provided callback will return false
		/// </remarks>
		/// <param name="success">Whether the user successfully signed in.</param>
		public virtual void DisplayLogInPanel(Action<bool> success)
		{
			_signInCallback = success;

			if (SUGARManager.UserSignedIn)
			{
				_signInCallback(true);

				return;
			}
			if (!_allowAutoLogin)
			{
				if (HasInterface)
				{
					_interface.Display();
					if (HasSavedLogin)
					{
						_interface.SetTokenText(savedUsername);
					}
				}
				else
				{
					_signInCallback(false);
				}
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
		/// Hide the AccountPanel game object
		/// </summary>
		public virtual void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_interface.gameObject);
			}
		}

		/// <summary>
		/// Sign out the currently signed in user.
		/// </summary>
		/// <remarks>
		/// - If no user is currently signed in, callback returns false
		/// </remarks>
		/// <param name="success">Whether the currently signed in user successfully signed out.</param>
		public virtual void Logout(Action<bool> success)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.unity.StartSpinner();
				ClearSavedLogin();
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
					Debug.LogError(exception);
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
				Debug.Log("Check interface " + HasInterface);
				if (HasInterface)
				{
					_interface.Display();
					if (HasSavedLogin)
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
			}
            else if (HasSavedLogin && _interface != null && _interface.ShouldUseToken(savedUsername))
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

			    var issueLoginToken = _interface != null && _interface.RememberLogin;
			    if (issueLoginToken)
			    {
                    ClearSavedLogin();
			    }

                var accountRequest = CreateAccountRequest(user, pass, sourceToken, issueLoginToken);

			    SUGARManager.client.Session.LoginAsync(
				    SUGARManager.GameId, 
				    accountRequest,
				    PostSignIn,
				    exception =>
				    {
					    Debug.LogError(exception);
					    if (HasInterface)
					    {
						    _interface.SetStatus(Localization.GetAndFormat("LOGIN_ERROR", false, exception));
					    }
					    _signInCallback(false);
					    SUGARManager.unity.StopSpinner();
				    });
			}
		}

		internal void LoginToken(string token)
		{
			SUGARManager.unity.StartSpinner();

			SUGARManager.Client.Session.LoginAsync(
			    token,
			    PostSignIn,
			    exception =>
			    {
				    Debug.LogError(exception);
				    if (HasInterface)
				    {
					    _interface.SetStatus(Localization.GetAndFormat("LOGIN_ERROR", false, exception));
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

		    ClearSavedLogin();
		    var issueLoginToken = _interface != null && _interface.RememberLogin;
            var accountRequest = CreateAccountRequest(user, pass, _defaultSourceToken, issueLoginToken);

			SUGARManager.client.Session.CreateAndLoginAsync(
			    SUGARManager.GameId, 
			    accountRequest,
			    PostSignIn,
			    exception =>
			    {
				    if (HasInterface)
				    {
					    _interface.SetStatus(Localization.GetAndFormat("REGISTER_ERROR", false, exception));
				    }
				    _signInCallback(false);
				    SUGARManager.unity.StopSpinner();
			    });
		}

		private void PostSignIn(AccountResponse response)
		{
			SUGARManager.unity.StopSpinner();

		    if (HasInterface && _interface.RememberLogin)
		    {
		        if (!string.IsNullOrEmpty(response.LoginToken))
		        {
		            SaveLogin(response.LoginToken, response.User.Name);
		        }
		    }
		    else
		    {
		        ClearSavedLogin();
		        // Clear text in the account panel
		        if (HasInterface)
		        {
		            _interface.ResetText();
		        }
		    }

            if (SUGARManager.unity.GameValidityCheck())
			{
				SUGARManager.SetCurrentUser(response.User);

			    var didGetResources = false;
			    var didGetGroups = false;

			    SUGARManager.Resource.StartCheck(
			        () =>
			        {
			            didGetResources = true;
			            if (didGetGroups && didGetResources)
			            {
			                _signInCallback(true);
                        }
			        });

                SUGARManager.userGroup.GetGroups(
                    groups =>
                    {
                        SUGARManager.SetCurrentGroup(SUGARManager.userGroup.Groups.FirstOrDefault()?.Actor);

                        didGetGroups = true;
                        if (didGetGroups && didGetResources)
                        {
                            _signInCallback(true);
                        }
                    });
			}
			
			Hide();
		}

		private void SaveLogin(string token, string username)
		{
			savedLoginToken = token;
			savedUsername = username;
			_savedPrefsHandler.Save("token", token);
			_savedPrefsHandler.Save("username",username);
		}

		private void ClearSavedLogin()
		{
			_savedPrefsHandler.Delete("token");
			_savedPrefsHandler.Delete("username");
			savedLoginToken = string.Empty;
			savedUsername = string.Empty;
		}

		private AccountRequest CreateAccountRequest(string user, string pass, string source, bool issueLoginToken)
		{
			return new AccountRequest
			{
				Name = user,
				Password = pass,
				SourceToken = source,
				IssueLoginToken = issueLoginToken
			};
		}
	}
}