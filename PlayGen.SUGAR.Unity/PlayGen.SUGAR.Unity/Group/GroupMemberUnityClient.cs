using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class GroupMemberUnityClient : BaseUnityClient<BaseGroupMemberInterface>
	{
		private ActorResponse _currentGroup;
		private readonly List<ActorResponseAllowableActions> _members = new List<ActorResponseAllowableActions>();

		public ActorResponse CurrentGroup => _currentGroup;
		public List<ActorResponseAllowableActions> Members => _members;

		public void Display(ActorResponse group)
		{
			_currentGroup = group;
			GetMembers(success =>
			{
				if (_interface)
				{
					_interface.Display(success);
				}
			});
		}

		public void AddFriend(int id, bool reload = true)
		{
			SUGARManager.userFriend.Add(id, result =>
			{
				if (reload)
				{
					GetMembers(success =>
					{
						_interface.Reload(success && result);
					});
				}
			});
		}

		internal void GetMembers(Action<bool> success)
		{
			SUGARManager.unity.StartSpinner();
			_members.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GroupMember.GetMembersAsync(_currentGroup.Id,
				response =>
				{
					SUGARManager.userFriend.RefreshLists(result =>
					{
						if (result)
						{
							foreach (var r in response)
							{
								if (SUGARManager.userFriend.Friends.Any(p => p.Actor.Id == r.Id) || SUGARManager.userFriend.PendingReceived.Any(p => p.Actor.Id == r.Id) || SUGARManager.userFriend.PendingSent.Any(p => p.Actor.Id == r.Id) || r.Id == SUGARManager.CurrentUser.Id)
								{
									_members.Add(new ActorResponseAllowableActions(r, false, false));
								}
								else
								{
									_members.Add(new ActorResponseAllowableActions(r, true, false));
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
	}
}