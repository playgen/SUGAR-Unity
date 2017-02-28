﻿using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class UserGroupUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseUserGroupInterface _userGroupInterface;

		public bool IsActive => _userGroupInterface && _userGroupInterface.gameObject.activeInHierarchy;

		public List<ActorResponseAllowableActions> Groups { get; private set; } = new List<ActorResponseAllowableActions>();
		public List<ActorResponseAllowableActions> PendingSent { get; private set; } = new List<ActorResponseAllowableActions>();
		private string _lastSearch;
		public List<ActorResponseAllowableActions> SearchResults { get; private set; } = new List<ActorResponseAllowableActions>();

		internal void CreateInterface(Canvas canvas)
		{
			if (_userGroupInterface)
			{
				bool inScene = _userGroupInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_userGroupInterface.gameObject, canvas.transform, false);
					newInterface.name = _userGroupInterface.name;
					_userGroupInterface = newInterface.GetComponent<BaseUserGroupInterface>();
				}
				_userGroupInterface.gameObject.SetActive(false);
			}
		}

		public void Display()
		{
			if (_userGroupInterface)
			{
				RefreshLists(success =>
				{
					_userGroupInterface.Display(success);
				});
			}
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

		public void AddGroup(int id, bool reload = true)
		{
			Add(id, result =>
			{
				if (reload)
				{
					_userGroupInterface.Reload(result);
				}
			});
		}

		public void ManageGroupRequest(int id, bool accept, bool reload = true)
		{
			UpdateRequest(id, accept, result =>
			{
				if (reload)
				{
					_userGroupInterface.Reload(result);
				}
			});
		}

		public void RemoveGroup(int id, bool reload = true)
		{
			Remove(id, result =>
			{
				if (reload)
				{
					_userGroupInterface.Reload(result);
				}
			});
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
					string error = "Failed to get groups list. " + exception.Message;
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
				SUGARManager.client.Group.GetAsync(searchString,
				response =>
				{
					var results = response.Select(r => (ActorResponse)r).Take(100).ToList();
					foreach (var r in results)
					{
						if (r.Id != SUGARManager.CurrentUser.Id)
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
				SUGARManager.client.GroupMember.CreateMemberRequestAsync(relationship,
				response =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to create group request. " + exception.Message;
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

		internal void UpdateRequest(int id, bool accept, Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.CurrentUser != null)
			{
				var relationship = new RelationshipStatusUpdate
				{
					RequestorId = SUGARManager.CurrentUser.Id,
					AcceptorId = id,
					Accepted = accept
				};
				SUGARManager.client.GroupMember.UpdateMemberRequestAsync(relationship,
				() =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to update group request. " + exception.Message;
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
				SUGARManager.client.GroupMember.UpdateMemberAsync(relationship,
				() =>
				{
					RefreshLists(success);
					SUGARManager.unity.StopSpinner();
				},
				exception =>
				{
					string error = "Failed to update group status. " + exception.Message;
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
				SUGARManager.unity.DisableObject(_userGroupInterface.gameObject);
			}
		}
	}
}
