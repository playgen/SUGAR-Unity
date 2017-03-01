using System.Linq;

using PlayGen.SUGAR.Common.Shared;
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
		SUGARManager.Leaderboard.SetPositionCount(_leaderboardPositions.Length);
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

	protected override void Draw()
	{
		if (!SUGARManager.Leaderboard.CurrentStandings.Any() && _pageNumber > 0)
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
			if (i >= SUGARManager.Leaderboard.CurrentStandings.Count)
			{
				_leaderboardPositions[i].Disable();
			}
			else
			{
				_leaderboardPositions[i].SetText(SUGARManager.Leaderboard.CurrentStandings[i]);
			}
		}
		_leaderboardName.text = SUGARManager.Leaderboard.CurrentLeaderboard != null ? SUGARManager.Leaderboard.CurrentLeaderboard.Name : string.Empty;
		_leaderboardType.text = Localization.Get(SUGARManager.Leaderboard.CurrentFilter.ToString());
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		_nextButton.interactable = false;
		SUGARManager.Leaderboard.GetLeaderboardStandings(_pageNumber + 1, success => {}, result =>
		{
			_nextButton.interactable = result.ToList().Count > 0;
		});
		if (SUGARManager.Leaderboard.CurrentLeaderboard != null)
		{
			_nearButton.interactable = SUGARManager.CurrentUser != null && SUGARManager.Leaderboard.CurrentLeaderboard.ActorType == ActorType.User;
			_friendsButton.interactable = SUGARManager.CurrentUser != null && SUGARManager.Leaderboard.CurrentLeaderboard.ActorType == ActorType.User;
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
		SUGARManager.Leaderboard.Display(SUGARManager.Leaderboard.CurrentLeaderboard.Token, SUGARManager.Leaderboard.CurrentFilter, _pageNumber);
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