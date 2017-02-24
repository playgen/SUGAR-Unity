using System.Collections.Generic;

using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAchievementPopupInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _name;
		[SerializeField]
		protected Image _image;

		protected readonly List<EvaluationNotification> _achievementQueue = new List<EvaluationNotification>();

		internal void Notification(EvaluationNotification notification)
		{
			_achievementQueue.Add(notification);
			Display(notification);
		}

		protected abstract void Display(EvaluationNotification notification);
	}
}
