using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to group lists.
	/// </summary>
	[DisallowMultipleComponent]
	public class UserGroupUnityClient : BaseUnityClient<BaseUserGroupInterface>
	{
		/// <summary>
		/// List of groups that the currently signed in user is a member of.
		/// </summary>
		public List<ActorResponseAllowableActions> Groups { get; private set; } = new List<ActorResponseAllowableActions>();

		/// <summary>
		/// List of groups that the currently signed in user has applied to join.
		/// </summary>
		public List<ActorResponseAllowableActions> PendingSent { get; private set; } = new List<ActorResponseAllowableActions>();

		/// <summary>
		/// List of groups that matched the last search string.
		/// </summary>
		public List<ActorResponseAllowableActions> SearchResults { get; } = new List<ActorResponseAllowableActions>();

		private string _lastSearch;

		/// <summary>
		/// Gathers updated versions of each list and displays UI object if provided.
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

		private void RefreshLists(Action<bool> success)
		{
			GetGroups(result =>
			{
				GetPendingSent(result2 =>
				{
					GetSearchResults(_lastSearch, result3 =>
					{
						success(result && result2 && result3);
					});
				});
			});
		}

		/// <summary>
		/// Send group membership request to group with id provided. If reload is true, UI is also redrawn.
		/// </summary>
		public void AddGroup(int id, bool reload = true)
		{
			Add(id, result =>
			{
				if (reload)
				{
					_interface.Reload(result);
				}
			});
		}

		/// <summary>
		/// Cancel sent membership request to group with id provided. If reload is true, UI is also redrawn.
		/// </summary>
		public void ManageGroupRequest(int id, bool reload = true)
		{
			UpdateRequest(id, result =>
			{
				if (reload)
				{
					_interface.Reload(result);
				}
			});
		}

		/// <summary>
		/// Leave group with id provided. If reload is true, UI is also redrawn.
		/// </summary>
		public void RemoveGroup(int id, bool reload = true)
		{
			Remove(id, result =>
			{
				if (reload)
				{
					_interface.Reload(result);
				}
			});
		}

		/// <summary>
		/// Get group list for the currently signed in user.
		/// </summary>
		public void GetGroupsList(Action<bool> success)
		{
			GetGroups(success);
		}

		internal void GetGroups(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			Groups.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GroupMember.GetUserGroupsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					Groups = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					var error = "Failed to get groups list. " + exception.Message;
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
				SUGARManager.client.GroupMember.GetSentRequestsAsync(SUGARManager.CurrentUser.Id,
				response =>
				{
					PendingSent = response.Select(r => new ActorResponseAllowableActions(r, false, true)).ToList();
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					var error = "Failed to get list. " + exception.Message;
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
				SUGARManager.client.Group.GetAsync(searchString,
				response =>
				{
					var results = response.Select(r => (ActorResponse)r).Take(100).ToList();
					foreach (var r in results)
					{
						if (Groups.Any(p => p.Actor.Id == r.Id) || PendingSent.Any(p => p.Actor.Id == r.Id))
						{
							SearchResults.Add(new ActorResponseAllowableActions(r, false, false));
						}
						else
						{
							SearchResults.Add(new ActorResponseAllowableActions(r, true, false));
						}
					}
					SUGARManager.unity.StopSpinner();
					success(true);
				},
				exception =>
				{
					var error = "Failed to get list. " + exception.Message;
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
					AcceptorId = id,
					AutoAccept = true
				};
				SUGARManager.client.GroupMember.CreateMemberRequestAsync(relationship,
				response =>
				{
					RefreshLists(success);
				},
				exception =>
				{
					var error = "Failed to create group request. " + exception.Message;
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

		internal void UpdateRequest(int id, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
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
					RefreshLists(success);
				},
				exception =>
				{
					var error = "Failed to update group request. " + exception.Message;
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
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id,
					Accepted = false
				};
				SUGARManager.client.GroupMember.UpdateMemberAsync(relationship,
				() =>
				{
					RefreshLists(success);
				},
				exception =>
				{
					var error = "Failed to update group status. " + exception.Message;
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
