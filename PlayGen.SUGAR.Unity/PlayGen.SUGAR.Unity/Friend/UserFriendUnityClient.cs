using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this to get current user's list of friends and send and handle friend requests and other friend related actions
	/// </summary>
	[DisallowMultipleComponent]
	public class UserFriendUnityClient : BaseUnityClient<BaseUserFriendInterface>
	{
		/// <value>
		/// Friends of the currently signed in user.
		/// </value>
		public List<ActorResponseAllowableActions> Friends { get; private set; } = new List<ActorResponseAllowableActions>();

		/// <value>
		/// Pending sent friend requests for currently signed in user.
		/// </value>
		public List<ActorResponseAllowableActions> PendingSent { get; private set; } = new List<ActorResponseAllowableActions>();

		/// <value>
		/// Received friend requests for currently signed in user.
		/// </value>
		public List<ActorResponseAllowableActions> PendingReceived { get; private set; } = new List<ActorResponseAllowableActions>();

		/// <value>
		/// Search results for the last search made.
		/// </value>
		public List<ActorResponseAllowableActions> SearchResults { get; } = new List<ActorResponseAllowableActions>();

		private string _lastSearch;

		/// <summary>
		/// Updates lists and displays UI interface if it has been provided.
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
		/// Send friend request to another user
		/// </summary>
		/// <param name="id">The id of the user to add</param>
		/// <param name="reload">**Optional** Whether the interface should reload after the Friend is Added (default: true)</param>
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
		/// Resolve friend requests sent to and from the current user
		/// </summary>
		/// <param name="id">The Id of the user who sent/received the request</param>
		/// <param name="accept">Whether the request has been accepted</param>
		/// <param name="reverse">Whether the request is cancelled (default: false)</param>
		/// <param name="reload">**Optional** Whether the interface should reload after the Friend is Added (default: true)</param>
		/// <remarks>
		/// - reverse and accept cannot both be set to true, if reverse = true, then the request is cancelled.
		/// </remarks>
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
		/// Remove a relationship between the currently signed in user and another user.
		/// </summary>
		/// <param name="id">The Id for the user which the current signed in user wishes to remove</param>
		/// <param name="reload">**Optional** Whether the UI should be redrawn upon Friend removal (default: true)</param>
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

		// TODO Verify Remark
		/// <summary>
		/// Get friends list for the currently signed in user.
		/// </summary>
		/// <param name="success">Callback which contains Whether the list was successfully returned</param>
		/// <remarks>,
		/// - If the retrieved list is empty, returns true
		/// </remarks>
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
			Friends.Clear();
			if (SUGARManager.UserSignedIn)
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
					Debug.LogError($"Failed to get friends list. {exception}");
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
			if (SUGARManager.UserSignedIn)
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
					Debug.LogError($"Failed to get list. {exception}");
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
			if (SUGARManager.UserSignedIn)
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
					Debug.LogError($"Failed to get list. {exception}");
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
			if (SUGARManager.UserSignedIn)
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
					Debug.LogError($"Failed to get list. {exception}");
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

		internal void Add(int id, Action<bool> success, bool autoAccept = true)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				var relationship = new RelationshipRequest
				{
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id,
					AutoAccept = autoAccept
				};
				SUGARManager.client.UserFriend.CreateFriendRequestAsync(relationship,
				response =>
				{
					RefreshLists(success);
				},
				exception =>
				{
					Debug.LogError($"Failed to create friend request. {exception}");
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
			if (SUGARManager.UserSignedIn)
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
				},
				exception =>
				{
					Debug.LogError($"Failed to update friend request. {exception}");
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
			if (SUGARManager.UserSignedIn)
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
				},
				exception =>
				{
					Debug.LogError($"Failed to update friend status. {exception}");
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

		internal void ResetClient()
		{
			Friends.Clear();
			PendingReceived.Clear();
			PendingSent.Clear();
			SearchResults.Clear();
			_lastSearch = null;
		}
	}
}
