using System.Linq;

using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

public class AchievementListInterface : BaseAchievementListInterface
{
	[SerializeField]
	private AchievementItemInterface[] _achievementItems;
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
		_closeButton.onClick.AddListener(delegate { SUGARManager.Unity.DisableObject(gameObject); });
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

	protected override void ShowAchievements(bool loadingSuccess)
	{
		SUGARManager.Account.Hide();
		SUGARManager.GameLeaderboard.Hide();
		SUGARManager.Leaderboard.Hide();
		SUGARManager.Unity.EnableObject(gameObject);
		_errorText.text = string.Empty;
		if (_signinButton)
		{
			_signinButton.gameObject.SetActive(false);
		}
		var achievementList = SUGARManager.Achievement.progress.Skip(_pageNumber * _achievementItems.Length).Take(_achievementItems.Length).ToList();
		if (!achievementList.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if (_pageNumber < 0)
		{
			UpdatePageNumber(1);
			return;
		}
		for (int i = 0; i < _achievementItems.Length; i++)
		{
			if (i >= achievementList.Count)
			{
				_achievementItems[i].Disable();
			}
			else
			{
				_achievementItems[i].SetText(achievementList[i].Name, Mathf.Approximately(achievementList[i].Progress, 1.0f));
			}
		}
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		_nextButton.interactable = SUGARManager.Achievement.progress.Count > (_pageNumber + 1) * _achievementItems.Length;
		if (!loadingSuccess)
		{
			if (SUGARManager.CurrentUser == null)
			{
				_errorText.text = Localization.Get("NO_USER_ERROR");
				if (SUGARManager.Account.hasInterface && _signinButton)
				{
					_signinButton.gameObject.SetActive(true);
				}
			}
			else
			{
				_errorText.text = Localization.Get("ACHIEVEMENT_LOAD_ERROR");
			}
		}
		else if (achievementList.Count == 0)
		{
			_errorText.text = Localization.Get("NO_ACHIEVEMENT_ERROR");
		}
		_achievementItems.Select(t => t.gameObject).BestFit();
	}

	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		ShowAchievements(true);
	}

	private void DoBestFit()
	{
		_achievementItems.Select(t => t.gameObject).BestFit();
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
	}

	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}
