using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base class for UnityClient classes
	/// </summary>
	public class BaseUnityClient<T> : MonoBehaviour where T : BaseInterface
	{
		/// <summary>
		/// UI object for this unity client. Can be left null if no UI is required.
		/// </summary>
		[Tooltip("UI object for this unity client. Can be left null if no UI is required.")]
		[SerializeField]
		protected T _interface;

		/// <summary>
		/// Has a UI object been provided for this Unity Client?
		/// </summary>
		public bool HasInterface => _interface;

		/// <summary>
		/// Is there a UI object and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		internal virtual void CreateInterface(Canvas canvas)
		{
			if (HasInterface)
			{
				var inScene = _interface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_interface.gameObject, canvas.transform, false);
					newInterface.name = _interface.name;
					_interface = newInterface.GetComponent<T>();
				}
				_interface.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Hide the UI object if it is currently active.
		/// </summary>
		public void Hide()
		{
			if (IsActive)
			{
				SUGARManager.unity.DisableObject(_interface.gameObject);
			}
		}
	}
}
