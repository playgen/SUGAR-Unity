using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
    class AchievementUnityClient : MonoBehaviour
    {
        private AchievementClient _achievementClient;

        [SerializeField]
        private AchievementListInterface _achievementListInterface;

        [SerializeField]
        private Text _errorText;

        void Awake()
        {
            _achievementClient = SUGARManager.Client.Achievement;
            _achievementClient.EnableNotifications(true);
            _achievementListInterface.GetAchievements += OnGetAchievments;

        
        }

        void Update()
        {
            EvaluationNotification notification;
            if (_achievementClient.TryGetPendingNotification(out notification))
            {
                HandleNotification(notification);
            }
        }

        private void HandleNotification(EvaluationNotification notification)
        {
            Debug.Log("NOTIFICATION");
            Debug.Log(notification.Name);
        }


        public void GetAchievements()
        {
            try
            {
                var achievementData = _achievementClient.GetGameProgress(SUGARManager.GameId, SUGARManager.CurrentUser.Id);
                _achievementListInterface.SetAchievementData(achievementData);


            }
            catch (Exception ex)
            {
                string error = "Failed to get achievements list. " + ex.Message;
                Debug.LogError(error);
                if (_errorText != null)
                {
                    _errorText.text = error;

                }
            }
        }

        private void OnGetAchievments(object sender, EventArgs EventArgs)
        {
            GetAchievements();
        }


    }
}
