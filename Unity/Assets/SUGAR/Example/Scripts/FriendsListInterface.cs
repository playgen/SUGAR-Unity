using System.Collections.Generic;

using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FriendsListInterface : BaseUserFriendInterface {
	[SerializeField]
	private FriendsListItemInterface[] _friendItems;
	[SerializeField]
	protected Text _listTypeText;
	private int _listType;
	[SerializeField]
	protected Button _friendsButton;
	[SerializeField]
	protected Button _requestButton;
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
		_friendsButton.onClick.AddListener(delegate { SetListType(0); });
		_requestButton.onClick.AddListener(delegate { SetListType(1); });
		_sentButton.onClick.AddListener(delegate { SetListType(2); });
		_searchButton.onClick.AddListener(delegate { SetListType(3); });
		_friendsButton.onClick.AddListener(GetFriends);
		_requestButton.onClick.AddListener(GetPendingReceived);
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

	protected override void Draw()
	{
		var actorList = new List<ActorResponseAllowableActions>();
		_searchArea.SetActive(false);
		switch (_listType)
		{
			case 0:
				actorList = SUGARManager.UserFriend.Friends;
				_listTypeText.text = Localization.Get("FRIENDS_LIST");
				break;
			case 1:
				actorList = SUGARManager.UserFriend.PendingReceived;
				_listTypeText.text = Localization.Get("FRIENDS_PENDING_REQUESTS");
				break;
			case 2:
				actorList = SUGARManager.UserFriend.PendingSent;
				_listTypeText.text = Localization.Get("FRIENDS_PENDING_SENT");
				break;
			case 3:
				actorList = SUGARManager.UserFriend.SearchResults;
				_listTypeText.text = Localization.Get("SEARCH");
				_searchArea.SetActive(true);
				_friendItems[0].gameObject.SetActive(false);
				break;
		}
		var length = _listType == 3 ? _friendItems.Length - 1 : _friendItems.Length;
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
		for (int i = _listType == 3 ? 1 : 0; i < _friendItems.Length; i++)
		{
			if (i - (_listType == 3 ? 1 : 0) >= actorList.Count)
			{
				_friendItems[i].gameObject.SetActive(false);
			}
			else
			{
				_friendItems[i].SetText(actorList[i - (_listType == 3 ? 1 : 0)], _listType == 1 || _listType == 2, _listType == 2);
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
