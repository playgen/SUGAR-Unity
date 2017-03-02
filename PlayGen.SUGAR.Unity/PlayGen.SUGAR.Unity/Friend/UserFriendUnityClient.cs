using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to user friends.
	/// </summary>
	[DisallowMultipleComponent]
	public class UserFriendUnityClient : BaseUnityClient<BaseUserFriendInterface>
	{
		private List<ActorResponseAllowableActions> _friends = new List<ActorResponseAllowableActions>();
		private List<ActorResponseAllowableActions> _pendingSent = new List<ActorResponseAllowableActions>();
		private List<ActorResponseAllowableActions> _pendingReceived = new List<ActorResponseAllowableActions>();
		private readonly List<ActorResponseAllowableActions> _searchResults = new List<ActorResponseAllowableActions>();

		/// <summary>
		/// Friends list for currently signed in user.
		/// </summary>
		public List<ActorResponseAllowableActions> Friends => _friends;
		/// <summary>
		/// Pending sent friend requests for currently signed in user.
		/// </summary>
		public List<ActorResponseAllowableActions> PendingSent => _pendingSent;
		/// <summary>
		/// Pending received friend requests for currently signed in user.
		/// </summary>
		public List<ActorResponseAllowableActions> PendingReceived => _pendingReceived;
		/// <summary>
		/// Last set of search results.
		/// </summary>
		public List<ActorResponseAllowableActions> SearchResults => _searchResults;

		private string _lastSearch;

		/// <summary>
		/// Updates lists and displays UI object if provided.
		/// </summary>
		public void Display()
		{
			RefreshLists(success =>
			{
				if (_interface)
				{
					_interface.Display(success);
				}
			});
		}

		/// <summary>
		/// Send friend request to user with id provided. If reload is true, UI is also redrawn.
		/// </summary>
		public void AddFriend(int id, bool reload = true)
		{
			Add(id, result =>
			{
				if (reload && _interface)
				{
					_interface.Reload(result);
				}
			});
		}

		/// <summary>
		/// Resolve friend request to user with id provided. If reload is true, UI is also redrawn.
		/// Reverse should be true if cancelling sent request. Accept and reverse cannot both be true.
		/// </summary>
		public void ManageFriendRequest(int id, bool accept, bool reverse = false, bool reload = true)
		{
			if (!(accept && reverse))
			{
				UpdateRequest(id, accept, reverse, result =>
				{
					if (reload && _interface)
					{
						_interface.Reload(result);
					}
				});
			}
		}

		/// <summary>
		/// Remove user with id provided from friends list. If reload is true, UI is also redrawn.
		/// </summary>
		public void RemoveFriend(int id, bool reload = true)
		{
			Remove(id, result =>
			{
				if (reload && _interface)
				{
					_interface.Reload(result);
				}
			});
		}

		/// <summary>
		/// Get friends list for the currently signed in user.
		/// </summary>
		public void GetFriendsList(Action<bool> success)
		{
			GetFriends(success);
		}

		internal void RefreshLists(Action<bool> success)
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

		internal void GetFriends(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			_friends.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					_friends = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
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
			_pendingSent.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					_pendingSent = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
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
			_pendingReceived.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.UserFriend.GetFriendRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					_pendingReceived = response.Select(r => new ActorResponseAllowableActions(r, true, true)).ToList();
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
			_searchResults.Clear();
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
							if (_friends.Any(p => p.Actor.Id == r.Id) || _pendingReceived.Any(p => p.Actor.Id == r.Id) || _pendingSent.Any(p => p.Actor.Id == r.Id))
							{
								_searchResults.Add(new ActorResponseAllowableActions(r, false, false));
							}
							else
							{
								_searchResults.Add(new ActorResponseAllowableActions(r, true, false));
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

		internal void Add(int id, Action<bool> success)
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
	}
}
