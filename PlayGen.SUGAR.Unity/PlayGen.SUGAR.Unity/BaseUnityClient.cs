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
			_landscapeInterface = SetInterface(_landscapeInterface, canvas);
			_landscapeInterface?.gameObject.SetActive(false);

			_portraitInterface = SetInterface(_portraitInterface, canvas);
			_portraitInterface?.gameObject.SetActive(false);
		}

		protected T SetInterface<T>(T panelInterface, Canvas canvas, string extension = "") where T : BaseInterface
		{
			if (!panelInterface)
				return null;
			var inScene = panelInterface.gameObject.scene == SceneManager.GetActiveScene() || panelInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScene)
			{
				var newInterface = Instantiate(panelInterface.gameObject, canvas.transform, false);
				newInterface.name = panelInterface.name + extension;
				panelInterface = newInterface.GetComponent<T>();
			}
			return panelInterface;
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
