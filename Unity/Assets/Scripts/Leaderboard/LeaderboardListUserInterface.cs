using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using SUGAR.Unity;

public class LeaderboardListUserInterface : MonoBehaviour {

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

	public void Awake()
	{
		_previousButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboards.UpdatePageNumber(-1); });
		_nextButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboards.UpdatePageNumber(1); });
		_userButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboards.UpdateFilter(1); });
		_groupButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboards.UpdateFilter(2); });
		_combinedButton.onClick.AddListener(delegate { SUGARManager.GameLeaderboards.UpdateFilter(0); });
		_closeButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
	}

	public void ShowLeaderboards(ActorType filter, IEnumerable<LeaderboardResponse> leaderboards, int pageNumber)
	{
		if (SUGARManager.CurrentUser == null)
		{
			return;
		}
		gameObject.SetActive(true);
		var leaderboardList = leaderboards.Skip(pageNumber * _leaderboardButtons.Length).Take(_leaderboardButtons.Length).ToList();
		if (!leaderboardList.Any() && pageNumber > 0)
		{
			SUGARManager.GameLeaderboards.UpdatePageNumber(-1);
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
	}
}
