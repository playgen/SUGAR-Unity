using PlayGen.SUGAR.Contracts;
using System;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// The different relationship states two actors can be in related to each other
	/// </summary>
	public enum RelationshipStatus
	{
		NoRelationship,
		PendingReceivedRequest,
		PendingSentRequest,
		ExistingRelationship
	}

	/// <summary>
	/// ActorResponse with additional information on the relationship between the current user and the actor.
	/// </summary>
	public abstract class ActorResponseRelationshipStatus<T> where T : ActorResponse
	{
		/// <summary>
		/// ActorResponse contains the actor ID and Name.
		/// </summary>
		public T Actor { get; set; }
		/// <summary>
		/// Current status of the relationship between this actor and the current user
		/// </summary>
		public RelationshipStatus RelationshipStatus { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public ActorResponseRelationshipStatus(T actor, RelationshipStatus status)
		{
			Actor = actor;
			RelationshipStatus = status;
		}
	}

	/// <summary>
	/// ActorResponse with additional information on the relationship between the current user and the actor.
	/// </summary>
	public class UserResponseRelationshipStatus : ActorResponseRelationshipStatus<ActorResponse>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public UserResponseRelationshipStatus(ActorResponse actor, RelationshipStatus status) : base(actor, status)
		{
		}

		/// <summary>
		/// Send a relationship request to this User.
		/// </summary>
		/// <param name="onComplete">Callback for if the request was successfully created</param>
		/// <param name="autoAccept">**Optional** Should the request be automatically accepted</param>
		public void Add(Action<bool> onComplete, bool autoAccept = true)
		{
			Add(Actor.Id, onComplete, autoAccept);
		}

		internal static void Add(int id, Action<bool> onComplete, bool autoAccept = true)
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
					SUGARManager.userFriend.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to create friend request. {exception}");
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

		/// <summary>
		/// Accept or decline a received relationship request from this User.
		/// </summary>
		/// <param name="accept">Accept or decline the request</param>
		/// <param name="onComplete">Callback for if the relationship was successfully updated</param>
		public void UpdateRequest(bool accept, Action<bool> onComplete)
		{
			UpdateRequest(Actor.Id, accept, onComplete);
		}

		internal static void UpdateRequest(int id, bool accept, Action<bool> onComplete)
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
				SUGARManager.client.UserFriend.UpdateFriendRequestAsync(relationship,
				() =>
				{
					SUGARManager.userFriend.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update friend request. {exception}");
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

		/// <summary>
		/// Cancel a relationship request to this User.
		/// </summary>
		/// <param name="onComplete">Callback for if the relationship request was successfully cancelled</param>
		public void CancelSentRequest(Action<bool> onComplete)
		{
			CancelSentRequest(Actor.Id, onComplete);
		}

		internal static void CancelSentRequest(int id, Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				var relationship = new RelationshipStatusUpdate
				{
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id,
					Accepted = false
				};
				SUGARManager.client.UserFriend.UpdateFriendRequestAsync(relationship,
				() =>
				{
					SUGARManager.userFriend.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update friend request. {exception}");
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

		/// <summary>
		/// Cancel the relationship with this User.
		/// </summary>
		/// <param name="onComplete">Callback for if the relationship request was successfully cancelled</param>
		public void Remove(Action<bool> onComplete)
		{
			Remove(Actor.Id, onComplete);
		}

		internal static void Remove(int id, Action<bool> onComplete)
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
					SUGARManager.userFriend.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update friend status. {exception}");
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
	}

	/// <summary>
	/// ActorResponse with additional information on the relationship between the current user and the actor.
	/// </summary>
	public class GroupResponseRelationshipStatus : ActorResponseRelationshipStatus<ActorResponse>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GroupResponseRelationshipStatus(ActorResponse actor, RelationshipStatus status) : base(actor, status)
		{
		}

		/// <summary>
		/// Send a relationship request to this Group.
		/// </summary>
		/// <param name="onComplete">Callback for if the request was successfully created</param>
		/// <param name="autoAccept">**Optional** Should the request be automatically accepted</param>
		public void Add(Action<bool> onComplete, bool autoAccept = true)
		{
			Add(Actor.Id, onComplete, autoAccept);
		}

		internal static void Add(int id, Action<bool> onComplete, bool autoAccept = true)
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
				SUGARManager.client.GroupMember.CreateMemberRequestAsync(relationship,
				response =>
				{
					SUGARManager.userGroup.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to create friend request. {exception}");
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

		/// <summary>
		/// Accept or decline a received relationship request from this Group.
		/// </summary>
		/// <param name="accept">Accept or decline the request</param>
		/// <param name="onComplete">Callback for if the relationship was successfully updated</param>
		public void UpdateRequest(bool accept, Action<bool> onComplete)
		{
			UpdateRequest(Actor.Id, accept, onComplete);
		}

		internal static void UpdateRequest(int id, bool accept, Action<bool> onComplete)
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
				SUGARManager.client.GroupMember.UpdateMemberRequestAsync(relationship,
				() =>
				{
					SUGARManager.userGroup.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update group request. {exception}");
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

		/// <summary>
		/// Cancel a relationship request to this Group.
		/// </summary>
		/// <param name="onComplete">Callback for if the relationship request was successfully cancelled</param>
		public void CancelSentRequest(Action<bool> onComplete)
		{
			CancelSentRequest(Actor.Id, onComplete);
		}

		internal static void CancelSentRequest(int id, Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				var relationship = new RelationshipStatusUpdate
				{
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id,
					Accepted = false
				};
				SUGARManager.client.GroupMember.UpdateMemberRequestAsync(relationship,
				() =>
				{
					SUGARManager.userGroup.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update group request. {exception}");
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

		/// <summary>
		/// Cancel the relationship with this Group.
		/// </summary>
		/// <param name="onComplete">Callback for if the relationship request was successfully cancelled</param>
		public void Remove(Action<bool> onComplete)
		{
			Remove(Actor.Id, onComplete);
		}

		internal static void Remove(int id, Action<bool> onComplete)
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
				SUGARManager.client.GroupMember.UpdateMemberAsync(relationship,
				() =>
				{
					SUGARManager.userGroup.RefreshRelationships(onComplete);
				},
				exception =>
				{
					Debug.LogError($"Failed to update group status. {exception}");
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
	}
}
