using UnityEngine;
using UnityEngine.UI;

namespace SUGAR.Unity
{
	public class AchievementItemInterface : MonoBehaviour
	{
		public Text AchieveName;

		public Image Tick;

		public void SetText(string achieveName, bool completed)
		{
			gameObject.SetActive(true);
			AchieveName.text = achieveName;
			Tick.enabled = completed;
		}

		public void Disbale()
		{
			gameObject.SetActive(false);
		}
	}
}
