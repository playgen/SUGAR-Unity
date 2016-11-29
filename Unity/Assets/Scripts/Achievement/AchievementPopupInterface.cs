using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;


namespace SUGAR.Unity
{
    public class AchievementPopupInterface : MonoBehaviour
    {
        [SerializeField]
        private Text _name;

        [SerializeField]
        private Vector2 _startPosition;

        void Awake()
        {
            gameObject.GetComponent<RectTransform>().position = _startPosition;
        }

        public void SetNotification(EvaluationNotification notification)
        {
            _name.text = notification.Name;
        }

        public void Animate()
        {
            
        }
    }
}
