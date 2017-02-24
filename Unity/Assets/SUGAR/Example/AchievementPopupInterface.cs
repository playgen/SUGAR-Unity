using System.Collections;

using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Unity;

using UnityEngine;

public class AchievementPopupInterface : BaseAchievementPopupInterface
{
	[SerializeField]
	private Animation _animation;

	protected override void Display(EvaluationNotification notification)
	{
		if (!_animation.isPlaying)
		{
			StartCoroutine(AnimatePopup());
		}
	}

	private IEnumerator AnimatePopup()
	{
		while (_achievementQueue.Count > 0)
		{
			_name.text = _achievementQueue[0].Name;
			_animation.Play();
			while (_animation.isPlaying)
			{
				yield return null;
			}
			_achievementQueue.RemoveAt(0);
			yield return null;
		}
	}
}
