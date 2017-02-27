using System;
using System.Collections.Generic;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class GroupUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseGroupInterface _groupInterface;

		public bool IsActive => _groupInterface && _groupInterface.gameObject.activeInHierarchy;

		public List<ActorResponse> Groups { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> PendingSent { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> PendingReceived { get; private set; } = new List<ActorResponse>();
		public List<ActorResponse> SearchResults { get; private set; } = new List<ActorResponse>();

		internal void CreateInterface(Canvas canvas)
		{
			if (_groupInterface)
			{
				bool inScene = _groupInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_groupInterface.gameObject, canvas.transform, false);
					newInterface.name = _groupInterface.name;
					_groupInterface = newInterface.GetComponent<BaseGroupInterface>();
				}
				_groupInterface.gameObject.SetActive(false);
			}
		}

		public void Display()
		{
			if (_groupInterface)
			{
				GetFriends(success =>
				{
					_groupInterface.Display(success);
				});
			}
		}

		private void GetFriends(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			Groups.Clear();
			/*if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GroupMember.GetUserGroupsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Groups = response.ToList();
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
				SUGARManager.client.GroupMember.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
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
				SUGARManager.client.GroupMember.GetMemberRequestsAsync(SUGARManager.CurrentUser.Id,
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
				SUGARManager.client.Group.GetAsync(searchString,
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
				SUGARManager.unity.DisableObject(_groupInterface.gameObject);
			}
		}
	}
}
