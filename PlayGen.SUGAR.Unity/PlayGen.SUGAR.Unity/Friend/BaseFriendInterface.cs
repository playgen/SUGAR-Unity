using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseFriendInterface : MonoBehaviour
	{
		internal void Display(bool loadingSuccess = true)
		{
			PreDisplay();
			ShowFriendsList(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowFriendsList(bool loadingSuccess)
		{
			PreDraw();
			DrawFriendsList(loadingSuccess);
			PostDraw(loadingSuccess);
		}

		private void PreDraw()
		{
			
		}

		protected abstract void DrawFriendsList(bool loadingSuccess);

		private void PostDraw(bool loadingSuccess)
		{
			
		}

		private void AttemptSignIn()
		{
			SUGARManager.account.DisplayPanel(success =>
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
