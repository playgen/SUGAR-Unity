using PlayGen.SUGAR.Contracts;
using System;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public enum RelationshipStatus
	{
		NoRelationship,
		PendingReceivedRequest,
		PendingSentRequest,
		ExistingRelationship
	}

	/// <summary>
	/// ActorResponse with additional information on if the current user can add and remove them.
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

		public ActorResponseRelationshipStatus(T actor, RelationshipStatus status)
		{
			Actor = actor;
			RelationshipStatus = status;
		}
	}

	public class UserResponseRelationshipStatus : ActorResponseRelationshipStatus<ActorResponse>
	{
		public UserResponseRelationshipStatus(ActorResponse actor, RelationshipStatus status) : base(actor, status)
		{
		}

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

	public class GroupResponseRelationshipStatus : ActorResponseRelationshipStatus<ActorResponse>
	{
		public GroupResponseRelationshipStatus(ActorResponse actor, RelationshipStatus status) : base(actor, status)
		{
		}

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
