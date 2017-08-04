using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Contracts;
using UnityEngine;
using System.Linq;

using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to achievements.
	/// </summary>
	[DisallowMultipleComponent]
	public class AchievementUnityClient : BaseUnityClient<BaseAchievementListInterface>
	{
		[Tooltip("UI object used for achievement completion notifications. Can be left null if not needed.")]
		[SerializeField]
		private BaseAchievementPopupInterface _achievementPopup;

		[SerializeField]
		[Range(0.1f, 10f)]
		[Tooltip("How often (in seconds) checks are made for if any achievements have been completed.")]
		private float _notificationCheckRate = 2.5f;

		private List<EvaluationProgressResponse> _progress = new List<EvaluationProgressResponse>();

		/// <summary>
		/// Current completion status for achievements in this application for this user.
		/// </summary>
		public List<EvaluationProgressResponse> Progress => _progress;

		internal override void CreateInterface(Canvas canvas)
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

		/// <summary>
		/// Gathers current achievement completion status and displays UI object if provided.
		/// </summary>
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
			_progress.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Achievement.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					_progress = response.Items.ToList();
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

		/// <summary>
		/// Force an achievement notification to be displayed with the provided text.
		/// </summary>
		public void ForceNotificationTest(string notification = "Test Notification")
		{
			HandleNotification(new EvaluationNotification
			{
				Name = notification
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
