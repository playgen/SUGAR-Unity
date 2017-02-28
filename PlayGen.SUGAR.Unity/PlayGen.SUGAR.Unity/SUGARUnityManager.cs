using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using UnityEngine;
using PlayGen.SUGAR.Client;
using PlayGen.Unity.Utilities.Loading;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	[RequireComponent(typeof(AccountUnityClient))]
	[RequireComponent(typeof(AchievementUnityClient))]
	[RequireComponent(typeof(UserFriendUnityClient))]
	[RequireComponent(typeof(UserGroupUnityClient))]
	[RequireComponent(typeof(GroupMemberUnityClient))]
	[RequireComponent(typeof(LeaderboardUnityClient))]
	[RequireComponent(typeof(LeaderboardListUnityClient))]
	[RequireComponent(typeof(ResponseHandler))]
	public class SUGARUnityManager : MonoBehaviour
	{
		[SerializeField]
		private string _baseAddress;
		[SerializeField]
		private string _gameToken;
		[SerializeField]
		private int _gameId;
		[SerializeField]
		private bool _useAchievements = true;
		[SerializeField]
		private bool _useFriends = true;
		[SerializeField]
		private bool _useGroups = true;
		[SerializeField]
		private bool _useLeaderboards = true;
		[SerializeField]
		private GameObject _uiBlocker;
		[SerializeField]
		private bool _useBlocker = true;
		[SerializeField]
		private bool _blockerClickClose = true;
		[SerializeField]
		private LoadingSpinner _uiSpinner;
		
		private GameObject _currentBlock;
		private readonly List<GameObject> _blockQueue = new List<GameObject>();

		internal string baseAddress => _baseAddress;

		private string ConfigPath => Application.streamingAssetsPath + "/SUGAR.config.json";

		internal string gameToken => _gameToken;

		internal int gameId
		{
			set { _gameId = value; }
		}

		private bool _validCheck;

		public bool AnyActiveUI => (SUGARManager.account && SUGARManager.account.IsActive) ||
									(SUGARManager.achievement && SUGARManager.achievement.IsActive) ||
									(SUGARManager.userFriend && SUGARManager.userFriend.IsActive) ||
									(SUGARManager.userGroup && SUGARManager.userGroup.IsActive) ||
									(SUGARManager.groupMember && SUGARManager.groupMember.IsActive) ||
									(SUGARManager.gameLeaderboard && SUGARManager.gameLeaderboard.IsActive) ||
									(SUGARManager.leaderboard && SUGARManager.leaderboard.IsActive);

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

			SUGARManager.unity = this;
			SUGARManager.GameId = _gameId;
			SUGARManager.account = GetComponent<AccountUnityClient>();
			SUGARManager.achievement = _useAchievements ? GetComponent<AchievementUnityClient>() : null;
			SUGARManager.userFriend = _useFriends ? GetComponent<UserFriendUnityClient>() : null;
			SUGARManager.userGroup = _useGroups ? GetComponent<UserGroupUnityClient>() : null;
			SUGARManager.groupMember = _useGroups ? GetComponent<GroupMemberUnityClient>() : null;
			SUGARManager.leaderboard = _useLeaderboards ? GetComponent<LeaderboardUnityClient>() : null;
			SUGARManager.gameLeaderboard = _useLeaderboards ? GetComponent<LeaderboardListUnityClient>() : null;

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
			path = "file:///" + path;
			var www = new WWW(path);
			yield return www;

			SUGARManager.config = JsonConvert.DeserializeObject<Config>(www.text);
			Debug.Log(SUGARManager.config.BaseUri);

			_baseAddress = SUGARManager.config.BaseUri;
			SetUpClient();
		}

		private void SetUpClient()
		{
			SUGARManager.client = new SUGARClient(_baseAddress);
			if (string.IsNullOrEmpty(gameToken))
			{
				ResetManager();
				throw new Exception("A game token must be provided in the SUGAR Unity Manager");
			}
			var canvas = GetComponentInChildren<Canvas>();
			GetComponent<AccountUnityClient>().CreateInterface(canvas);
			if (_useLeaderboards)
			{
				GetComponent<LeaderboardListUnityClient>().CreateInterface(canvas);
				GetComponent<LeaderboardUnityClient>().CreateInterface(canvas);
			}
			if (_useAchievements)
			{
				GetComponent<AchievementUnityClient>().CreateInterface(canvas);
			}
			if (_useFriends)
			{
				GetComponent<UserFriendUnityClient>().CreateInterface(canvas);
			}
			if (_useGroups)
			{
				GetComponent<UserGroupUnityClient>().CreateInterface(canvas);
				GetComponent<GroupMemberUnityClient>().CreateInterface(canvas);
			}
			if (_uiBlocker)
			{
				bool blockerInScene = _uiBlocker.scene == SceneManager.GetActiveScene();
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
				bool spinnerInScene = _uiSpinner.gameObject.scene == SceneManager.GetActiveScene();
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

		internal bool GameValidityCheck()
		{
			if (!_validCheck)
			{
				_validCheck = true;
				var game = SUGARManager.client.Game.Get(gameToken).FirstOrDefault();
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
			SUGARManager.achievement = null;
			SUGARManager.leaderboard = null;
			SUGARManager.gameLeaderboard = null;
			SUGARManager.userFriend = null;
			SUGARManager.userGroup = null;
			SUGARManager.groupMember = null;
		}

		public void SetBlocker(bool use, bool block)
		{
			_useBlocker = use;
			_blockerClickClose = block;
		}

		internal void EnableObject(GameObject activeObject)
		{
			if (_uiBlocker && _useBlocker)
			{
				_uiBlocker.GetComponent<Button>().onClick.RemoveAllListeners();
				var objectToDisable = activeObject;
				if (_blockerClickClose)
				{
					_uiBlocker.GetComponent<Button>().onClick.AddListener(delegate { DisableObject(objectToDisable); });
				}
				if (_currentBlock != null && activeObject != _currentBlock)
				{
					_blockQueue.Add(_currentBlock);
				}
				_currentBlock = activeObject;
				_uiBlocker.transform.SetAsLastSibling();
				_uiBlocker.SetActive(true);
			}
			activeObject.transform.SetAsLastSibling();
			activeObject.SetActive(true);
		}

		internal void DisableObject(GameObject activeObject)
		{
			if (_uiBlocker)
			{
				if (activeObject == _currentBlock)
				{
					_currentBlock = null;
				}
				activeObject.SetActive(false);
				if (_blockQueue.Count > 0)
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

		public void SetSpinner(bool clockwise, int speed)
		{
			Loading.Set(speed, clockwise);
		}

		public void StartSpinner(string text = "")
		{
			Loading.Start(text);
			Loading.LoadingSpinner.transform.SetAsLastSibling();
		}

		public void StopSpinner(string text = "", float stopDelay = 0f)
		{
			Loading.Stop(text, stopDelay);
		}
	}
}