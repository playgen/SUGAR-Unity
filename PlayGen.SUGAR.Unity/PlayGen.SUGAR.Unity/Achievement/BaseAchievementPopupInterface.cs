using System.Collections.Generic;

using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAchievementPopupInterface : MonoBehaviour
	{
		[SerializeField]
		internal Text _name;
		[SerializeField]
		internal Image _image;

		internal readonly List<EvaluationNotification> _achievementQueue = new List<EvaluationNotification>();

		internal void Notification(EvaluationNotification notification)
		{
			_achievementQueue.Add(notification);
			Display(notification);
		}

		internal abstract void Display(EvaluationNotification notification);
	}
}
