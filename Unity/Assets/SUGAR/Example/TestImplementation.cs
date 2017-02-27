using UnityEngine;
using PlayGen.SUGAR.Unity;

public class TestImplementation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SUGARManager.Account.DisplayPanel(success =>
		{

		});
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.T))
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
			if (Input.GetKeyDown(KeyCode.S))
			{
				SUGARManager.Unity.StartSpinner();
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				SUGARManager.Unity.StopSpinner();
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				SUGARManager.Friend.Display();
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				SUGARManager.Group.Display();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
