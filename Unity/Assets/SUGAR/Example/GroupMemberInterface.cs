using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

public class GroupMemberInterface : BaseGroupMemberInterface
{
	[SerializeField]
	private GroupMemberItemInterface[] _memberItems;
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

	protected override void DrawMemberList(bool loadingSuccess)
	{
		var actorList = SUGARManager.GroupMember.Members;
		_nextButton.interactable = actorList.Count > (_pageNumber + 1) * _memberItems.Length;
		actorList = actorList.Skip(_pageNumber * _memberItems.Length).Take(_memberItems.Length).ToList();
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
		for (int i = 0; i < _memberItems.Length; i++)
		{
			if (i >= actorList.Count)
			{
				_memberItems[i].gameObject.SetActive(false);
			}
			else
			{
				_memberItems[i].SetText(actorList[i]);
			}
		}
		_pageNumberText.text = Localization.GetAndFormat("PAGE", false, _pageNumber + 1);
		_previousButton.interactable = _pageNumber > 0;
		if (!actorList.Any())
		{
			if (_errorText)
			{
				_errorText.text = Localization.Get("NO_RESULTS_ERROR");
			}
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
		ShowGroupMemberList(true);
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