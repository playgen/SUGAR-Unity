using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardUserInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _leaderboardName;
		[SerializeField]
		protected Text _leaderboardType;
		protected LeaderboardFilterType _filter;
		[SerializeField]
		protected Button _topButton;
		[SerializeField]
		protected Button _nearButton;
		[SerializeField]
		protected Button _friendsButton;
		[SerializeField]
		protected Text _errorText;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Button _signinButton;

		protected virtual void Awake()
		{
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
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(delegate { SUGARManager.unity.DisableObject(gameObject); });
			}
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		internal void Display(LeaderboardFilterType filter, IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess = true)
		{
			PreDisplay();
			_filter = filter;
			ShowLeaderboard(standings, loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected abstract void ShowLeaderboard(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess);

		private void AttemptSignIn()
		{
			SUGARManager.account.DisplayPanel(success =>
			{
				if (success)
				{
					OnSignIn();
				}
			});
		}

		protected abstract void OnSignIn();

		private void UpdateFilter(int filter)
		{
			_filter = (LeaderboardFilterType)filter;
			SUGARManager.leaderboard.GetLeaderboardStandings(_filter, 0, result =>
			{
				var standings = result.ToList();
				ShowLeaderboard(standings, true);
			});
		}
	}
}
