using UnityEngine;
using SUGAR.Unity;

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
		if (Input.GetKeyDown(KeyCode.A))
		{
			SUGARManager.Achievement.DisplayList();
		}
	}
}
