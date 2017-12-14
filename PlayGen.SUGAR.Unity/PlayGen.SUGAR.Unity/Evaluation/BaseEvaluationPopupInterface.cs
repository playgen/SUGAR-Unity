using System.Collections.Generic;

using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the UI object related to evaluation notifications.
	/// </summary>
	public abstract class BaseEvaluationPopupInterface : MonoBehaviour
	{
		/// <summary>
		/// Text used for display notification string (usually evaluation name). Can be left null.
		/// </summary>
		[Tooltip("Text used for display notification string (usually evaluation name). Can be left null.")]
		[SerializeField]
		protected Text _name;

		/// <summary>
		/// Image displayed alongside notification. Can be left null.
		/// </summary>
		[Tooltip("Image displayed alongside notification. Can be left null.")]
		[SerializeField]
		protected Image _image;

		/// <summary>
		/// Queue of notifications to be displayed.
		/// </summary>
		protected readonly List<EvaluationNotification> _evaluationQueue = new List<EvaluationNotification>();

		internal void Notification(EvaluationNotification notification)
		{
			_evaluationQueue.Add(notification);
			transform.SetAsLastSibling();
			Display(notification);
		}

		/// <summary>
		/// Functionality to be triggered when a notification is received.
		/// </summary>
		protected abstract void Display(EvaluationNotification notification);
	}
}
