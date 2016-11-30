using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class LeaderboardUserInterface : MonoBehaviour
	{
		[SerializeField]
		private Text _leaderboardName;
		[SerializeField]
		private Text _leaderboardType;
		[SerializeField]
		private LeaderboardPositionInterface[] _leaderboardPositions;
		[SerializeField]
		private Button _previousButton;
		[SerializeField]
		private Button _nextButton;
		[SerializeField]
		private Text _pageNumber;
		[SerializeField]
		private Button _topButton;
		[SerializeField]
		private Button _nearButton;
		[SerializeField]
		private Button _friendsButton;
		[SerializeField]
		private Button _closeButton;

		private void Awake()
		{
			_previousButton.onClick.AddListener(delegate { SUGARManager.Leaderboard.UpdatePageNumber(-1); });
			_nextButton.onClick.AddListener(delegate { SUGARManager.Leaderboard.UpdatePageNumber(1); });
			_topButton.onClick.AddListener(delegate { SUGARManager.Leaderboard.UpdateFilter(0); });
			_nearButton.onClick.AddListener(delegate { SUGARManager.Leaderboard.UpdateFilter(1); });
			_friendsButton.onClick.AddListener(delegate { SUGARManager.Leaderboard.UpdateFilter(2); });
			_closeButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
		}

		internal int GetPossiblePositionCount()
		{
			return _leaderboardPositions.Length;
		}

		internal void ShowLeaderboard(LeaderboardResponse leaderboard, LeaderboardFilterType filter, IEnumerable<LeaderboardStandingsResponse> standings, int pageNumber)
		{
			if (SUGARManager.CurrentUser == null || standings == null)
			{
				return;
			}
			gameObject.SetActive(true);
			var standingsList = standings.ToList();
			if (!standingsList.Any() && pageNumber > 0)
			{
				SUGARManager.Leaderboard.UpdatePageNumber(-1);
				return;
			}
			for (int i = 0; i < _leaderboardPositions.Length; i++)
			{
				if (i >= standingsList.Count)
				{
					_leaderboardPositions[i].Disbale();
				} else
				{
					_leaderboardPositions[i].SetText(standingsList[i]);
				}
			}
			_leaderboardName.text = leaderboard.Name;
			_leaderboardType.text = filter.ToString();
			_pageNumber.text = "Page " + (pageNumber + 1);
			_previousButton.interactable = pageNumber > 0;
			_nextButton.interactable = standingsList.Count > _leaderboardPositions.Length;
			_nearButton.interactable = SUGARManager.CurrentUser != null && leaderboard.ActorType == ActorType.User;
			_friendsButton.interactable = SUGARManager.CurrentUser != null && leaderboard.ActorType == ActorType.User;
		}
	}
}
