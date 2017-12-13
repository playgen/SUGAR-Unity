using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to leaderboards
	/// </summary>
	public abstract class BaseLeaderboardInterface : BaseInterface
	{
		/// <summary>
		/// Text used for displaying leaderboard name. Can be left null.
		/// </summary>
		[Tooltip("Text used for displaying leaderboard name. Can be left null.")]
		[SerializeField]
		protected Text _leaderboardName;

		/// <summary>
		/// Text used for displaying current leaderboard filter. Can be left null.
		/// </summary>
		[Tooltip("Text used for displaying current leaderboard filter. Can be left null.")]
		[SerializeField]
		protected Text _leaderboardType;

		/// <summary>
		/// Button used to change the current leaderboard filter to 'Top'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current leaderboard filter to 'Top'. Can be left null.")]
		[SerializeField]
		protected Button _topButton;

		/// <summary>
		/// Button used to change the current leaderboard filter to 'Near'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current leaderboard filter to 'Near'. Can be left null.")]
		[SerializeField]
		protected Button _nearButton;

		/// <summary>
		/// Button used to change the current leaderboard filter to 'Friends'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current leaderboard filter to 'Friends'. Can be left null.")]
		[SerializeField]
		protected Button _friendsButton;

		/// <summary>
		/// Base Awake method adds onClick listeners for the close, signin, top, near and friends filter buttons.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			if (_topButton)
			{
				_topButton.onClick.AddListener(delegate { UpdateFilter(0); });
			}
			if (_nearButton)
			{
				_nearButton.onClick.AddListener(delegate { UpdateFilter(1); });
			}
			if (_friendsButton)
			{
				_friendsButton.onClick.AddListener(delegate { UpdateFilter(2); });
			}
		}

		/// <summary>
		/// Hides Account, Evaluation, UserFriend, GroupMember and UserGroup UI objects. Makes filter buttons interactable.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Evaluation.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();
			if (_topButton)
			{
				_topButton.interactable = true;
			}
			if (_nearButton)
			{
				_nearButton.interactable = true;
			}
			if (_friendsButton)
			{
				_friendsButton.interactable = true;
			}
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no results are available.
		/// Filter button interactable set to false if no user is signed in or loading issues occur.
		/// </summary>
		protected override void ErrorDraw(bool loadingSuccess)
		{
			if (SUGARManager.leaderboard.CurrentLeaderboard == null)
			{
				_errorText.text = Localization.Get("LEADERBOARD_LOAD_ERROR");
				loadingSuccess = false;
			}
			if (!loadingSuccess)
			{
				if (_topButton)
				{
					_topButton.interactable = false;
				}
				if (_nearButton)
				{
					_nearButton.interactable = false;
				}
				if (_friendsButton)
				{
					_friendsButton.interactable = false;
				}
			}
			else if (!SUGARManager.leaderboard.CurrentStandings.Any())
			{
				if (_errorText)
				{
					_errorText.text = NoResultsErrorText();
				}
			}
		}

		private void UpdateFilter(int filter)
		{
			SUGARManager.leaderboard.Display(SUGARManager.leaderboard.CurrentLeaderboard.Token, (LeaderboardFilterType)filter);
		}

		/// <summary>
		/// Get error string from Localization with key "LEADERBOARD_LOAD_ERROR" if there were issues loading the leaderboard standings list.
		/// </summary>
		protected override string LoadErrorText()
		{
			return Localization.Get("LEADERBOARD_LOAD_ERROR");
		}

		/// <summary>
		/// Get error string from Localization with key "NO_LEADERBOARD_ERROR" if there were no leaderboard standings to display.
		/// </summary>
		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_LEADERBOARD_ERROR");
		}
	}
}
