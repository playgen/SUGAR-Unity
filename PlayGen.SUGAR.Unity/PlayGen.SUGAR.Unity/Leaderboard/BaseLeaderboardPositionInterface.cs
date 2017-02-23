using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardPositionInterface : MonoBehaviour
	{
		[SerializeField]
		protected Text _position;

		[SerializeField]
		protected Text _playerName;

		[SerializeField]
		protected Text _score;

		internal virtual void Enable()
		{
			gameObject.SetActive(false);
		}

		internal virtual void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}
