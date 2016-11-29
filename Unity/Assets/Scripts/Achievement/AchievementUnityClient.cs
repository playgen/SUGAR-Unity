﻿using System;
using System.Collections.Generic;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace SUGAR.Unity
{
    class AchievementUnityClient : MonoBehaviour
    {
        private AchievementClient _achievementClient;

		private int _pageNumber;

		private List<EvaluationProgressResponse> _progress = new List<EvaluationProgressResponse>();

		[SerializeField]
        private AchievementListInterface _achievementListInterface;

        [SerializeField]
        private Text _errorText;

        [SerializeField]
        private Canvas _popupTarget;

        [SerializeField]
        private AchievementPopupInterface _achievementPopup;

        void Awake()
        {
            _achievementClient = SUGARManager.Client.Achievement;
            _achievementClient.EnableNotifications(true);
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
            var popup = Instantiate(_achievementPopup);
            popup.transform.SetParent(_popupTarget.transform);
            popup.SetNotification(notification);
            popup.Animate();
        }

        public void GetAchievements()
        {
			if (SUGARManager.CurrentUser != null)
			{
				try
				{
					_progress = _achievementClient.GetGameProgress(SUGARManager.GameId, SUGARManager.CurrentUser.Id).ToList();
					_achievementListInterface.SetAchievementData(_progress, _pageNumber);
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
        }

		public void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			_achievementListInterface.SetAchievementData(_progress, _pageNumber);
		}
    }
}
