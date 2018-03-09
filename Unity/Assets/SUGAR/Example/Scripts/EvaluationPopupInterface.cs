﻿using System.Collections;

using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Unity;

using UnityEngine;

public class EvaluationPopupInterface : BaseEvaluationPopupInterface
{
	/// <summary>
	/// Animation for displaying the evaluation notification.
	/// </summary>
	[Tooltip("Animation for displaying the evaluation notification")]
	[SerializeField]
	private Animation _animation;

	/// <summary>
	/// If the animation is not playing, start the animation coroutine.
	/// </summary>
	protected override void Display(EvaluationNotification notification)
	{
		if (!_animation.isPlaying)
		{
			StartCoroutine(AnimatePopup());
		}
	}

	/// <summary>
	/// While there are notifications to display, cycle the animation.
	/// </summary>
	private IEnumerator AnimatePopup()
	{
		while (_evaluationQueue.Count > 0)
		{
			_name.text = _evaluationQueue[0].Name;
			_animation.Play();
			while (_animation.isPlaying)
			{
				yield return null;
			}
			_evaluationQueue.RemoveAt(0);
			yield return null;
		}
	}
}
