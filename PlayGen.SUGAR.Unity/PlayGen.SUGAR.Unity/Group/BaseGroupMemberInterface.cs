using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseGroupMemberInterface : BaseInterface
	{
		[SerializeField]
		protected Text _groupName;

		internal void Reload(bool loadingSuccess = true)
		{
			Show(loadingSuccess);
		}

		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			if (_groupName)
			{
				_groupName.text = SUGARManager.groupMember.CurrentGroup.Name;
			}
		}

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

		protected override string LoadErrorText()
		{
			return Localization.Get("GROUPS_LOAD_ERROR");
		}

		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}
	}
}