using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the interface related to displaying the latest standings for a leaderboard.
	/// </summary>
	public abstract class BaseLeaderboardInterface : BaseInterface
	{
		/// <value>
		/// Text used for displaying leaderboard name. Can be left null.
		/// </value>
		[Tooltip("Text used for displaying leaderboard name. Can be left null.")]
		[SerializeField]
		protected Text _leaderboardName;

		/// <value>
		/// Text used for displaying current leaderboard filter. Can be left null.
		/// </value>
		[Tooltip("Text used for displaying current leaderboard filter. Can be left null.")]
		[SerializeField]
		protected Text _leaderboardType;

		/// <value>
		/// Button used to change the current leaderboard filter to 'Top'. Can be left null.
		/// </value>
		[Tooltip("Button used to change the current leaderboard filter to 'Top'. Can be left null.")]
		[SerializeField]
		protected Button _topButton;

		/// <value>
		/// Button used to change the current leaderboard filter to 'Near'. Can be left null.
		/// </value>
		[Tooltip("Button used to change the current leaderboard filter to 'Near'. Can be left null.")]
		[SerializeField]
		protected Button _nearButton;

		/// <value>
		/// Button used to change the current leaderboard filter to 'Friends'. Can be left null.
		/// </value>
		[Tooltip("Button used to change the current leaderboard filter to 'Friends'. Can be left null.")]
		[SerializeField]
		protected Button _friendsButton;

		/// <value>
		/// Button used to change the current leaderboard filter to 'Group Members'. Can be left null.
		/// </value>
		[Tooltip("Button used to change the current leaderboard filter to 'Group Members'. Can be left null.")]
		[SerializeField]
		protected Button _membersButton;

		/// <value>
		/// Button used to change the current leaderboard filter to 'Alliances'. Can be left null.
		/// </value>
		[Tooltip("Button used to change the current leaderboard filter to 'Alliances'. Can be left null.")]
		[SerializeField]
		protected Button _alliancesButton;

		/// <value>
		/// Base Awake method adds onClick listeners for the close, signin, top, near, friends, group member and alliance filter buttons.
		/// </value>
		protected override void Awake()
		{
			base.Awake();
			_topButton?.onClick.AddListener(() => UpdateFilter(LeaderboardFilterType.Top));
			_nearButton?.onClick.AddListener(() => UpdateFilter(LeaderboardFilterType.Near));
			_friendsButton?.onClick.AddListener(() => UpdateFilter(LeaderboardFilterType.Friends));
			_membersButton?.onClick.AddListener(() => UpdateFilter(LeaderboardFilterType.GroupMembers));
			_alliancesButton?.onClick.AddListener(() => UpdateFilter(LeaderboardFilterType.Alliances));
		}

		/// <summary>
		/// Hides Account, Evaluation, UserFriend, GroupMember and UserGroup UI objects. Makes filter buttons interactable. Set leaderboard related text.
		/// </summary>
		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Evaluation.Hide();
			SUGARManager.UserFriend.Hide();
			SUGARManager.GroupMember.Hide();
			SUGARManager.UserGroup.Hide();

			SetInteractable(_topButton, IsValid(SUGARManager.CurrentUser, ActorType.User) || IsValid(SUGARManager.CurrentGroup, ActorType.Group));
			SetInteractable(_nearButton, IsValid(SUGARManager.CurrentUser, ActorType.User) || IsValid(SUGARManager.CurrentGroup, ActorType.Group));
			SetInteractable(_nearButton, IsValid(SUGARManager.CurrentUser, ActorType.User));
			SetInteractable(_membersButton, IsValid(SUGARManager.CurrentGroup, ActorType.User));
			SetInteractable(_alliancesButton, IsValid(SUGARManager.CurrentGroup, ActorType.Group));

			_topButton?.gameObject.SetActive(_topButton.interactable);
			_nearButton?.gameObject.SetActive(_nearButton.interactable);
			_friendsButton?.gameObject.SetActive(_friendsButton.interactable);
			_membersButton?.gameObject.SetActive(_membersButton.interactable);
			_alliancesButton?.gameObject.SetActive(_alliancesButton.interactable);

			if (_leaderboardName)
			{
				_leaderboardName.text = SUGARManager.leaderboard.CurrentLeaderboard != null ? SUGARManager.leaderboard.CurrentLeaderboard.Name : string.Empty;
			}
			if (_leaderboardType)
			{
				_leaderboardType.text = Localization.Get(SUGARManager.leaderboard.CurrentFilter.ToString());
			}
		}

		/// <summary>
		/// Used to set error text in case of no user being signed in, loading issues or if no results are available.
		/// Filter button interactable set to false if no user is signed in or loading issues occur.
		/// </summary>
		/// <param name="loadingSuccess">Was the data successfully loaded?</param>
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

		/// <summary>
		/// If the response actor isn't null, does the leaerboard ActorType match the actorType provided or the Combined ActorType
		/// </summary>
		/// <param name="response">Actor that would be used for the filter</param>
		/// <param name="actorType">Valid actorType for the leaderboard if response is used as the basis</param>
		protected bool IsValid(ActorResponse response, ActorType actorType)
		{
			return response != null && (SUGARManager.leaderboard.CurrentLeaderboard.ActorType == actorType || SUGARManager.leaderboard.CurrentLeaderboard.ActorType == ActorType.Undefined);
		}

		/// <summary>
		/// Set the button's interactable value
		/// </summary>
		/// <param name="button">Button to enable/disable the use of</param>
		/// <param name="interactable">Value to change the button's interactable boolean to</param>
		protected void SetInteractable(Button button, bool interactable = true)
		{
			if (button)
			{
				button.interactable = interactable;
			}
		}

		/// <summary>
		/// Change the leaderboard filter currently being used
		/// </summary>
		/// <param name="filter">The filter to use for display leaderboard standings</param>
		protected void UpdateFilter(LeaderboardFilterType filter)
		{
			SUGARManager.leaderboard.Display(SUGARManager.leaderboard.CurrentLeaderboard.Token, filter, SUGARManager.leaderboard.MultiplePerActor);
		}

		/// <summary>
		/// Change the multiple per actor setting currently being used
		/// </summary>
		/// <param name="multiplePerActor">Setting that determines if actors can appear on a leaderboard multiple times</param>
		protected void UpdateMultiplePerActor(bool multiplePerActor)
		{
			SUGARManager.leaderboard.Display(SUGARManager.leaderboard.CurrentLeaderboard.Token, SUGARManager.leaderboard.CurrentFilter, multiplePerActor);
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
