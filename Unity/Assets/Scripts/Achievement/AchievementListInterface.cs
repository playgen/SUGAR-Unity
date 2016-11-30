using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class AchievementListInterface : MonoBehaviour
	{
		[SerializeField]
		private AchievementItemInterface[] _achievementItems;
		[SerializeField]
		private Button _previousButton;
		[SerializeField]
		private Button _nextButton;
		[SerializeField]
		private Text _pageNumber;
		[SerializeField]
		private Button _closeButton;

		private void Awake()
		{
			_previousButton.onClick.AddListener(delegate { SUGARManager.Achievement.UpdatePageNumber(-1); });
			_nextButton.onClick.AddListener(delegate { SUGARManager.Achievement.UpdatePageNumber(1); });
			_closeButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
		}

		internal void SetAchievementData(IEnumerable<EvaluationProgressResponse> achievements, int pageNumber)
		{
			if (SUGARManager.CurrentUser == null)
			{
				return;
			}
			gameObject.SetActive(true);
			var achievementList = achievements.Skip(pageNumber * _achievementItems.Length).Take(_achievementItems.Length).ToList();
			if (!achievementList.Any() && pageNumber > 0)
			{
				SUGARManager.Achievement.UpdatePageNumber(-1);
				return;
			}
			for (int i = 0; i < _achievementItems.Length; i++)
			{
				if (i >= achievementList.Count)
				{
					_achievementItems[i].Disbale();
				}
				else
				{
					_achievementItems[i].SetText(achievementList[i].Name, Mathf.Approximately(achievementList[i].Progress, 1.0f));
				}
			}
			_pageNumber.text = "Page " + (pageNumber + 1);
			_previousButton.interactable = pageNumber > 0;
			_nextButton.interactable = achievementList.Count > pageNumber * _achievementItems.Length;
		}
	}
}
