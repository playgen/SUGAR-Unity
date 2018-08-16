using System.Collections.Generic;

using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for controlling the interface related to display evaluation notifications when an evalaution is completed.
	/// </summary>
	public abstract class BaseEvaluationPopupInterface : MonoBehaviour
	{
		/// <value>
		/// Text used for display notification string (usually evaluation name). Can be left null.
		/// </value>
		[Tooltip("Text used for display notification string (usually evaluation name). Can be left null.")]
		[SerializeField]
		protected Text _name;

		/// <value>
		/// Image displayed alongside notification. Can be left null.
		/// </value>
		[Tooltip("Image displayed alongside notification. Can be left null.")]
		[SerializeField]
		protected Image _image;

		/// <value>
		/// Queue of notifications to be displayed.
		/// </value>
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
		/// <param name="notification">Notification which will be displayed.</param>
		protected abstract void Display(EvaluationNotification notification);
	}
}
