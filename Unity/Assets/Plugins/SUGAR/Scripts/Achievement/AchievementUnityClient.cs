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
		private List<EvaluationProgressResponse> _progress = new List<EvaluationProgressResponse>();

		internal List<EvaluationProgressResponse> Progress
		{
			get { return _progress; }
		}

		[SerializeField]
		private AchievementListInterface _achievementListInterface;

		[SerializeField]
		private AchievementPopupInterface _achievementPopup;

		[SerializeField]
		[Range(0f, 10f)]
		private float _notificationCheckRate = 2.5f;

		internal void CreateInterface(Canvas canvas)
		{
			bool inScene = _achievementListInterface.gameObject.scene == SceneManager.GetActiveScene();
			if (!inScene)
			{
				var newInterface = Instantiate(_achievementListInterface, canvas.transform, false);
				newInterface.name = _achievementListInterface.name;
				_achievementListInterface = newInterface;
			}
			_achievementListInterface.gameObject.SetActive(false);
			SUGARManager.Client.Achievement.EnableNotifications(true);
			bool inScenePopUp = _achievementPopup.gameObject.scene == gameObject.scene;
			if (!inScenePopUp)
			{
				var newPopUp = Instantiate(_achievementPopup, canvas.transform, false);
				newPopUp.name = _achievementPopup.name;
				_achievementPopup = newPopUp;
			}
			_achievementPopup.gameObject.SetActive(false);
			InvokeRepeating("NotificatonCheck", _notificationCheckRate, _notificationCheckRate);
		}

		public void DisplayList()
		{
			GetAchievements();
			_achievementListInterface.Display();
		}

		private void GetAchievements()
		{
			if (SUGARManager.CurrentUser != null)
			{
				SUGARManager.Client.Achievement.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					_progress = response.ToList();
				},
				exception =>
				{
					string error = "Failed to get achievements list. " + exception.Message;
					Debug.LogError(error);
				});
			}
		}

		private void NotificatonCheck()
		{
			EvaluationNotification notification;
			if (SUGARManager.Client.Achievement.TryGetPendingNotification(out notification))
			{
				HandleNotification(notification);
			}
		}

		private void HandleNotification(EvaluationNotification notification)
		{
			Debug.Log("NOTIFICATION");
			_achievementPopup.Animate(notification);
		}
	}
}
