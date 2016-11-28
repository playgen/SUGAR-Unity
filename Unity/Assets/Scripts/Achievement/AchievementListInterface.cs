using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public event EventHandler GetAchievements;
        
        
        void OnEnable()
        {
            Debug.Log("OnEnabled");
            InvokeUpdateAchievmentsList();
        }

        private void InvokeUpdateAchievmentsList()
        {
            if (GetAchievements != null) GetAchievements(this, null);
        }

        public void SetAchievementData(IEnumerable<EvaluationProgressResponse> achievementsEnum)
        {
            Debug.Log(achievementsEnum.Count());
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
                if (achievement.Progress != 1.0f)
                {
                    Destroy(achievementItem.transform.FindChild("Tick").gameObject);
                }
                counter++;
            }
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
