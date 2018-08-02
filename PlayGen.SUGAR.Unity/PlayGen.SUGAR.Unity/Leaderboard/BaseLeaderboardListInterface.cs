using PlayGen.SUGAR.Common;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the interface related to displaying a list of leaderboards.
	/// </summary>
	public abstract class BaseLeaderboardListInterface : BaseInterface
	{
		/// <summary>
		/// Text used for displaying current leaderboard type. Can be left null.
		/// </summary>
		[Tooltip("Text used for displaying current leaderboard type. Can be left null.")]
		[SerializeField]
		protected Text _leaderboardType;

		/// <summary>
		/// Button used to change the current actor type filter to 'User'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current actor type filter to 'User'. Can be left null.")]
		[SerializeField]
		protected Button _userButton;

		/// <summary>
		/// Button used to change the current actor type filter to 'Group'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current actor type filter to 'Group'. Can be left null.")]
		[SerializeField]
		protected Button _groupButton;

		/// <summary>
		/// Button used to change the current actor type filter to 'Combined'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current actor type filter to 'Combined'. Can be left null.")]
		[SerializeField]
		protected Button _combinedButton;

		/// <summary>
		/// Base Awake method adds onClick listeners for the close, signin, user, group and combined filter buttons.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			if (_userButton)
			{
				_userButton.onClick.AddListener(() => UpdateFilter(ActorType.User));
			}
			if (_groupButton)
			{
				_groupButton.onClick.AddListener(() => UpdateFilter(ActorType.Group));
			}
			if (_combinedButton)
			{
				_combinedButton.onClick.AddListener(() => UpdateFilter(ActorType.Undefined));
			}
		}

		/// <summary>
		/// Hides Account, Evaluation, UserFriend, GroupMember and UserGroup UI objects. Makes filter buttons interactable. Set leaderboard type related text.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Evaluation.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
			if (_userButton)
			{
				_userButton.interactable = true;
			}
			if (_groupButton)
			{
				_groupButton.interactable = SUGARManager.CurrentGroup != null;
			}
			if (_combinedButton)
			{
				_combinedButton.interactable = true;
			}

			if (_leaderboardType)
			{
				_leaderboardType.text = SUGARManager.GameLeaderboard.CurrentActorType == ActorType.Undefined ? Localization.Get("COMBINED") : Localization.Get(SUGARManager.GameLeaderboard.CurrentActorType.ToString());
			}
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no leaderboards are available.
		/// Filter button interactable set to false if no user is signed in or loading issues occur.
		/// </summary>
		protected override void ErrorDraw(bool loadingSuccess)
		{
			base.ErrorDraw(loadingSuccess);
			if (!loadingSuccess)
			{
				if (_userButton)
				{
					_userButton.interactable = false;
				}
				if (_groupButton)
				{
					_groupButton.interactable = false;
				}
				if (_combinedButton)
				{
					_combinedButton.interactable = false;
				}
			}
			else if (SUGARManager.gameLeaderboard.Leaderboards[SUGARManager.gameLeaderboard.CurrentActorType].Count == 0)
			{
				if (_errorText)
				{
					_errorText.text = NoResultsErrorText();
				}
			}
		}

		/// <summary>
		/// Get error string from Localization with key "LEADERBOARD_LIST_LOAD_ERROR" if there were issues loading the leaderboard list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("LEADERBOARD_LIST_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_LEADERBOARD_LIST_ERROR" if there were no leaderboards to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_LEADERBOARD_LIST_ERROR");
		}

		/// <summary>
		/// Chnage the filter currently being used to get leaderboard for a particular type of actor
		/// </summary>
		/// <param name="filter">The filter to use for display leaderboard standings</param>
		protected void UpdateFilter(ActorType filter)
		{
			SUGARManager.gameLeaderboard.SetFilter(filter);
			Display();
		}
	}
}