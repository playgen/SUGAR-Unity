using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this to get current user's list of groups and send and handle group requests
	/// </summary>
	[DisallowMultipleComponent]
	public class UserGroupUnityClient : BaseUnityClient<BaseUserGroupInterface>
	{
		/// <value>
		/// Groups with some sort of relationship with the currently signed in user.
		/// </value>
		public List<GroupResponseRelationshipStatus> Relationships { get; private set; } = new List<GroupResponseRelationshipStatus>();

        /// <summary>
        /// Current groups that a user is a member of
        /// </summary>
	    public List<GroupResponseRelationshipStatus> Groups => Relationships
	        .Where(r => r.RelationshipStatus == RelationshipStatus.ExistingRelationship).ToList();

        /// <summary>
        /// Group membership requests sent by this user to other groups.
        /// </summary>
        public List<GroupResponseRelationshipStatus> PendingSentRequests => Relationships
	        .Where(r => r.RelationshipStatus == RelationshipStatus.PendingSentRequest).ToList();

	    /// <summary>
	    /// Membership invitations sent by groups to this user.
	    /// </summary>
	    public List<GroupResponseRelationshipStatus> PendingReceivedRequests => Relationships
	        .Where(r => r.RelationshipStatus == RelationshipStatus.PendingReceivedRequest).ToList();

        /// <summary>
        /// Gathers updated versions of each type of relationship and displays interface UI object if it has been provided.
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
		/// Send group membership request to a group
		/// </summary>
		/// <param name="id">The id of the group to send the request to</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully performed</param>
		public void AddGroup(int id, Action<bool> onComplete = null)
		{
			GroupResponseRelationshipStatus.Add(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Resolve a group membership request sent to the current user
		/// </summary>
		/// <param name="id">The Id of the group that sent the request</param>
		/// <param name="accept">Whether the request has been accepted</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully resolved</param>
		public void ManageGroupRequest(int id, bool accept, Action<bool> onComplete = null)
		{
			GroupResponseRelationshipStatus.UpdateRequest(id, accept, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Cancel a group membership request sent by the current user
		/// </summary>
		/// <param name="id">The Id of the group that received the request</param>
		/// <param name="onComplete">**Optional** Callback for if the request was successfully cancelled</param>
		public void CancelSentGroupRequest(int id, Action<bool> onComplete = null)
		{
			GroupResponseRelationshipStatus.CancelSentRequest(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Leave a group the current user is a member of
		/// </summary>
		/// <param name="id">The Id for the group which the current signed in user wishes to leave</param>
		/// <param name="onComplete">**Optional** Callback for if the group membership was successfully cancelled</param>
		public void LeaveGroup(int id, Action<bool> onComplete = null)
		{
			GroupResponseRelationshipStatus.Remove(id, result =>
			{
				RefreshRelationships(refresh =>
				{
					onComplete?.Invoke(result && result);
				});
			});
		}

		/// <summary>
		/// Get list of groups the currently signed in user is a member of.
		/// </summary>
		/// <param name="onComplete">Callback which contains the list of groups for the current user</param>
		public void GetGroupsList(Action<List<ActorResponse>> onComplete)
		{
			GetGroups(result =>
			{
				if (result)
				{
					onComplete(Relationships.Where(r => r.RelationshipStatus == RelationshipStatus.ExistingRelationship).Select(r => r.Actor).ToList());
				}
			});
		}

		/// <summary>
		/// Refresh the Relationship list with up to date information
		/// </summary>
		/// <param name="onComplete">Callback for if the update was successfully performed</param>
		public void RefreshRelationships(Action<bool> onComplete)
		{
			GetGroups(result =>
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

		internal void GetGroups(Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			Relationships = Relationships.Where(r => r.RelationshipStatus != RelationshipStatus.ExistingRelationship).ToList();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.GroupMember.GetUserGroupsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					var responseIds = response.Select(r => r.Id).ToList();
					Relationships = Relationships.Where(r => !responseIds.Contains(r.Actor.Id)).ToList();
					Relationships.AddRange(response.Select(r => new GroupResponseRelationshipStatus(r, RelationshipStatus.ExistingRelationship)).ToList());
					Relationships = Relationships.OrderBy(r => r.Actor.Name).ToList();
					SUGARManager.unity.StopSpinner();
					onComplete(true);
				},
				exception =>
				{
					Debug.LogError($"Failed to get groups list. {exception}");
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
				SUGARManager.client.GroupMember.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					var responseIds = response.Select(r => r.Id).ToList();
					Relationships = Relationships.Where(r => !responseIds.Contains(r.Actor.Id)).ToList();
					Relationships.AddRange(response.Select(r => new GroupResponseRelationshipStatus(r, RelationshipStatus.PendingSentRequest)).ToList());
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
				SUGARManager.client.GroupMember.GetMemberRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					var responseIds = response.Select(r => r.Id).ToList();
					Relationships = Relationships.Where(r => !responseIds.Contains(r.Actor.Id)).ToList();
					Relationships.AddRange(response.Select(r => new GroupResponseRelationshipStatus(r, RelationshipStatus.PendingReceivedRequest)).ToList());
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
