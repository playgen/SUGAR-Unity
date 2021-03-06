using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using PlayGen.SUGAR.Client;
using PlayGen.Unity.Utilities.Loading;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("PlayGen.SUGAR.Unity.Editor")]
namespace PlayGen.SUGAR.Unity
{
	[Serializable]
	public class CustomInterface
	{
		public string Name;
		public GameObject GameObject;
	}

	/// <summary>
	/// Class for managing Unity elements of the asset
	/// </summary>
	[RequireComponent(typeof(AccountUnityClient))]
	[RequireComponent(typeof(EvaluationUnityClient))]
	[RequireComponent(typeof(UserFriendUnityClient))]
	[RequireComponent(typeof(UserGroupUnityClient))]
	[RequireComponent(typeof(GroupMemberUnityClient))]
	[RequireComponent(typeof(LeaderboardUnityClient))]
	[RequireComponent(typeof(LeaderboardListUnityClient))]
	[RequireComponent(typeof(ResourceUnityClient))]
	[RequireComponent(typeof(ResponseHandler))]
	public class SUGARUnityManager : MonoBehaviour
	{
		[Tooltip("Address where SUGAR is hosted. Overwritten by config file if provided.")]
		[SerializeField]
		private string _baseAddress;

		[Tooltip("Unique game token.")]
		[SerializeField]
		private string _gameToken;

		[Tooltip("ID for this game.")]
		[SerializeField]
		private int _gameId;

		[Tooltip("The GameObject used as the 'blocker' behind SUGAR UI objects. Can be left null if not required.")]
		[SerializeField]
		private GameObject _uiBlocker;

		[Tooltip("Should the blocker be used?")]
		[SerializeField]
		private bool _useBlocker = true;

		[Tooltip("Should clicking the blocker close the currently active SUGAR UI object?")]
		[SerializeField]
		private bool _blockerClickClose = true;

		[Tooltip("Object with LoadingSpinner to be used as a blocker when loading is occurring. Can be left null.")]
		[SerializeField]
		private LoadingSpinner _uiSpinner;

		[Tooltip("A list of custom interfaces intended for SUGAR that aren't provided on Unity Clients. Names must be unique.")]
		[SerializeField]
		private List<CustomInterface> _customInterfaceList;

		public Dictionary<string, GameObject> CustomInterfaces;
		private GameObject _currentBlock;
		private readonly List<GameObject> _blockQueue = new List<GameObject>();

		internal string baseAddress => _baseAddress;

		private string ConfigPath => Path.Combine(Application.streamingAssetsPath, "SUGAR.config.json");

		internal string gameToken
		{
			set => _gameToken = value;
		}

		internal int gameId
		{
			set => _gameId = value;
		}

		private bool _validCheck;

		/// <value>
		/// Is any piece of SUGAR UI currently active?
		/// </value>
		public bool AnyActiveUI => (SUGARManager.account && SUGARManager.account.IsActive) ||
									(SUGARManager.evaluation && SUGARManager.evaluation.IsActive) ||
									(SUGARManager.userFriend && SUGARManager.userFriend.IsActive) ||
									(SUGARManager.userGroup && SUGARManager.userGroup.IsActive) ||
									(SUGARManager.groupMember && SUGARManager.groupMember.IsActive) ||
									(SUGARManager.gameLeaderboard && SUGARManager.gameLeaderboard.IsActive) ||
									(SUGARManager.leaderboard && SUGARManager.leaderboard.IsActive) ||
									CustomInterfaces.Values.Any(go => go.activeSelf);

		/// <value>
		/// Whether the spinner UI is currently active
		/// </value>
		public bool SpinnerActive => _uiSpinner && _uiSpinner.IsActive;

