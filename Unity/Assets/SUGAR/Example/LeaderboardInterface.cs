using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;

public class LeaderboardInterface : BaseLeaderboardInterface
{
	[SerializeField]
	private LeaderboardPositionInterface[] _leaderboardPositions;
	[SerializeField]
	private Button _previousButton;
	[SerializeField]
	private Button _nextButton;
	[SerializeField]
	private Text _pageNumberText;
	private int _pageNumber;

	protected override void Awake()
	{
		base.Awake();
		SUGARManager.Leaderboard.SetPositionCount(_leaderboardPositions.Length - 1);
		_previousButton.onClick.AddListener(delegate { UpdatePageNumber(-1); });
		_nextButton.onClick.AddListener(delegate { UpdatePageNumber(1); });
	}

	private void OnEnable()
	{
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
		Localization.LanguageChange += OnLanguageChange;
	}

	private void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
		Localization.LanguageChange -= OnLanguageChange;
	}

	protected override void PreDisplay()
	{
		_pageNumber = 0;
	}

	protected override void DrawLeaderboard(IEnumerable<LeaderboardStandingsResponse> standings, bool loadingSuccess)
	{
		var standingsList = standings.ToList();
		if (!standingsList.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if (_pageNumber < 0)
		{
			UpdatePageNumber(1);
			return;
		}
		for (int i = 0; i < _leaderboardPositions.Length; i++)
		{
			if (i >= standingsList.Count)
			{
				_leaderboardPositions[i].Disable();
			}
			else
			{
				_leaderboardPositions[i].SetText(standingsList[i]);
			}
		}
		_leaderboardName.text = _currentLeaderboard != null ? _currentLeaderboard.Name : string.Empty;
		_leaderboardType.text = Localization.Get(_filter.ToString());
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		GetStandings(_filter, _pageNumber + 1, result =>
		{
			_nextButton.interactable = result.ToList().Count > 0;
		});
		if (_currentLeaderboard == null)
		{
			loadingSuccess = false;
		}
		else
		{
			_nearButton.interactable = SUGARManager.CurrentUser != null && _currentLeaderboard.ActorType == ActorType.User;
			_friendsButton.interactable = SUGARManager.CurrentUser != null && _currentLeaderboard.ActorType == ActorType.User;
		}
		_leaderboardPositions.Select(t => t.gameObject).BestFit();
	}

	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		GetStandings(_filter, _pageNumber, result =>
		{
			var standings = result.ToList();
			ShowLeaderboard(standings, true);
		});
	}

	private void DoBestFit()
	{
		_leaderboardPositions.Select(t => t.gameObject).BestFit();
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
	}

	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}