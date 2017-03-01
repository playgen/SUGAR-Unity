using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	public class BaseUnityClient<T> : MonoBehaviour where T : BaseInterface
	{
		[SerializeField]
		protected T _interface;

		public bool HasInterface => _interface;

		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		internal virtual void CreateInterface(Canvas canvas)
		{
			if (HasInterface)
			{
				bool inScene = _interface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_interface.gameObject, canvas.transform, false);
					newInterface.name = _interface.name;
					_interface = newInterface.GetComponent<T>();
				}
				_interface.gameObject.SetActive(false);
			}
		}

		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_interface.gameObject);
			}
		}
	}
}
