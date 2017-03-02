using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to friends lists
	/// </summary>
	public abstract class BaseUserFriendInterface : BaseInterface
	{
		internal void Reload(bool loadingSuccess = true)
		{
			Show(loadingSuccess);
		}

		/// <summary>
		/// Hides Account, GameLeaderboard, Leaderboard, Achievement, GroupMember and UserGroup UI objects.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
		}

		/// <summary>
		/// Get error string from Localization with key "FRIENDS_LOAD_ERROR" if there were issues loading the friends list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("FRIENDS_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_RESULTS_ERROR" if there were no friends to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}

		/// <summary>
		/// Get and display the friends list for the currently signed in user.
		/// </summary>
		protected void GetFriends()
		{
			SUGARManager.userFriend.GetFriends(Show);
		}

		/// <summary>
		/// Get and display the list of pending sent friend requests for the currently signed in user.
		/// </summary>
		protected void GetPendingSent()
		{
			SUGARManager.userFriend.GetPendingSent(Show);
		}

		/// <summary>
		/// Get and display the list of pending received friend requests for the currently signed in user.
		/// </summary>
		protected void GetPendingReceived()
		{
			SUGARManager.userFriend.GetPendingReceived(Show);
		}

		/// <summary>
		/// Get and display the search results for the provided string.
		/// </summary>
		protected void GetSearchResults(string search)
		{
			SUGARManager.userFriend.GetSearchResults(search, Show);
		}
	}
}
