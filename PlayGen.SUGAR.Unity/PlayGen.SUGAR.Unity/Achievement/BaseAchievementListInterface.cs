using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAchievementListInterface : BaseInterface
	{
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
		}

		protected override void ErrorDraw(bool loadingSuccess)
		{
			base.ErrorDraw(loadingSuccess);
			if (loadingSuccess)
			{
				if (SUGARManager.Achievement.Progress.Count == 0)
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
			return Localization.Get("ACHIEVEMENT_LOAD_ERROR");
		}

		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_ACHIEVEMENT_ERROR");
		}
	}
}