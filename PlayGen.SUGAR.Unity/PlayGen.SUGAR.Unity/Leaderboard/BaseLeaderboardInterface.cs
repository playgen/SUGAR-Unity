using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardInterface : BaseInterface
	{
		[SerializeField]
		protected Text _leaderboardName;
		[SerializeField]
		protected Text _leaderboardType;
		[SerializeField]
		protected Button _topButton;
		[SerializeField]
		protected Button _nearButton;
		[SerializeField]
		protected Button _friendsButton;

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

		protected override void HideInterfaces()
		{
			SUGARManager.Account.Hide();
			SUGARManager.Achievement.Hide();
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

		protected override void ErrorDraw(bool loadingSuccess)
		{
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

		protected override string LoadErrorText()
		{
			return Localization.Get("LEADERBOARD_LOAD_ERROR");
		}

		protected override string NoResultsErrorText()
		{
			return Localization.Get("NO_LEADERBOARD_ERROR");
		}
	}
}
