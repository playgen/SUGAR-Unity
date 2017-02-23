using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseAchievementItemInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _achieveName;
		[SerializeField]
		protected Image _achieveImage;

		internal virtual void Enable()
		{
			gameObject.SetActive(true);
		}

		internal virtual void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}
