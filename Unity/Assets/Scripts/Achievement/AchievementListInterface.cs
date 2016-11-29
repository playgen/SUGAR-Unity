using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class AchievementListInterface : MonoBehaviour
	{
		[SerializeField] private GameObject _achievementList;

		[SerializeField] private GameObject _achievementPrefab;

		[SerializeField] private int _listDisplaySize;

		[SerializeField] private Button _closeButton;
		public event EventHandler GetAchievements;


		void Awake()
		{
			_closeButton.onClick.AddListener(ClosePanel);
		}

		void OnEnable()
		{
			InvokeUpdateAchievmentsList();
		}

		void OnDisable()
		{
			ClearList();
		}

		private void InvokeUpdateAchievmentsList()
		{
			if (GetAchievements != null) GetAchievements(this, null);
		}

		public void SetAchievementData(IEnumerable<EvaluationProgressResponse> achievementsEnum)
		{
			int counter = 0;
			var achievements = achievementsEnum.ToList();
			var listRect = _achievementList.GetComponent<RectTransform>().rect;
			foreach (var achievement in achievements)
			{
				var achievementItem = Instantiate(_achievementPrefab);
				achievementItem.transform.SetParent(_achievementList.transform, false);
				var itemRectTransform = achievementItem.GetComponent<RectTransform>();
				itemRectTransform.sizeDelta = new Vector2(listRect.width, listRect.height / _listDisplaySize);
				itemRectTransform.anchoredPosition = new Vector2(0, (counter * -(listRect.height / _listDisplaySize)));
				achievementItem.GetComponentInChildren<Text>().text = achievement.Name;
				Debug.Log(achievement.Progress);
				if (Mathf.Approximately(achievement.Progress,1.0f))
				{
					Destroy(achievementItem.transform.FindChild("Tick").gameObject);
				}
				counter++;
			}
		}

		private void ClosePanel()
		{
			gameObject.SetActive(false);
		}

		private void ClearList()
		{
			//Remove old achievemnts list
			foreach (Transform child in _achievementList.transform)
			{
				Destroy(child.gameObject);
			}
		}
	}
}
