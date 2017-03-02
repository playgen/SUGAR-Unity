using PlayGen.SUGAR.Unity;
using System.Collections.Generic;
using System.Linq;

using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

public class UserGroupInterface : BaseUserGroupInterface
{
	/// <summary>
	/// An array of the GroupListItemInterface on this GameObject, set in the Inspector.
	/// </summary>
	[Tooltip("An array of the GroupListItemInterface on this GameObject, set in the Inspector.")]
	[SerializeField]
	private GroupListItemInterface[] _groupItems;

	/// <summary>
	/// Text for displaying the current category being displayed.
	/// </summary>
	[Tooltip("Text for displaying the current category being displayed")]
	[SerializeField]
	protected Text _listTypeText;

	/// <summary>
	/// The current category being displayed.
	/// </summary>
	private int _listType;

	/// <summary>
	/// Button used to change display to show user groups.
	/// </summary>
	[Tooltip("Button used to change display to show user groups.")]
	[SerializeField]
	protected Button _groupsButton;

	/// <summary>
	/// Button used to change display to show user pending sent requests.
	/// </summary>
	[Tooltip("Button used to change display to show user pending sent requests")]
	[SerializeField]
	protected Button _sentButton;

	/// <summary>
	/// Button used to change display to show the results of a search by group name.
	/// </summary>
	[Tooltip("Button used to change display to show the results of a search by group name")]
	[SerializeField]
	protected Button _searchButton;

	/// <summary>
	/// GameObject containing input field and button used for searching.
	/// </summary>
	[Tooltip("GameObject containing input field and button used for searching")]
	[SerializeField]
	protected GameObject _searchArea;

	/// <summary>
	/// Input field used to search by group name.
	/// </summary>
	[Tooltip("Input field used to search by group name")]
	[SerializeField]
	protected InputField _searchInput;

	/// <summary>
	/// Button used to trigger searching.
	/// </summary>
	[Tooltip("Button used to trigger searching")]
	[SerializeField]
	protected Button _searchTextButton;

	/// <summary>
	/// Button used to go to the previous page of results.
	/// </summary>
	[Tooltip("Button used to go to the previous page of results.")]
	[SerializeField]
	private Button _previousButton;

	/// <summary>
	/// Button used to go to the next page of results.
	/// </summary>
	[Tooltip("Button used to go to the next page of results.")]
	[SerializeField]
	private Button _nextButton;

	/// <summary>
	/// Text which displays the current page.
	/// </summary>
	[Tooltip("Text which displays the current page.")]
	[SerializeField]
	private Text _pageNumberText;

	/// <summary>
	/// The current page number.
	/// </summary>
	private int _pageNumber;

	/// <summary>
	/// In addition to base onclick adding, adds listeners for the previous and next buttons and all of the category changing buttons.
	/// </summary>
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

	/// <summary>
	/// Sets search text to be empty, triggers DoBestFit method and add event listeners for when resolution and language changes.
	/// </summary>
	private void OnEnable()
	{
		_searchInput.text = string.Empty;
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
		Localization.LanguageChange += OnLanguageChange;
	}

	/// <summary>
	/// Remove event listeners
	/// </summary>
	private void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
		Localization.LanguageChange -= OnLanguageChange;
	}

	/// <summary>
	/// Set the pageNumber and current list type to 0 before displaying the UI.
	/// </summary>
	protected override void PreDisplay()
	{
		_pageNumber = 0;
		_listType = 0;
	}

	/// <summary>
	/// Adjust GroupListItemInterface pool to display a page of groups.
	/// </summary>
	protected override void Draw()
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

	/// <summary>
	/// If a user signs in via this panel, refresh the current page (which should be page 1).
	/// </summary>
	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	/// <summary>
	/// Set the category of groups to display and redraw the UI.
	/// </summary>
	private void SetListType(int type)
	{
		_listType = type;
		Show(true);
	}

	/// <summary>
	/// Adjust the current page number and redraw the UI.
	/// </summary>
	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		Show(true);
	}

	/// <summary>
	/// Set the text of all buttons not a child of a FriendsListItemInterface to be as big as possible and the same size.
	/// </summary>
	private void DoBestFit()
	{
		GetComponentsInChildren<Button>(true).Where(t => !t.GetComponentInParent<FriendsListItemInterface>()).Select(t => t.gameObject).BestFit();
	}

	/// <summary>
	/// Refresh the current page to ensure any text set in code is also translated.
	/// </summary>
	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}