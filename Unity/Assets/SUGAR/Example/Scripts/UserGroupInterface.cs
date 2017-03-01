using PlayGen.SUGAR.Unity;
using System.Collections.Generic;
using System.Linq;

using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

public class UserGroupInterface : BaseUserGroupInterface
{
	[SerializeField]
	private GroupListItemInterface[] _groupItems;
	[SerializeField]
	protected Text _listTypeText;
	private int _listType;
	[SerializeField]
	protected Button _groupsButton;
	[SerializeField]
	protected Button _sentButton;
	[SerializeField]
	protected Button _searchButton;
	[SerializeField]
	protected GameObject _searchArea;
	[SerializeField]
	protected InputField _searchInput;
	[SerializeField]
	protected Button _searchTextButton;
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
		_groupsButton.onClick.AddListener(delegate { SetListType(0); });
		_sentButton.onClick.AddListener(delegate { SetListType(1); });
		_searchButton.onClick.AddListener(delegate { SetListType(2); });
		_groupsButton.onClick.AddListener(GetGroups);
		_sentButton.onClick.AddListener(GetPendingSent);
		_searchTextButton.onClick.AddListener(delegate { GetSearchResults(_searchInput.text); });
		_previousButton.onClick.AddListener(delegate { UpdatePageNumber(-1); });
		_nextButton.onClick.AddListener(delegate { UpdatePageNumber(1); });
	}

	private void OnEnable()
	{
		_searchInput.text = string.Empty;
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
		_listType = 0;
	}

	protected override void Draw(bool loadingSuccess)
	{
		var actorList = new List<ActorResponseAllowableActions>();
		_searchArea.SetActive(false);
		switch (_listType)
		{
			case 0:
				actorList = SUGARManager.UserGroup.Groups;
				_listTypeText.text = Localization.Get("GROUP_LIST");
				break;
			case 1:
				actorList = SUGARManager.UserGroup.PendingSent;
				_listTypeText.text = Localization.Get("PENDING_SENT");
				break;
			case 2:
				actorList = SUGARManager.UserGroup.SearchResults;
				_listTypeText.text = Localization.Get("SEARCH");
				_searchArea.SetActive(true);
				_groupItems[0].gameObject.SetActive(false);
				break;
		}
		var length = _listType == 2 ? _groupItems.Length - 1 : _groupItems.Length;
		_nextButton.interactable = actorList.Count > (_pageNumber + 1) * length;
		actorList = actorList.Skip(_pageNumber * length).Take(length).ToList();
		if (!actorList.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if (_pageNumber < 0)
		{
			UpdatePageNumber(1);
			return;
		}
		for (int i = _listType == 2 ? 1 : 0; i < _groupItems.Length; i++)
		{
			if (i - (_listType == 2 ? 1 : 0) >= actorList.Count)
			{
				_groupItems[i].gameObject.SetActive(false);
			}
			else
			{
				_groupItems[i].SetText(actorList[i - (_listType == 2 ? 1 : 0)], _listType == 1, _listType == 0);
			}
		}
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		if (!actorList.Any())
		{
			if (_errorText)
			{
				_errorText.text = NoResultsErrorText();
			}
		}
		DoBestFit();
	}

	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	private void SetListType(int type)
	{
		_listType = type;
		Show(true);
	}

	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		Show(true);
	}

	private void DoBestFit()
	{
		GetComponentsInChildren<Button>(true).Where(t => !t.GetComponentInParent<FriendsListItemInterface>()).Select(t => t.gameObject).BestFit();
	}

	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}