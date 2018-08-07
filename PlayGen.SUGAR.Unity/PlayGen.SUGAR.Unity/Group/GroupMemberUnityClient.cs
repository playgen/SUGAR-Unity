using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;

using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Use this for actions related to group member lists.
	/// </summary>
	[DisallowMultipleComponent]
	public class GroupMemberUnityClient : BaseUnityClient<BaseGroupMemberInterface>
	{
		/// <value>
		/// Currently selected/displayed group.
		/// </value>
		public ActorResponse CurrentGroup { get; private set; }

		/// <value>
		/// Members for the current group.
		/// </value>
		public List<ActorResponseAllowableActions> Members { get; } = new List<ActorResponseAllowableActions>();

		/// <summary>
		/// Sets current group and gathers member list for that group. Displays UI interface if provided.
		/// </summary>
		/// <param name="group">The group which should be set to CurrentGroup</param>
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

		/// <summary>
		/// Send group request to user with id provided. 
		/// </summary>
		/// <param name="id">The group id</param>
		/// <param name="reload">Whether the UI should reload on complete</param>
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
			if (SUGARManager.UserSignedIn)
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
								var canAdd = !(SUGARManager.userFriend.Friends.Any(p => p.Actor.Id == r.Id) 
									|| SUGARManager.userFriend.PendingReceived.Any(p => p.Actor.Id == r.Id) 
									|| SUGARManager.userFriend.PendingSent.Any(p => p.Actor.Id == r.Id) 
									|| r.Id == SUGARManager.CurrentUser.Id);

								Members.Add(new ActorResponseAllowableActions(r, canAdd, false));
							}
						}
						SUGARManager.unity.StopSpinner();
						success(true);
					});
				},
				exception =>
				{
					var error = "Failed to get friends list. " + exception.Message;
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

		internal void ResetClient()
		{
			CurrentGroup = null;
			Members.Clear();
		}
	}
}