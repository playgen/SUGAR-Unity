using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayGen.SUGAR.Common.Shared;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseLeaderboardListUserInterface : MonoBehaviour
	{
		protected ActorType _actorType;
		[SerializeField]
		protected Button _userButton;
		[SerializeField]
		protected Button _groupButton;
		[SerializeField]
		protected Button _combinedButton;
		[SerializeField]
		protected Text _errorText;
		[SerializeField]
		protected Button _closeButton;
		[SerializeField]
		protected Button _signinButton;

		protected virtual void Awake()
		{
			if (_userButton)
			{
				_userButton.onClick.AddListener(delegate { UpdateFilter(1); });
			}
			if (_groupButton)
			{
				_groupButton.onClick.AddListener(delegate { UpdateFilter(2); });
			}
			if (_combinedButton)
			{
				_combinedButton.onClick.AddListener(delegate { UpdateFilter(0); });
			}
			if (_closeButton)
			{
				_closeButton.onClick.AddListener(delegate { SUGARManager.Unity.DisableObject(gameObject); });
			}
			if (_signinButton)
			{
				_signinButton.onClick.AddListener(AttemptSignIn);
			}
		}

		internal void Display(ActorType filter, bool loadingSuccess)
		{
			PreDisplay();
			_actorType = filter;
			ShowLeaderboards(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected abstract void ShowLeaderboards(bool loadingSuccess);

		private void UpdateFilter(int filter)
		{
			Display((ActorType)filter, true);
		}

		private void AttemptSignIn()
		{
			SUGARManager.Account.DisplayPanel(success =>
			{
				if (success)
				{
					OnSignIn();
				}
			});
		}

		protected abstract void OnSignIn();
	}
}