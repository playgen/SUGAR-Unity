using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	public class GroupMemberUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseGroupMemberInterface _groupMemberInterface;

		public bool IsActive => _groupMemberInterface && _groupMemberInterface.gameObject.activeInHierarchy;

		public ActorResponse CurrentGroup { get; private set; }
		public List<ActorResponseAllowableActions> Members { get; private set; } = new List<ActorResponseAllowableActions>();

		internal void CreateInterface(Canvas canvas)
		{
			if (_groupMemberInterface)
			{
				bool inScene = _groupMemberInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_groupMemberInterface.gameObject, canvas.transform, false);
					newInterface.name = _groupMemberInterface.name;
					_groupMemberInterface = newInterface.GetComponent<BaseGroupMemberInterface>();
				}
				_groupMemberInterface.gameObject.SetActive(false);
			}
		}

		public void Display(ActorResponse group)
		{
			if (_groupMemberInterface)
			{
				CurrentGroup = group;
				GetMembers(success =>
				{
					_groupMemberInterface.Display(success);
				});
			}
		}

		public void AddFriend(int id, bool reload = true)
		{
			SUGARManager.userFriend.Add(id, result =>
			{
				if (reload)
				{
					_groupMemberInterface.Reload(result);
				}
			});
		}

		public void ManageFriendRequest(int id, bool accept, bool reverse = false, bool reload = true)
		{
			SUGARManager.userFriend.UpdateRequest(id, accept, reverse, result =>
			{
				if (reload)
				{
					_groupMemberInterface.Reload(result);
				}
			});
		}

		public void RemoveFriend(int id, bool reload = true)
		{
			SUGARManager.userFriend.Remove(id, result =>
			{
				if (reload)
				{
					_groupMemberInterface.Reload(result);
				}
			});
		}

		internal void GetMembers(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			Members.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GroupMember.GetMembersAsync(CurrentGroup.Id,
				response =>
				{
					var results = response.Select(r => (ActorResponse)r).ToList();
					SUGARManager.userFriend.RefreshLists(result =>
					{
						if (result)
						{
							foreach (var r in results)
							{
								if (r.Id != SUGARManager.CurrentUser.Id)
								{
									if (SUGARManager.userFriend.Friends.Any(p => p.Actor.Id == r.Id) || SUGARManager.userFriend.PendingReceived.Any(p => p.Actor.Id == r.Id) || SUGARManager.userFriend.PendingSent.Any(p => p.Actor.Id == r.Id))
									{
										Members.Add(new ActorResponseAllowableActions(r, false, false));
									}
									else
									{
										Members.Add(new ActorResponseAllowableActions(r, true, false));
									}
								}
							}
						}
						SUGARManager.unity.StopSpinner();
						success(true);
					});
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

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_groupMemberInterface.gameObject);
			}
		}
	}
}