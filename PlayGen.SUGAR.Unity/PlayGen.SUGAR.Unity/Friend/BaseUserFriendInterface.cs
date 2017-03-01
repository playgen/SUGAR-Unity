using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseUserFriendInterface : BaseInterface
	{
		internal void Reload(bool loadingSuccess = true)
		{
			Show(loadingSuccess);
		}

		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
		}

		protected override string LoadErrorText()
		{
			return Localization.Get("FRIENDS_LOAD_ERROR");
		}

		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}

		protected void GetFriends()
		{
			SUGARManager.userFriend.GetFriends(Show);
		}

		protected void GetPendingSent()
		{
			SUGARManager.userFriend.GetPendingSent(Show);
		}

		protected void GetPendingReceived()
		{
			SUGARManager.userFriend.GetPendingReceived(Show);
		}

		protected void GetSearchResults(string search)
		{
			SUGARManager.userFriend.GetSearchResults(search, Show);
		}
	}
}
