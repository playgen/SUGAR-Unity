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
		[Tooltip("Landscape UI object used for evaluation completion notifications. Can be left null if not needed.")]
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
			if (_interface)
			{
				SUGARManager.client.Achievement.EnableNotifications(true);
				SUGARManager.client.Skill.EnableNotifications(true);
			}
			if (_landscapeAchievementPopup)
			{
				SetInterface(_landscapeAchievementPopup, canvas).gameObject.SetActive(true);
			}
			if (_portraitAchievementPopup)
			{
				SetInterface(_portraitAchievementPopup, canvas).gameObject.SetActive(true);
			}

			InvokeRepeating("NotificatonCheck", _notificationCheckRate, _notificationCheckRate);
		}

		protected BaseEvaluationPopupInterface SetInterface(BaseEvaluationPopupInterface popupInterface, Canvas canvas)
		{
			var inScenePopUp = popupInterface.gameObject.scene == SceneManager.GetActiveScene() || popupInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScenePopUp)
			{
				var newPopUp = Instantiate(popupInterface.gameObject, canvas.transform, false);
				newPopUp.name = popupInterface.name;
				popupInterface = newPopUp.GetComponent<BaseEvaluationPopupInterface>();
			}
			return popupInterface;
		}

		protected override void Update()
		{
			base.Update();
			if (_landscapeAchievementPopup && _landscapeAchievementPopup == _achievementPopup && !_landscapeAchievementPopup.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_landscapeAchievementPopup.gameObject);
				SUGARManager.unity.EnableObject(_achievementPopup.gameObject);
			}
			if (_portraitAchievementPopup && _portraitAchievementPopup == _achievementPopup && !_portraitAchievementPopup.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_portraitAchievementPopup.gameObject);
				SUGARManager.unity.EnableObject(_achievementPopup.gameObject);
			}
		}

		/// <summary>
		/// Gathers current evaluation completion status and displays UI object if provided.
		/// </summary>
		public void DisplayAchievementList()
		{
			SUGARManager.unity.StartSpinner();
			GetAchievements(SUGARManager.CurrentUser, success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
			});
		}

		/// <summary>
		/// Gathers current evaluation completion status and displays UI object if provided.
		/// </summary>
		public void DisplayGroupAchievementList()
		{
			SUGARManager.unity.StartSpinner();
			GetAchievements(SUGARManager.CurrentGroup, success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
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
		/// Gathers current skill completion status and displays UI object if provided.
		/// </summary>
		public void DisplaySkillList()
		{
			SUGARManager.unity.StartSpinner();
			GetSkills(SUGARManager.CurrentUser, success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
			});
		}

		/// <summary>
		/// Gathers current skill completion status and displays UI object if provided.
		/// </summary>
		public void DisplayGroupSkillList()
		{
			SUGARManager.unity.StartSpinner();
			GetSkills(SUGARManager.CurrentGroup, success =>
			{
				SUGARManager.unity.StopSpinner();
				if (HasInterface)
				{
					_interface.Display(success);
				}
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
			if (_landscapeAchievementPopup)
			{
				_landscapeAchievementPopup.Notification(notification);
			}
			if (_portraitAchievementPopup)
			{
				_portraitAchievementPopup.Notification(notification);
			}
		}
	}
}
