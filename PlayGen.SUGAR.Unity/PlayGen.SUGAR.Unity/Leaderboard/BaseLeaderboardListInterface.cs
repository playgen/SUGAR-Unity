using UnityEngine;
using UnityEngine.UI;
using PlayGen.SUGAR.Common;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to getting all leaderboards for a game
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
				_userButton.onClick.AddListener(delegate { UpdateFilter(1); });
			}
			if (_groupButton)
			{
				_groupButton.onClick.AddListener(delegate { UpdateFilter(2); });
			}
			if (_combinedButton)
			{
				_combinedButton.onClick.AddListener(delegate { UpdateFilter(0); });
			}
		}

		/// <summary>
		/// Hides Account, Achievement, UserFriend, GroupMember and UserGroup UI objects. Makes filter buttons interactable.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
			if (_userButton)
			{
				_userButton.interactable = true;
			}
			if (_groupButton)
			{
				_groupButton.interactable = true;
			}
			if (_combinedButton)
			{
				_combinedButton.interactable = true;
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

		private void UpdateFilter(int filter)
		{
			SUGARManager.gameLeaderboard.SetFilter((ActorType)filter);
			Display();
		}
	}
}