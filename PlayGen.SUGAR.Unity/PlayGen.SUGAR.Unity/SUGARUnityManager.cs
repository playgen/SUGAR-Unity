using System.Collections;
using System.Collections.Generic;
using System.IO;
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

		public bool AnyActiveUI => (SUGARManager.account && SUGARManager.account.IsActive) ||
									(SUGARManager.achievement && SUGARManager.achievement.IsActive) ||
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
			SUGARManager.Client = new SUGARClient(_baseAddress);
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