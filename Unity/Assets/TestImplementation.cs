using UnityEngine;
using PlayGen.SUGAR.Unity;

public class TestImplementation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SUGARManager.Account.TrySignIn(success =>
		{

		});
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			SUGARManager.GameLeaderboard.DisplayList();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			SUGARManager.Achievement.DisplayList();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			SUGARManager.Achievement.ForceNotificationTest();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
