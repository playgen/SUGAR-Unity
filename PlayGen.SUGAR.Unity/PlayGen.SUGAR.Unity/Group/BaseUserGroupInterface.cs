using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to user groups.
	/// </summary>
	public abstract class BaseUserGroupInterface : BaseInterface
	{
		internal void Reload(bool loadingSuccess = true)
		{
			Show(loadingSuccess);
		}

		/// <summary>
		/// Hides Account, Evaluation, Leaderboard, GameLeaderboard and UserFriend UI objects.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Evaluation.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GameLeaderboard.Hide();
			SUGARManager.Leaderboard.Hide();
		}

		/// <summary>
		/// Get error string from Localization with key "GROUPS_LOAD_ERROR" if there were issues loading the group list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("GROUPS_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_RESULTS_ERROR" if there were no groups to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_RESULTS_ERROR");
		}

		/// <summary>
		/// Get and display the list of groups the currently signed in user is in.
		/// </summary>
		protected void GetGroups()
		{
			SUGARManager.userGroup.GetGroups(Show);
		}

		/// <summary>
		/// Get and display the list of groups the currently signed in user has applied to join.
		/// </summary>
		protected void GetPendingSent()
		{
			SUGARManager.userGroup.GetPendingSent(Show);
		}

		/// <summary>
		/// Get and display the list of groups that match the search string provided.
		/// </summary>
		protected void GetSearchResults(string search)
		{
			SUGARManager.userGroup.GetSearchResults(search, Show);
		}
	}
}
