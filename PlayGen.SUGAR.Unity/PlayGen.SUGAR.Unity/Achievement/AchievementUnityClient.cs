using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class AchievementUnityClient : MonoBehaviour
	{
		[SerializeField]
		private BaseAchievementListInterface _achievementInterface;

		[SerializeField]
		private BaseAchievementPopupInterface _achievementPopup;

		[SerializeField]
		[Range(0f, 10f)]
		private float _notificationCheckRate = 2.5f;

		internal List<EvaluationProgressResponse> progress { get; private set; } = new List<EvaluationProgressResponse>();

		public bool IsActive => _achievementInterface && _achievementInterface.gameObject.activeInHierarchy;

		internal void CreateInterface(Canvas canvas)
		{
			if (_achievementInterface)
			{
				bool inScene = _achievementInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_achievementInterface.gameObject, canvas.transform, false);
					newInterface.name = _achievementInterface.name;
					_achievementInterface = newInterface.GetComponent<BaseAchievementListInterface>();
				}
				_achievementInterface.gameObject.SetActive(false);
				SUGARManager.client.Achievement.EnableNotifications(true);
			}
			if (_achievementPopup)
			{
				bool inScenePopUp = _achievementPopup.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScenePopUp)
				{
					var newPopUp = Instantiate(_achievementPopup.gameObject, canvas.transform, false);
					newPopUp.name = _achievementPopup.name;
					_achievementPopup = newPopUp.GetComponent<BaseAchievementPopupInterface>();
				}
				_achievementPopup.gameObject.SetActive(true);
				InvokeRepeating("NotificatonCheck", _notificationCheckRate, _notificationCheckRate);
			}
		}

		public void DisplayList()
		{
			if (_achievementInterface)
			{
				GetAchievements(success =>
				{
					_achievementInterface.Display(success);
				});
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_achievementInterface.gameObject);
			}
		}

		public void ForceNotificationTest()
		{
			HandleNotification(new EvaluationNotification
			{
				Name = "Test Notification"
			});
		}

		private void GetAchievements(Action<bool> success)
		{
			progress.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Achievement.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					progress = response.ToList();
					success(true);
				},
				exception =>
				{
					string error = "Failed to get achievements list. " + exception.Message;
					Debug.LogError(error);
					success(false);
				});
			}
			else
			{
				success(false);
			}
		}

		private void NotificatonCheck()
		{
			EvaluationNotification notification;
			if (SUGARManager.client.Achievement.TryGetPendingNotification(out notification))
			{
				HandleNotification(notification);
			}
		}

		private void HandleNotification(EvaluationNotification notification)
		{
			if (_achievementPopup)
			{
				_achievementPopup.Notification(notification);
			}
		}
	}
}
