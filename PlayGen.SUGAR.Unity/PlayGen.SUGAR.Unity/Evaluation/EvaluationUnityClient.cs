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
	/// Unity client for functionality related to gathering evaluation progress and notifications when an evaluation is completed.
	/// </summary>
	[DisallowMultipleComponent]
	public class EvaluationUnityClient : BaseUnityClient<BaseEvaluationListInterface>
	{
		[Tooltip("Landscape interface used for evaluation completion notifications. Can be left null if not needed.")]
		[SerializeField]
		private BaseEvaluationPopupInterface _landscapeAchievementPopup;

		[Tooltip("Portrait UI object used for evaluation completion notifications. Can be left null if not needed.")]
		[SerializeField]
		private BaseEvaluationPopupInterface _portraitAchievementPopup;

		private BaseEvaluationPopupInterface _achievementPopup => Screen.width > Screen.height ? _landscapeAchievementPopup ?? _portraitAchievementPopup : _portraitAchievementPopup ?? _landscapeAchievementPopup;

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
			_landscapeAchievementPopup = SetInterface(_landscapeAchievementPopup, canvas);
			_landscapeAchievementPopup?.gameObject.SetActive(true);

			_portraitAchievementPopup = SetInterface(_portraitAchievementPopup, canvas);
			_portraitAchievementPopup?.gameObject.SetActive(true);

			if (_landscapeAchievementPopup || _portraitAchievementPopup)
			{
				SUGARManager.client.Achievement.EnableNotifications(true);
				SUGARManager.client.Skill.EnableNotifications(true);
				InvokeRepeating("NotificatonCheck", _notificationCheckRate, _notificationCheckRate);
			}
		}

		internal BaseEvaluationPopupInterface SetInterface(BaseEvaluationPopupInterface popupInterface, Canvas canvas, string extension = "")
		{
			if (!popupInterface)
			{
				return null;
			}
			var inScenePopUp = popupInterface.gameObject.scene == SceneManager.GetActiveScene() || popupInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScenePopUp)
			{
				var newPopUp = Instantiate(popupInterface, canvas.transform, false);
				newPopUp.name = popupInterface.name + extension;
				popupInterface = newPopUp;
			}
			return popupInterface;
		}

		/// <summary>
		/// Change the used interfaces if the aspect ratio changes.
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (_landscapeAchievementPopup && _landscapeAchievementPopup != _achievementPopup && _landscapeAchievementPopup.gameObject.activeInHierarchy)
			{
				_landscapeAchievementPopup.gameObject.SetActive(false);
				_achievementPopup.gameObject.SetActive(true);
			}
			if (_portraitAchievementPopup && _portraitAchievementPopup != _achievementPopup && _portraitAchievementPopup.gameObject.activeInHierarchy)
			{
				_portraitAchievementPopup.gameObject.SetActive(false);
				_achievementPopup.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Gathers current user achievement completion status and displays interface if provided.
		/// </summary>
		public void DisplayAchievementList()
		{
			SUGARManager.unity.StartSpinner();
			GetAchievements(SUGARManager.CurrentUser, success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		/// <summary>
		/// Gathers current group achievement completion status and displays interface if provided.
		/// </summary>
		public void DisplayGroupAchievementList()
		{
			SUGARManager.unity.StartSpinner();
			GetAchievements(SUGARManager.CurrentGroup, success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		private void GetAchievements(ActorResponse actor, Action<bool> success)
		{
			Progress.Clear();
			if (actor != null)
			{
				SUGARManager.client.Achievement.GetGameProgressAsync(SUGARManager.GameId, actor.Id,
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
		/// Gathers current user skill completion status and displays interface if provided.
		/// </summary>
		public void DisplaySkillList()
		{
			SUGARManager.unity.StartSpinner();
			GetSkills(SUGARManager.CurrentUser, success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		/// <summary>
		/// Gathers current group skill completion status and displays interface if provided.
		/// </summary>
		public void DisplayGroupSkillList()
		{
			SUGARManager.unity.StartSpinner();
			GetSkills(SUGARManager.CurrentGroup, success =>
			{
				SUGARManager.unity.StopSpinner();
				_interface?.Display(success);
			});
		}

		private void GetSkills(ActorResponse actor, Action<bool> success)
		{
			Progress.Clear();
			if (actor != null)
			{
				SUGARManager.client.Skill.GetGameProgressAsync(SUGARManager.GameId, actor.Id,
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
		/// <param name="notification">String which will be used in the notification.</param>
		public void ForceNotification(string notification = "Test Notification")
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
			_landscapeAchievementPopup?.Notification(notification);
			_portraitAchievementPopup?.Notification(notification);
		}

		internal void ResetClient()
		{
			Progress = null;
		}
	}
}
