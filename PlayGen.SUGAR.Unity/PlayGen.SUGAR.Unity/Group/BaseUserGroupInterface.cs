using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseUserGroupInterface : BaseInterface
	{
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
		}

		protected override string LoadErrorText()
		{
			return Localization.Get("GROUPS_LOAD_ERROR");
		}

		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}

		protected void GetGroups()
		{
			SUGARManager.userGroup.GetGroups(Show);
		}

		protected void GetPendingSent()
		{
			SUGARManager.userGroup.GetPendingSent(Show);
		}

		protected void GetSearchResults(string search)
		{
			SUGARManager.userGroup.GetSearchResults(search, Show);
		}
	}
}