		private void Awake()
		{
			if (SUGARManager.Register(this))
			{
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			CustomInterfaces = new Dictionary<string, GameObject>();
			foreach (var ci in _customInterfaceList)
			{
				if (CustomInterfaces.ContainsKey(ci.Name))
				{
					Debug.LogError($"Custom Interface names must be unique. Name {ci.Name} is reused.");
				}
				else
				{
					CustomInterfaces.Add(ci.Name, ci.GameObject);
				}
				if (ci.GameObject.scene == SceneManager.GetActiveScene() || ci.GameObject.scene.name == "DontDestroyOnLoad")
				{
					ci.GameObject.SetActive(false);
				}
			}

			SUGARManager.unity = this;
			SUGARManager.GameId = _gameId;
			SUGARManager.account = GetComponent<AccountUnityClient>();
			SUGARManager.evaluation = GetComponent<EvaluationUnityClient>();
			SUGARManager.userFriend = GetComponent<UserFriendUnityClient>();
			SUGARManager.userGroup = GetComponent<UserGroupUnityClient>();
			SUGARManager.groupMember = GetComponent<GroupMemberUnityClient>();
			SUGARManager.leaderboard = GetComponent<LeaderboardUnityClient>();
			SUGARManager.gameLeaderboard = GetComponent<LeaderboardListUnityClient>();
			SUGARManager.resource = GetComponent<ResourceUnityClient>();

			if (!LoadConfig())
			{
				SetUpClient();
			}
		}

		private bool LoadConfig()
		{
			var path = ConfigPath;
			if (File.Exists(path))
			{
				StartCoroutine(LoadOnlineConfig(path));
				return true;
			}
			return false;
		}

		private IEnumerator LoadOnlineConfig(string path)
		{
			string data;
		
			if (path.Contains("://"))
			{
				var www = UnityWebRequest.Get(path);
				yield return www.SendWebRequest();
				data = www.downloadHandler.text;
			}
			else
			{ 
				data = File.ReadAllText(path);
			}

			SUGARManager.config = JsonConvert.DeserializeObject<Config>(data);
			Debug.Log(SUGARManager.config.BaseUri);

			_baseAddress = SUGARManager.config.BaseUri;
			SetUpClient();
		}

		private void SetUpClient()
		{
		    SUGARManager.client = CreateSUGARClient(_baseAddress);
		    
			if (string.IsNullOrEmpty(_gameToken))
			{
				ResetManager();
				throw new Exception("A game token must be provided in the SUGAR Unity Manager");
			}
			var canvas = GetComponentInChildren<Canvas>();
			GetComponent<AccountUnityClient>().CreateInterface(canvas);
			GetComponent<LeaderboardListUnityClient>().CreateInterface(canvas);
			GetComponent<LeaderboardUnityClient>().CreateInterface(canvas);
			GetComponent<EvaluationUnityClient>().CreateInterface(canvas);
			GetComponent<UserFriendUnityClient>().CreateInterface(canvas);
			GetComponent<UserGroupUnityClient>().CreateInterface(canvas);
			GetComponent<GroupMemberUnityClient>().CreateInterface(canvas);
			var customKeys = CustomInterfaces.Keys.ToList();
			foreach (var key in customKeys)
			{
				var inScene = CustomInterfaces[key].scene == SceneManager.GetActiveScene() || CustomInterfaces[key].scene.name == "DontDestroyOnLoad";
				if (!inScene)
				{
					var newInterface = Instantiate(CustomInterfaces[key], canvas.transform, false);
					newInterface.name = CustomInterfaces[key].name;
					CustomInterfaces[key] = newInterface;
				}
				CustomInterfaces[key].SetActive(false);
			}
			if (_uiBlocker)
			{
				var blockerInScene = _uiBlocker.scene == SceneManager.GetActiveScene() || _uiBlocker.scene.name == "DontDestroyOnLoad";
				if (!blockerInScene)
				{
					var newBlocker = Instantiate(_uiBlocker, canvas.transform, false);
					newBlocker.name = _uiBlocker.name;
					_uiBlocker = newBlocker;
				}
				_uiBlocker.gameObject.SetActive(false);
			}
			if (_uiSpinner)
			{
				var spinnerInScene = _uiSpinner.gameObject.scene == SceneManager.GetActiveScene() || _uiSpinner.gameObject.scene.name == "DontDestroyOnLoad";
				if (!spinnerInScene)
				{
					var newSpinner = Instantiate(_uiSpinner, canvas.transform, false);
					newSpinner.name = _uiSpinner.name;
					_uiSpinner = newSpinner;
				}
				_uiSpinner.gameObject.SetActive(true);
				Loading.Stop();
			}
		}

		/// <summary>
		/// Create a SUGAR Client from a string 
		/// </summary>
		/// <param name="baseAddress">uri to create SUGAR Client from</param>
		/// <returns>new SUGARClient</returns>
		protected virtual SUGARClient CreateSUGARClient(string baseAddress)
		{
		    return new SUGARClient(baseAddress);
		}

		/// <summary>
		/// Check if the current game is valid by the current gameToken
		/// </summary>
		/// <returns>Whether the _gameToken returns a valid game</returns>
		public bool GameValidityCheck()
		{
			if (!_validCheck)
			{
				_validCheck = true;
				var game = SUGARManager.client.Game.Get(_gameToken).FirstOrDefault();

				if (game == null)
				{
					ResetManager();
					Debug.LogError("Game token does not exist");
					return false;
				}

				if (_gameId != game.Id)
				{
					ResetManager();
					Debug.LogError("Game ID provided does not match game ID for provided token");
					return false;
				}

				return true;
			}
			return gameObject.activeSelf;
		}

		private void ResetManager()
		{
			gameObject.SetActive(false);
			SUGARManager.unity = null;
			SUGARManager.client = null;
			SUGARManager.GameId = 0;
			SUGARManager.account = null;
			SUGARManager.evaluation = null;
			SUGARManager.leaderboard = null;
			SUGARManager.gameLeaderboard = null;
			SUGARManager.userFriend = null;
			SUGARManager.userGroup = null;
			SUGARManager.groupMember = null;
		}

		internal void ResetClients()
		{
			SUGARManager.evaluation.ResetClient();
			SUGARManager.leaderboard.ResetClient();
			SUGARManager.gameLeaderboard.ResetClient();
			SUGARManager.userFriend.ResetClient();
			SUGARManager.userGroup.ResetClient();
			SUGARManager.groupMember.ResetClient();
			SUGARManager.resource.ResetClient();
		}

		/// <summary>
		/// Setup for blocker
		/// </summary>
		/// <param name="use">Whether the blocker should be used</param>
		/// <param name="block">Whether clicking on the blocker should close the current UI</param>
		public void SetBlocker(bool use, bool block)
		{
			_useBlocker = use;
			_blockerClickClose = block;
		}

		/// <summary>
		/// Enable a piece of SUGAR UI.
		/// </summary>
		/// <remarks>
		/// - This should be used instead of SetActive to ensure UI and blocker ordering is correct.
		/// </remarks>
		/// <param name="activeObject">The object that should be enabled</param>
		public void EnableObject(GameObject activeObject)
		{
			if (_uiBlocker && _useBlocker)
			{
				_uiBlocker.GetComponent<Button>().onClick.RemoveAllListeners();
				var objectToDisable = activeObject;
				if (_blockerClickClose)
				{
					_uiBlocker.GetComponent<Button>().onClick.AddListener(delegate { DisableObject(objectToDisable); });
				}
				if (_currentBlock != null && activeObject != _currentBlock && !_blockQueue.Contains(_currentBlock))
				{
					_blockQueue.Add(_currentBlock);
				}
				if (activeObject.gameObject.scene.name == null)
				{
					return;
				}
				_currentBlock = activeObject;
				_uiBlocker.transform.SetAsLastSibling();
				_uiBlocker.SetActive(true);
			}
			activeObject.transform.SetAsLastSibling();
			activeObject.SetActive(true);
		}

		/// <summary>
		/// Disable a piece of SUGAR UI.
		/// </summary>
		/// <remarks>
		/// - This should be used instead of SetActive to ensure UI and blocker ordering is correct.
		/// </remarks>
		/// <param name="activeObject">The object that should be disabled</param>
		public void DisableObject(GameObject activeObject)
		{
			if (_uiBlocker)
			{
				if (activeObject == _currentBlock)
				{
					_currentBlock = null;
				}
				activeObject.SetActive(false);
				_blockQueue.Remove(activeObject);
				if (!_currentBlock && _blockQueue.Count > 0)
				{
					EnableObject(_blockQueue[_blockQueue.Count - 1]);
					_blockQueue.RemoveAt(_blockQueue.Count - 1);
					return;
				}
				_uiBlocker.SetActive(false);
				return;
			}
			activeObject.SetActive(false);
		}

		/// <summary>
		/// Setup the spinner
		/// </summary>
		/// <param name="clockwise">Whether the spinner should rotate clockwise or not</param>
		/// <param name="speed">The speed of the rotation</param>
		public void SetSpinner(bool clockwise, int speed)
		{
			Loading.Set(speed, clockwise);
		}

		/// <summary>
		/// Start the loading spinner.
		/// </summary>
		/// <remarks>
		/// - This method should be used instead of directly calling Loading.Start to ensure UI and blocker ordering is correct.
		/// </remarks>
		/// <param name="text">**Optional** Text to display with the spinner. (default: "")</param>
		public void StartSpinner(string text = "")
		{
			Loading.Start(text);
			Loading.LoadingSpinner.transform.SetAsLastSibling();
		}

		/// <summary>
		/// Stop the loading spinner.
		/// </summary>
		/// <remarks>
		/// - This method should be used instead of directly calling Loading.Start to ensure UI and blocker ordering is correct.
		/// </remarks>
		/// <param name="text">**Optional** Text to display when the spinner stops. (default: "")</param>
		/// <param name="stopDelay">**Optional** The time, in seconds, the text should be displayed for before disabling (default: 0)</param>
		public void StopSpinner(string text = "", float stopDelay = 0f)
		{
			Loading.Stop(text, stopDelay);
		}
	}
}