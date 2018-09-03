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
		/// Users with some sort of relationship with the currently signed in user.
		/// </value>
		public List<UserResponseRelationshipStatus> Relationships { get; private set; } = new List<UserResponseRelationshipStatus>();

		/// <summary>
		/// Current friends
		/// </summary>
		public List<UserResponseRelationshipStatus> Friends => Relationships
			.Where(r => r.RelationshipStatus == RelationshipStatus.ExistingRelationship).ToList();

		/// <summary>
		/// Friendship requests sent by this user to other Users.
		/// </summary>
		public List<UserResponseRelationshipStatus> PendingSentRequests => Relationships
			.Where(r => r.RelationshipStatus == RelationshipStatus.PendingSentRequest).ToList();

		/// <summary>
		/// Friendship invitations sent by other users to this user.
		/// </summary>
		public List<UserResponseRelationshipStatus> PendingReceivedRequests => Relationships
			.Where(r => r.RelationshipStatus == RelationshipStatus.PendingReceivedRequest).ToList();

		/// <summary>
		/// Updates lists and displays UI interface if it has been provided.
		/// </summary>
		public void Display()
		{
			RefreshRelationships(onComplete =>
			{
				if (_interface)
				{
					_interface.Display(onComplete);
				}
			});
		}

		/// <summary>
		/// Send friend request to another user
		/// </summary>
		/// <param name="id">The id of the user to add</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully performed</param>
		public void AddFriend(int id, Action<bool> onComplete = null)
		{
			UserResponseRelationshipStatus.Add(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Resolve a friend request sent to the current user
		/// </summary>
		/// <param name="id">The Id of the user who sent the request</param>
		/// <param name="accept">Whether the request has been accepted</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully resolved</param>
		public void ManageFriendRequest(int id, bool accept, Action<bool> onComplete = null)
		{
			UserResponseRelationshipStatus.UpdateRequest(id, accept, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Cancel a friend request sent by the current user
		/// </summary>
		/// <param name="id">The Id of the user who received the request</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully cancelled</param>
		public void CancelSentFriendRequest(int id, Action<bool> onComplete = null)
		{
			UserResponseRelationshipStatus.CancelSentRequest(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Remove a relationship between the currently signed in user and another user.
		/// </summary>
		/// <param name="id">The Id for the user which the current signed in user wishes to remove</param>
		/// <param name="onComplete">**Optional** Callback for if the friendship was successfully cancelled</param>
		public void RemoveFriend(int id, Action<bool> onComplete = null)
		{
			UserResponseRelationshipStatus.Remove(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Get friends list for the currently signed in user.
		/// </summary>
		/// <param name="onComplete">Callback which contains the list of friends for the current user</param>
		public void GetFriendsList(Action<List<ActorResponse>> onComplete)
		{
			GetFriends(result =>
			{
				if (result)
				{
					onComplete(Relationships.Where(r => r.RelationshipStatus == RelationshipStatus.ExistingRelationship).Select(r => r.Actor).ToList());
				}
			});
		}

		public void RefreshRelationships(Action<bool> onComplete)
		{
			GetFriends(result =>
			{
				GetPendingReceived(result2 =>
				{
					GetPendingSent(result3 =>
					{
						onComplete(result && result2 && result3);
					});
				});
			});
		}

		internal void GetFriends(Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			Relationships = Relationships.Where(r => r.RelationshipStatus != RelationshipStatus.ExistingRelationship).ToList();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.UserFriend.GetFriendsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Relationships.AddRange(response.Select(r => new UserResponseRelationshipStatus(r, RelationshipStatus.ExistingRelationship)).ToList());
					Relationships = Relationships.OrderBy(r => r.Actor.Name).ToList();
					SUGARManager.unity.StopSpinner();
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get friends list. {exception}");
					SUGARManager.unity.StopSpinner();
					onComplete(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				onComplete(false);
			}
		}

		internal void GetPendingSent(Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			Relationships = Relationships.Where(r => r.RelationshipStatus != RelationshipStatus.PendingSentRequest).ToList();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.UserFriend.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Relationships.AddRange(response.Select(r => new UserResponseRelationshipStatus(r, RelationshipStatus.PendingSentRequest)).ToList());
					Relationships = Relationships.OrderBy(r => r.Actor.Name).ToList();
					SUGARManager.unity.StopSpinner();
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get list. {exception}");
					SUGARManager.unity.StopSpinner();
					onComplete(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				onComplete(false);
			}
		}

		internal void GetPendingReceived(Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			Relationships = Relationships.Where(r => r.RelationshipStatus != RelationshipStatus.PendingReceivedRequest).ToList();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.UserFriend.GetFriendRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Relationships.AddRange(response.Select(r => new UserResponseRelationshipStatus(r, RelationshipStatus.PendingReceivedRequest)).ToList());
					Relationships = Relationships.OrderBy(r => r.Actor.Name).ToList();
					SUGARManager.unity.StopSpinner();
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get list. {exception}");
					SUGARManager.unity.StopSpinner();
					onComplete(false);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				onComplete(false);
			}
		}

		internal void ResetClient()
		{
			Relationships.Clear();
		}
	}
}
