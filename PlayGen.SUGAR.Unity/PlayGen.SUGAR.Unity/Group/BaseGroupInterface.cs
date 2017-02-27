using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public abstract class BaseGroupInterface : MonoBehaviour
	{
		internal void Display(bool loadingSuccess = true)
		{
			PreDisplay();
			ShowGroupList(loadingSuccess);
		}

		protected abstract void PreDisplay();

		protected void ShowGroupList(bool loadingSuccess)
		{
			PreDraw();
			DrawGroupList(loadingSuccess);
			PostDraw(loadingSuccess);
		}

		private void PreDraw()
		{

		}

		protected abstract void DrawGroupList(bool loadingSuccess);

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
