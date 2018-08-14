using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Base abstract class for UnityClient classes
	/// </summary>
	public abstract class BaseUnityClient<T> : MonoBehaviour where T : BaseInterface
	{
		/// <summary>
		/// Landscape interface for this area of functionality. Can be left null if not required.
		/// </summary>
		[Tooltip("Landscape interface for this area of functionality. Can be left null if not required.")]
		[SerializeField]
		protected T _landscapeInterface;

		/// <summary>
		/// Portrait interface for this area of functionality. Can be left null if not required.
		/// </summary>
		[Tooltip("Portrait interface for this area of functionality. Can be left null if not required.")]
		[SerializeField]
		protected T _portraitInterface;

		/// <summary>
		/// The interface that is used for the current aspect ratio.
		/// </summary>
		protected T _interface => Screen.width > Screen.height ? _landscapeInterface ?? _portraitInterface : _portraitInterface ?? _landscapeInterface;

		/// <summary>
		/// Has an interface been provided for this Unity Client?
		/// </summary>
		public bool HasInterface => _interface;

		/// <summary>
		/// Is there an interface and if so is it currently active?
		/// </summary>
		public bool IsActive => HasInterface && _interface.gameObject.activeInHierarchy;

		internal virtual void CreateInterface(Canvas canvas)
		{
			_landscapeInterface = SetInterface(_landscapeInterface, canvas);
			_landscapeInterface?.gameObject.SetActive(false);

			_portraitInterface = SetInterface(_portraitInterface, canvas);
			_portraitInterface?.gameObject.SetActive(false);
		}

		internal T SetInterface(T panelInterface, Canvas canvas, string extension = "")
		{
			if (!panelInterface)
			{
				return null;
			}
			var inScene = panelInterface.gameObject.scene == SceneManager.GetActiveScene() || panelInterface.gameObject.scene.name == "DontDestroyOnLoad";
			if (!inScene)
			{
				var newInterface = Instantiate(panelInterface, canvas.transform, false);
				newInterface.name = panelInterface.name + extension;
				panelInterface = newInterface;
			}
			return panelInterface;
		}

		/// <summary>
		/// Change the used interface if the aspect ratio changes.
		/// </summary>
		protected virtual void Update()
		{
			if (_landscapeInterface && _landscapeInterface != _interface && _landscapeInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_landscapeInterface.gameObject);
				_interface.Display();
			}
			if (_portraitInterface && _portraitInterface != _interface && _portraitInterface.gameObject.activeInHierarchy)
			{
				SUGARManager.unity.DisableObject(_portraitInterface.gameObject);
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
