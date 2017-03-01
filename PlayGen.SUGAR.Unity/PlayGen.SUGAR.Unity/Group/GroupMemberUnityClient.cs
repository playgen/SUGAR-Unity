using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public class GroupMemberUnityClient : BaseUnityClient<BaseGroupMemberInterface>
	{
		public ActorResponse CurrentGroup { get; private set; }
		public List<ActorResponseAllowableActions> Members { get; } = new List<ActorResponseAllowableActions>();

		public void Display(ActorResponse group)
		{
			CurrentGroup = group;
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
			Members.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.GroupMember.GetMembersAsync(CurrentGroup.Id,
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
									Members.Add(new ActorResponseAllowableActions(r, false, false));
								}
								else
								{
									Members.Add(new ActorResponseAllowableActions(r, true, false));
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