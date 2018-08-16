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
	/// Use this for gathering evaluation progress and notifications when an evaluation is completed.
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

		/// <value>
		/// Current completion status for evaluations in this application for the currently signed in user.
		/// </value>
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
		/// Update the interface to be used when the aspect ration changes
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
		/// Gathers current user achievement and skill completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplayEvaluationList()
		{
			Progress.Clear();
			GetEvaluationProgress(SUGARManager.CurrentUser, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers current group achievement and skill completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplayGroupEvaluationList()
		{
			Progress.Clear();
			GetEvaluationProgress(SUGARManager.CurrentGroup, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers achievement and skill completion status for the actor provided.
		/// </summary>
		/// <param name="actor">The user or group the achievement and skill progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the achievement and skill progress for the actor provided.</param>
		public void GetEvaluationProgress(ActorResponse actor, Action<List<EvaluationProgressResponse>> progress)
		{
			if (actor != null)
			{
				GetEvaluationProgress(actor.Id, progress);
			}
			else
			{
				progress(null);
			}
		}

		/// <summary>
		/// Gathers achievement and skill completion status for the actor id provided.
		/// </summary>
		/// <param name="id">The id of the user or group the achievement and skill progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the achievement and skill progress for the actor provided.</param>
		public void GetEvaluationProgress(int id, Action<List<EvaluationProgressResponse>> progress)
		{
			GetAchievements(id,
			achievementProgress =>
			{
				GetSkills(id,
				skillProgress =>
				{
					if (achievementProgress == null && skillProgress == null)
					{
						progress(null);
					}
					else
					{
						var progressList = new List<EvaluationProgressResponse>();
						if (achievementProgress != null)
						{
							progressList.AddRange(achievementProgress);
						}
						if (skillProgress != null)
						{
							progressList.AddRange(skillProgress);
						}
						progress(progressList);
					}
				});
			});
		}

		/// <summary>
		/// Gathers current user achievement completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplayAchievementList()
		{
			Progress.Clear();
			GetAchievementProgress(SUGARManager.CurrentUser, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers current group achievement completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplayGroupAchievementList()
		{
			Progress.Clear();
			GetAchievementProgress(SUGARManager.CurrentGroup, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers achievement completion status for the actor provided.
		/// </summary>
		/// <param name="actor">The user or group the achievement progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the achievement progress for the actor provided.</param>
		public void GetAchievementProgress(ActorResponse actor, Action<List<EvaluationProgressResponse>> progress)
		{
			if (actor != null)
			{
				GetAchievementProgress(actor.Id, progress);
			}
			else
			{
				progress(null);
			}
		}

		/// <summary>
		/// Gathers achievement completion status for the actor id provided.
		/// </summary>
		/// <param name="id">The id of the user or group the achievement progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the achievement progress for the actor provided.</param>
		public void GetAchievementProgress(int id, Action<List<EvaluationProgressResponse>> progress)
		{
			GetAchievements(id, progress);
		}

		private void GetAchievements(int id, Action<List<EvaluationProgressResponse>> progress)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Achievement.GetGameProgressAsync(SUGARManager.GameId, id,
				response =>
				{
					SUGARManager.unity.StopSpinner();
					progress(response.ToList());
				},
				exception =>
				{
					SUGARManager.unity.StopSpinner();
					Debug.LogError($"Failed to get achievements list. {exception}");
					progress(null);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				progress(null);
			}
		}

		/// <summary>
		/// Gathers current user skill completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplaySkillList()
		{
			Progress.Clear();
			GetSkillProgress(SUGARManager.CurrentUser, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers current group skill completion status and displays the interface if it is provided.
		/// </summary>
		public void DisplayGroupSkillList()
		{
			Progress.Clear();
			GetSkillProgress(SUGARManager.CurrentGroup, progress =>
			{
				if (progress != null)
				{
					Progress = progress;
				}
				_interface?.Display(progress != null);
			});
		}

		/// <summary>
		/// Gathers skill completion status for the actor provided.
		/// </summary>
		/// <param name="actor">The user or group the skill progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the skill progress for the actor provided.</param>
		public void GetSkillProgress(ActorResponse actor, Action<List<EvaluationProgressResponse>> progress)
		{
			if (actor != null)
			{
				GetSkillProgress(actor.Id, progress);
			}
			else
			{
				progress(null);
			}
		}

		/// <summary>
		/// Gathers skill completion status for the actor id provided.
		/// </summary>
		/// <param name="id">The id of the user or group the skill progress will be gathered for.</param>
		/// <param name="progress">Callback which will return the skill progress for the actor provided.</param>
		public void GetSkillProgress(int id, Action<List<EvaluationProgressResponse>> progress)
		{
			GetSkills(id, progress);
		}

		private void GetSkills(int id, Action<List<EvaluationProgressResponse>> progress)
		{
			SUGARManager.unity.StartSpinner();
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Skill.GetGameProgressAsync(SUGARManager.GameId, id,
				response =>
				{
					SUGARManager.unity.StopSpinner();
					progress(response.ToList());
				},
				exception =>
				{
					SUGARManager.unity.StopSpinner();
					Debug.LogError($"Failed to get skills list. {exception}");
					progress(null);
				});
			}
			else
			{
				SUGARManager.unity.StopSpinner();
				progress(null);
			}
		}

		/// <summary>
		/// Force a notification to be displayed with the provided notification text.
		/// </summary>
		/// <remarks>
		/// - This uses the EvaluationPopupInterface to display the text in the application
		/// </remarks>
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
			Progress.Clear();
		}
	}
}
