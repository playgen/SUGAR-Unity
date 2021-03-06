﻿using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to group member lists.
	/// </summary>
	public abstract class BaseGroupMemberInterface : BaseInterface
	{
		/// <value>
		/// Text used for providing the group name for this list. Can be left null.
		/// </value>
		[Tooltip("Input field used for providing usernames. Required.")]
		[SerializeField]
		protected Text _groupName;

		/// <summary>
		/// Hides Account, Evaluation, Leaderboard, GameLeaderboard and UserFriend UI objects. Set groupName text to match name of CurrentGroup.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Evaluation.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			if (_groupName)
			{
				_groupName.text = SUGARManager.groupMember.CurrentGroup.Name;
			}
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no results are available.
		/// </summary>
		protected override void ErrorDraw(bool loadingSuccess)
		{
			base.ErrorDraw(loadingSuccess);
			if (loadingSuccess)
			{
				if (SUGARManager.groupMember.Members.Count == 0)
				{
					if (_errorText)
					{
						_errorText.text = NoResultsErrorText();
					}
				}
			}
		}

		/// <summary>
		/// Get error string from Localization with key "GROUPS_LOAD_ERROR" if there were issues loading the evaluation list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("GROUPS_MEMBER_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_RESULTS_ERROR" if there were no group members to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}
	}
}