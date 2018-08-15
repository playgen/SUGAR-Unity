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
			GetMembers(onComplete =>
			{
				if (_interface)
				{
					_interface.Display(onComplete);
				}
			});
		}

		internal void GetMembers(Action<bool> onComplete)
		{
			SUGARManager.unity.StartSpinner();
			Members.Clear();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.GroupMember.GetMembersAsync(CurrentGroup.Id,
				response =>
				{
					SUGARManager.userFriend.RefreshRelationships(result =>
					{
						if (result)
						{
							response = response.OrderBy(r => r.Name);
							foreach (var r in response)
							{
								var relationship = SUGARManager.userFriend.Relationships.FirstOrDefault(a => r.Id == a.Actor.Id)?.RelationshipStatus ?? RelationshipStatus.NoRelationship;
								Members.Add(new UserResponseRelationshipStatus(r, relationship));
							}
						}
						SUGARManager.unity.StopSpinner();
						onComplete(true);
					});
				},
				exception =>
				{
					Debug.LogError($"Failed to get friends list. {exception}");
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
			CurrentGroup = null;
			Members.Clear();
		}
	}
}