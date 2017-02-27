using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class FriendUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseFriendInterface _friendInterface;

		public bool IsActive => _friendInterface && _friendInterface.gameObject.activeInHierarchy;

		public List<ActorResponse> Friends { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> PendingSent { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> PendingReceived { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> SearchResults { get; private set; } = new List<ActorResponse>();

		internal void CreateInterface(Canvas canvas)
		{
			if (_friendInterface)
			{
				bool inScene = _friendInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_friendInterface.gameObject, canvas.transform, false);
					newInterface.name = _friendInterface.name;
					_friendInterface = newInterface.GetComponent<BaseFriendInterface>();
				}
				_friendInterface.gameObject.SetActive(false);
			}
		}

		public void Display()
		{
			if (_friendInterface)
			{
				GetFriends(success =>
				{
					_friendInterface.Display(success);
				});
			}
		}

		private void GetFriends(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			Friends.Clear();
			/*if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Friends = response.ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					string error = "Failed to get friends list. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}*/
		}

		private void GetPendingSent(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			PendingSent.Clear();
			/*if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					PendingSent = response.ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					string error = "Failed to get list. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}*/
		}

		private void GetPendingReceived(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			PendingReceived.Clear();
			/*if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					PendingReceived = response.ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					string error = "Failed to get list. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}*/
		}

		private void GetSearchResults(string searchString, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			SearchResults.Clear();
			/*if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.User.GetAsync(searchString,
				response =>
				{
					SearchResults = response.ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					string error = "Failed to get list. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}*/
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_friendInterface.gameObject);
			}
		}
	}
}
