using System;
using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;
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
		/// Button used to change the current leaderboard filter to 'Group Members'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current leaderboard filter to 'Group Members'. Can be left null.")]
		[SerializeField]
		protected Button _membersButton;

		/// <summary>
		/// Button used to change the current leaderboard filter to 'Alliances'. Can be left null.
		/// </summary>
		[Tooltip("Button used to change the current leaderboard filter to 'Alliances'. Can be left null.")]
		[SerializeField]
		protected Button _alliancesButton;

		/// <summary>
		/// Base Awake method adds onClick listeners for the close, signin, top, near and friends filter buttons.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			_topButton?.onClick.AddListener(delegate { UpdateFilter(0); });
			_nearButton?.onClick.AddListener(delegate { UpdateFilter(1); });
			_friendsButton?.onClick.AddListener(delegate { UpdateFilter(2); });
			_membersButton?.onClick.AddListener(delegate { UpdateFilter(3); });
			_alliancesButton?.onClick.AddListener(delegate { UpdateFilter(4); });
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

			SetInteractable(_topButton, IsValid(SUGARManager.CurrentUser, ActorType.Group) || IsValid(SUGARManager.CurrentGroup, ActorType.User));
			SetInteractable(_nearButton, IsValid(SUGARManager.CurrentUser, ActorType.Group) || IsValid(SUGARManager.CurrentGroup, ActorType.User));
			SetInteractable(_nearButton, IsValid(SUGARManager.CurrentUser, ActorType.Group));
			SetInteractable(_membersButton, IsValid(SUGARManager.CurrentGroup, ActorType.Group));
			SetInteractable(_alliancesButton, IsValid(SUGARManager.CurrentGroup, ActorType.User));

			_topButton?.gameObject.SetActive(_topButton.interactable);
			_nearButton?.gameObject.SetActive(_nearButton.interactable);
			_friendsButton?.gameObject.SetActive(_friendsButton.interactable);
			_membersButton?.gameObject.SetActive(_membersButton.interactable);
			_alliancesButton?.gameObject.SetActive(_alliancesButton.interactable);
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no results are available.
		/// Filter button interactable set to false if no user is signed in or loading issues occur.
		/// </summary>
		protected override void ErrorDraw(bool loadingSuccess)
		{
			base.ErrorDraw(loadingSuccess);
			if (SUGARManager.UserSignedIn && SUGARManager.leaderboard.CurrentLeaderboard == null)
			{
				_errorText.text = Localization.Get("LEADERBOARD_LOAD_ERROR");
				loadingSuccess = false;
			}
			if (!loadingSuccess)
			{
				SetInteractable(_topButton, false);
				SetInteractable(_nearButton, false);
				SetInteractable(_friendsButton, false);
				SetInteractable(_membersButton, false);
				SetInteractable(_alliancesButton, false);
			}
			else if (!SUGARManager.leaderboard.CurrentStandings.Any())
			{
				if (_errorText)
				{
					_errorText.text = NoResultsErrorText();
				}
			}
		}

		private void SetInteractable(Button button, bool interactable = true)
		{
			if (button)
			{
				button.interactable = interactable;
			}
		}

		private bool IsValid(ActorResponse response, ActorType actorType)
		{
			return response != null && SUGARManager.Leaderboard.CurrentLeaderboard.ActorType != actorType;
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
