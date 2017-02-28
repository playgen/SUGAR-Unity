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

		public List<ActorResponseAllowableActions> Friends { get; private set; } = new List<ActorResponseAllowableActions>();
		public List<ActorResponseAllowableActions> PendingSent { get; private set; } = new List<ActorResponseAllowableActions>();
		public List<ActorResponseAllowableActions> PendingReceived { get; private set; } = new List<ActorResponseAllowableActions>();
		private string _lastSearch;
		public List<ActorResponseAllowableActions> SearchResults { get; private set; } = new List<ActorResponseAllowableActions>();

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
				RefreshLists(success =>
				{
					_friendInterface.Display(success);
				});
			}
		}

		private void RefreshLists(Action<bool> success)
		{
			GetFriends(result =>
			{
				GetPendingReceived(result2 =>
				{
					GetPendingSent(result3 =>
					{
						GetSearchResults(_lastSearch, result4 =>
						{
							success(result && result2 && result3 && result4);
						});
					});
				});
			});
		}

		public void AddFriend(int id, bool reload = true)
		{
			Add(id, result =>
			{
				if (reload)
				{
					_friendInterface.Reload(result);
				}
			});
		}

		public void ManageFriendRequest(int id, bool accept, bool reverse = false, bool reload = true)
		{
			UpdateRequest(id, accept, reverse, result =>
			{
				if (reload)
				{
					_friendInterface.Reload(result);
				}
			});
		}

		public void RemoveFriend(int id, bool reload = true)
		{
			Remove(id, result =>
			{
				if (reload)
				{
					_friendInterface.Reload(result);
				}
			});
		}

		internal void GetFriends(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			Friends.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Friends = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
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
			}
		}

		internal void GetPendingSent(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			PendingSent.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					PendingSent = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
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
			}
		}

		internal void GetPendingReceived(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			PendingReceived.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					PendingReceived = response.Select(r => new ActorResponseAllowableActions(r, true, true)).ToList();
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
			}
		}

		internal void GetSearchResults(string searchString, Action<bool> success)
		{
			_lastSearch = searchString;
			SearchResults.Clear();
			if (string.IsNullOrEmpty(searchString))
			{
				success(true);
				return;
			}
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.User.GetAsync(searchString,
				response =>
				{
					var results = response.Select(r => (ActorResponse)r).Take(100).ToList();
					foreach (var r in results)
					{
						if (r.Id != SUGARManager.CurrentUser.Id)
						{
							if (Friends.Any(p => p.Actor.Id == r.Id) || PendingReceived.Any(p => p.Actor.Id == r.Id) || PendingSent.Any(p => p.Actor.Id == r.Id))
							{
								SearchResults.Add(new ActorResponseAllowableActions(r, false, false));
							}
							else
							{
								SearchResults.Add(new ActorResponseAllowableActions(r, true, false));
							}
						}
					}
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
			}
		}

		private void Add(int id, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
			{
				var relationship = new RelationshipRequest
				{
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id
				};
				SUGARManager.client.UserFriend.CreateFriendRequestAsync(relationship,
				response =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to create friend request. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}
		}

		internal void UpdateRequest(int id, bool accept, bool reverse, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
			{
				var relationship = new RelationshipStatusUpdate
				{
					RequestorId = id,
					AcceptorId = SUGARManager.CurrentUser.Id,
					Accepted = accept
				};
				if (reverse)
				{
					relationship.RequestorId = SUGARManager.CurrentUser.Id;
					relationship.AcceptorId = id;
				}
				SUGARManager.client.UserFriend.UpdateFriendRequestAsync(relationship,
				() =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to update friend request. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}
		}

		internal void Remove(int id, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
			{
				var relationship = new RelationshipStatusUpdate
				{
					RequestorId = id,
					AcceptorId = SUGARManager.CurrentUser.Id,
					Accepted = false
				};
				SUGARManager.client.UserFriend.UpdateFriendAsync(relationship,
				() =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to update friend status. " + exception.Message;
					Debug.LogError(error);
					SUGARManager.unity.StopSpinner();
					success(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				success(false);
			}
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
