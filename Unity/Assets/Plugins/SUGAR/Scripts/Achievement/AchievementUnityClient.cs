using System;
using System.Collections.Generic;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Contracts.Shared;

using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace SUGAR.Unity
{
	[DisallowMultipleComponent]
	public class AchievementUnityClient : MonoBehaviour
	{
		private AchievementClient _achievementClient;

		private int _pageNumber;

		private List<EvaluationProgressResponse> _progress = new List<EvaluationProgressResponse>();

		[SerializeField]
		private AchievementListInterface _achievementListInterface;

		[SerializeField]
		private Canvas _popupTarget;

		[SerializeField]
		private AchievementPopupInterface _achievementPopup;

		private void Awake()
		{
			_achievementClient = SUGARManager.Client.Achievement;
			_achievementClient.EnableNotifications(true);
		}

		private void Update()
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

		public void DisplayList()
		{
			GetAchievements();
		}

		private void GetAchievements()
		{
			if (SUGARManager.CurrentUser != null)
			{
				_achievementClient.GetGameProgressAsync(SUGARManager.GameId, SUGARManager.CurrentUser.Id,
				response =>
				{
					_progress = response.ToList();
					_achievementListInterface.SetAchievementData(_progress, _pageNumber);
				},
				exception =>
				{
					string error = "Failed to get achievements list. " + exception.Message;
					Debug.LogError(error);
				});
			}
		}

		internal void UpdatePageNumber(int changeAmount)
		{
			_pageNumber += changeAmount;
			_achievementListInterface.SetAchievementData(_progress, _pageNumber);
		}
	}
}
