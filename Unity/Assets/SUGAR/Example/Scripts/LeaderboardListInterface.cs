using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

public class LeaderboardListInterface : BaseLeaderboardListInterface
{
	[SerializeField]
	private Button[] _leaderboardButtons;
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

	protected override void Draw(bool loadingSuccess)
	{
		var leaderboardList = SUGARManager.GameLeaderboard.Leaderboards[SUGARManager.GameLeaderboard.CurrentActorType].ToList();
		_nextButton.interactable = leaderboardList.Count > (_pageNumber + 1) * _leaderboardButtons.Length;
		leaderboardList = leaderboardList.Skip(_pageNumber * _leaderboardButtons.Length).Take(_leaderboardButtons.Length).ToList();
		if (!leaderboardList.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if (_pageNumber < 0)
		{
			UpdatePageNumber(1);
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
				_leaderboardButtons[i].onClick.AddListener(delegate { SUGARManager.Leaderboard.Display(token, SUGARManager.Leaderboard.CurrentFilter); });
				_leaderboardButtons[i].gameObject.SetActive(true);
			}
		}
		_leaderboardType.text = SUGARManager.GameLeaderboard.CurrentActorType == ActorType.Undefined ? Localization.Get("COMBINED") : Localization.Get(SUGARManager.GameLeaderboard.CurrentActorType.ToString());
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		DoBestFit();
	}

	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		Show(true);
	}

	private void DoBestFit()
	{
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
	}

	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}