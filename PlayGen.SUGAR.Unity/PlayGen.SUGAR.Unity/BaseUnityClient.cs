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
		/// Landscape UI object for this unity client. Can be left null if not required.
		/// </summary>
		[Tooltip("Landscape UI object for this unity client. Can be left null if not required.")]
		[SerializeField]
		protected T _landscapeInterface;

		/// <summary>
		/// Portrait UI object for this unity client. Can be left null if not required.
		/// </summary>
		[Tooltip("Portrait UI object for this unity client. Can be left null if not required.")]
		[SerializeField]
		protected T _portraitInterface;

		protected T _interface => Screen.width > Screen.height ? _landscapeInterface ?? _portraitInterface : _portraitInterface ?? _landscapeInterface;

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
			if (_landscapeInterface)
			{
				var inScene = _landscapeInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_landscapeInterface.gameObject, canvas.transform, false);
					newInterface.name = _landscapeInterface.name;
					_landscapeInterface = newInterface.GetComponent<T>();
				}
				_landscapeInterface.gameObject.SetActive(false);
			}
			if (_portraitInterface)
			{
				var inScene = _portraitInterface.gameObject.scene == SceneManager.GetActiveScene();
				if (!inScene)
				{
					var newInterface = Instantiate(_portraitInterface.gameObject, canvas.transform, false);
					newInterface.name = _portraitInterface.name;
					_portraitInterface = newInterface.GetComponent<T>();
				}
				_portraitInterface.gameObject.SetActive(false);
			}
		}

		protected virtual void Update()
		{
			if (_landscapeInterface && _landscapeInterface != _interface && _landscapeInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_landscapeInterface.gameObject);
				SUGARManager.unity.EnableObject(_interface.gameObject);
				_interface.Display();
			}
			if (_portraitInterface && _portraitInterface != _interface && _portraitInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_portraitInterface.gameObject);
				SUGARManager.unity.EnableObject(_interface.gameObject);
				_interface.Display();
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
