using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

public class LeaderboardListUserInterface : BaseLeaderboardListUserInterface
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

	protected override void ShowLeaderboards(bool loadingSuccess)
	{
		SUGARManager.Account.Hide();
		SUGARManager.Achievement.Hide();
		SUGARManager.Unity.EnableObject(gameObject);
		_errorText.text = string.Empty;
		if (_signinButton)
		{
			_signinButton.gameObject.SetActive(false);
		}
		_userButton.interactable = true;
		_groupButton.interactable = true;
		_combinedButton.interactable = true;
		var leaderboardList = SUGARManager.GameLeaderboard.Leaderboards[(int)_actorType].ToList();
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
				_leaderboardButtons[i].onClick.AddListener(delegate { SUGARManager.Leaderboard.Display(token); });
				_leaderboardButtons[i].gameObject.SetActive(true);
			}
		}
		_leaderboardType.text = _actorType == ActorType.Undefined ? Localization.Get("COMBINED") : Localization.Get(_actorType.ToString());
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		if (!loadingSuccess)
		{
			if (SUGARManager.CurrentUser == null)
			{
				_errorText.text = Localization.Get("NO_USER_ERROR");
				if (SUGARManager.Account.HasInterface && _signinButton)
				{
					_signinButton.gameObject.SetActive(true);
				}
			}
			else
			{
				_errorText.text = Localization.Get("LEADERBOARD_LIST_LOAD_ERROR");
			}
			_userButton.interactable = false;
			_groupButton.interactable = false;
			_combinedButton.interactable = false;
		}
		else if (leaderboardList.Count == 0)
		{
			_errorText.text = Localization.Get("NO_LEADERBOARD_LIST_ERROR");
		}
		DoBestFit();
	}

	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		ShowLeaderboards(true);
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