using UnityEngine;
using SUGAR.Unity;

public class TestImplementation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SUGARManager.Account.SignIn(success =>
		{
			if (success)
			{
				SUGARManager.Achievement.DisplayList();
				SUGARManager.GameLeaderboard.DisplayList();
			}
		});
	}
}
