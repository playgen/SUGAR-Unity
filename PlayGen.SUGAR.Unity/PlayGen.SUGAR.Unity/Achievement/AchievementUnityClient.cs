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
	public class AchievementUnityClient : BaseUnityClient<BaseAchievementListInterface>
	{
		[SerializeField]
		private BaseAchievementPopupInterface _achievementPopup;

		[SerializeField]
		[Range(1f, 10f)]
		private float _notificationCheckRate = 2.5f;

		public List<EvaluationProgressResponse> Progress { get; private set; } = new List<EvaluationProgressResponse>();

		internal void CreateInterface(Canvas canvas)
		{
			base.CreateInterface(canvas);
			if (_interface)
			{
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
			SUGARManager.unity.StartSpinner();
			GetAchievements(success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
			});
		}

		private void GetAchievements(Action<bool> success)
		{
			Progress.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Achievement.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					Progress = response.ToList();
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

		public void ForceNotificationTest()
		{
			HandleNotification(new EvaluationNotification
			{
				Name = "Test Notification"
			});
		}

		private void NotificatonCheck()
		{
			EvaluationNotification notification;
			if (SUGARManager.client != null && SUGARManager.client.Achievement.TryGetPendingNotification(out notification))
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
