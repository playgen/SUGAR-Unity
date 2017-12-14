using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using System.Linq;

using PlayGen.SUGAR.Contracts;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity client for calls related to evaluations.
	/// </summary>
	[DisallowMultipleComponent]
	public class EvaluationUnityClient : BaseUnityClient<BaseEvaluationListInterface>
	{
		[Tooltip("UI object used for evaluation completion notifications. Can be left null if not needed.")]
		[SerializeField]
		private BaseEvaluationPopupInterface _achievementPopup;

		[SerializeField]
		[Range(0.1f, 10f)]
		[Tooltip("How often (in seconds) checks are made for if any evaluations have been completed.")]
		private float _notificationCheckRate = 2.5f;

		/// <summary>
		/// Current completion status for evaluations in this application for this user.
		/// </summary>
		public List<EvaluationProgressResponse> Progress { get; private set; } = new List<EvaluationProgressResponse>();

		internal override void CreateInterface(Canvas canvas)
		{
			base.CreateInterface(canvas);
			if (_interface)
			{
				SUGARManager.client.Achievement.EnableNotifications(true);
				SUGARManager.client.Skill.EnableNotifications(true);
			}
			if (_achievementPopup)
			{
				var inScenePopUp = _achievementPopup.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScenePopUp)
				{
					var newPopUp = Instantiate(_achievementPopup.gameObject, canvas.transform, false);
					newPopUp.name = _achievementPopup.name;
					_achievementPopup = newPopUp.GetComponent<BaseEvaluationPopupInterface>();
				}
				_achievementPopup.gameObject.SetActive(true);
				InvokeRepeating("NotificatonCheck", _notificationCheckRate, _notificationCheckRate);
			}
		}

		/// <summary>
		/// Gathers current evaluation completion status and displays UI object if provided.
		/// </summary>
		public void DisplayAchievementList()
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
					var error = "Failed to get achievements list. " + exception.Message;
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
		/// Gathers current skill completion status and displays UI object if provided.
		/// </summary>
		public void DisplaySkillList()
		{
			SUGARManager.unity.StartSpinner();
			GetSkills(success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
			});
		}

		private void GetSkills(Action<bool> success)
		{
			Progress.Clear();
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.client.Skill.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					Progress = response.ToList();
					success(true);
				},
				exception =>
				{
					var error = "Failed to get skills list. " + exception.Message;
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
		/// Force an evaluation notification to be displayed with the provided text.
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
			if (SUGARManager.client != null && SUGARManager.client.Achievement.TryGetPendingNotification(out var notification))
			{
				HandleNotification(notification);
			}
			if (SUGARManager.client != null && SUGARManager.client.Skill.TryGetPendingNotification(out notification))
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
