using System.Collections;
using PlayGen.SUGAR.Client.EvaluationEvents;
using UnityEngine;
using UnityEngine.UI;


namespace SUGAR.Unity
{
	public class AchievementPopupInterface : MonoBehaviour
	{
		[SerializeField]
		private Text _name;

		[SerializeField]
		[Range(0f, 10f)]
		private float _animationDuration;

		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = gameObject.GetComponent<RectTransform>();
		}

		private void Start()
		{
			SetInitialPosition();
		}

		private void SetInitialPosition()
		{
			_rectTransform.localScale = Vector3.one;
			_rectTransform.anchorMin = new Vector2(1, 0);
			_rectTransform.anchorMax = new Vector2(1, 0);
			_rectTransform.pivot = Vector2.one;
			_rectTransform.anchoredPosition = Vector2.zero;
		}

		internal void SetNotification(EvaluationNotification notification)
		{
			_name.text = notification.Name;
		}

		internal void Animate()
		{
			StartCoroutine(AnimatePopup());
		}

		private IEnumerator AnimatePopup()
		{
			var deltaTime = 0f;
			var startPos = _rectTransform.anchoredPosition;
			var endpos = startPos + new Vector2(0f, _rectTransform.rect.height);

			while (deltaTime <= _animationDuration)
			{
				_rectTransform.anchoredPosition = Vector2.Lerp(startPos, endpos, deltaTime/_animationDuration);
				deltaTime += Time.deltaTime;
				yield return null;
			}
			_rectTransform.anchoredPosition = endpos;
		}
	}
}
