﻿using UnityEngine;
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
				SUGARManager.GameLeaderboard.DisplayGameList();
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
				SUGARManager.Evaluation.DisplayAchievementList();
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				SUGARManager.Evaluation.ForceNotificationTest();
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
				SUGARManager.UserFriend.Display();
			}
			if (Input.GetKeyDown(KeyCode.G))
			{
				SUGARManager.UserGroup.Display();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
