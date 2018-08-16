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
		public List<UserResponseRelationshipStatus> Members { get; } = new List<UserResponseRelationshipStatus>();

		/// <summary>
		/// Sets current group and gathers member list for that group. Displays UI interface if provided.
		/// </summary>
		/// <param name="group">The group which should be set to CurrentGroup</param>
		public void Display(ActorResponse group)
		{
			CurrentGroup = group;
			Members.Clear();
			GetGroupMembers(CurrentGroup, members =>
			{
				if (members != null)
				{
					Members.AddRange(members);
				}
				if (_interface)
				{
					_interface.Display(members != null);
				}
			});
		}

		/// <summary>
		/// Gather member list for the provided group.
		/// </summary>
		/// <param name="group">The group for which members will be gathered</param>
		/// <param name="members">Callback which will return the list of group members and their current relationship with the signed in user.</param>
		public void GetGroupMembers(ActorResponse group, Action<List<UserResponseRelationshipStatus>> members)
		{
			if (group != null)
			{
				GetGroupMembers(group.Id, members);
			}
			else
			{
				members(null);
			}
		}

		/// <summary>
		/// Gather member list for the group with the provided id.
		/// </summary>
		/// <param name="groupId">The id of the group for which members will be gathered</param>
		/// <param name="members">Callback which will return the list of group members and their current relationship with the signed in user.</param>
		public void GetGroupMembers(int groupId, Action<List<UserResponseRelationshipStatus>> members)
		{
			GetMembers(groupId, members);
		}

		internal void GetMembers(int groupId, Action<List<UserResponseRelationshipStatus>> members)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.GroupMember.GetMembersAsync(groupId,
				response =>
				{
					SUGARManager.userFriend.RefreshRelationships(result =>
					{
						var memberList = new List<UserResponseRelationshipStatus>();
						if (result)
						{
							response = response.OrderBy(r => r.Name);
							foreach (var r in response)
							{
								var relationship = SUGARManager.userFriend.Relationships.FirstOrDefault(a => r.Id == a.Actor.Id)?.RelationshipStatus ?? RelationshipStatus.NoRelationship;
								memberList.Add(new UserResponseRelationshipStatus(r, relationship));
							}
						}
						SUGARManager.unity.StopSpinner();
						members(memberList);
					});
				},
				exception =>
				{
					Debug.LogError($"Failed to get friends list. {exception}");
					SUGARManager.unity.StopSpinner();
					members(null);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				members(null);
			}
		}

		internal void ResetClient()
		{
			CurrentGroup = null;
			Members.Clear();
		}
	}
}