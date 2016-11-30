﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	public class LeaderboardListUserInterface : MonoBehaviour
	{

		[SerializeField]
		private Button[] _leaderboardButtons;
		[SerializeField]
		private Text _leaderboardType;
		[SerializeField]
		private Button _previousButton;
		[SerializeField]
		private Button _nextButton;
		[SerializeField]
		private Text _pageNumber;
		[SerializeField]
		private Button _userButton;
		[SerializeField]
		private Button _groupButton;
		[SerializeField]
		private Button _combinedButton;
		[SerializeField]
		private Button _closeButton;

		private void Awake()
		{
			_previousButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboard.UpdatePageNumber(-1); });
			_nextButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboard.UpdatePageNumber(1); });
			_userButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboard.UpdateFilter(1); });
			_groupButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboard.UpdateFilter(2); });
			_combinedButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboard.UpdateFilter(0); });
			_closeButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
		}

		internal void ShowLeaderboards(ActorType filter, IEnumerable<LeaderboardResponse> leaderboards, int pageNumber)
		{
			if (SUGARManager.CurrentUser == null)
			{
				return;
			}
			gameObject.SetActive(true);
			var leaderboardList = leaderboards.Skip(pageNumber * _leaderboardButtons.Length).Take(_leaderboardButtons.Length).ToList();
			if (!leaderboardList.Any() && pageNumber > 0)
			{
				SUGARManager.GameLeaderboard.UpdatePageNumber(-1);
				return;
			}
			for (int i = 0; i < _leaderboardButtons.Length; i++)
			{
				if (i >= leaderboardList.Count)
				{
					_leaderboardButtons[i].gameObject.SetActive(false);
				}
				else
				{
					_leaderboardButtons[i].onClick.RemoveAllListeners();
					_leaderboardButtons[i].GetComponentInChildren<Text>().text = leaderboardList[i].Name;
					var token = leaderboardList[i].Token;
					_leaderboardButtons[i].onClick.AddListener(delegate { SUGARManager.Leaderboard.SetLeaderboard(token); });
					_leaderboardButtons[i].gameObject.SetActive(true);
				}
			}
			_leaderboardType.text = filter == ActorType.Undefined ? "Combined" : filter.ToString();
			_pageNumber.text = "Page " + (pageNumber + 1);
			_previousButton.interactable = pageNumber > 0;
			_nextButton.interactable = leaderboardList.Count > pageNumber * _leaderboardButtons.Length;
		}
	}
}