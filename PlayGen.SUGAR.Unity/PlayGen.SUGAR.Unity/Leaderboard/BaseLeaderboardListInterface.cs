using UnityEngine;
using UnityEngine.UI;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardListInterface : BaseInterface
	{
		[SerializeField]
		protected Text _leaderboardType;
		[SerializeField]
		protected Button _userButton;
		[SerializeField]
		protected Button _groupButton;
		[SerializeField]
		protected Button _combinedButton;

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

		protected override string LoadErrorText()
		{
			return Localization.Get("LEADERBOARD_LIST_LOAD_ERROR");
		}

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