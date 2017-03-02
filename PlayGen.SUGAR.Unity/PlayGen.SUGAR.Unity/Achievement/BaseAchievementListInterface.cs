using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to achievement lists.
	/// </summary>
	public abstract class BaseAchievementListInterface : BaseInterface
	{
		/// <summary>
		/// Hides Account, GameLeaderboard, Leaderboard, UserFriend, GroupMember and UserGroup UI objects.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no results are available.
		/// </summary>
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

		/// <summary>
		/// Get error string from Localization with key "ACHIEVEMENT_LOAD_ERROR" if there were issues loading the achievement list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("ACHIEVEMENT_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_ACHIEVEMENT_ERROR" if there were no achievements to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_ACHIEVEMENT_ERROR");
		}
	}
}