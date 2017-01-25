using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace PlayGen.SUGAR.Unity.Editor
{
	public static class SeedAchievements
	{
		[MenuItem("Tools/SUGAR/Seed Game")]
		public static void SeedAchivements()
		{
			AdminLogIn window = ScriptableObject.CreateInstance<AdminLogIn>();
			window.titleContent.text = "Seed Game";
			window.Show();
		}

		public static void LogInUser(string username, string password, TextAsset textAsset)
		{
			var unityManager = GameObject.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
			if (unityManager == null)
			{
				return;
			}
			SUGARManager.Client = new SUGARClient(unityManager.baseAddress);
			var response = LoginAdmin(username, password);
			if (response != null)
			{
				Debug.Log("Admin Login SUCCESS");
				var game = SUGARManager.Client.Game.Get(unityManager.gameToken).FirstOrDefault();
				if (game != null)
				{
					Debug.Log("Game Found");
					unityManager.gameId = game.Id;
					SUGARManager.GameId = game.Id;
				}
				else
				{
					Debug.Log("Creating Game");
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding " + unityManager.gameToken, 0);
					var gameResponse = SUGARManager.Client.Game.Create(new GameRequest()
					{
						Name = unityManager.gameToken
					});
					if (gameResponse != null)
					{
						unityManager.gameId = gameResponse.Id;
						SUGARManager.GameId = gameResponse.Id;
					}
					else
					{
						Debug.LogError("Unable to create game");
						return;
					}
					EditorUtility.ClearProgressBar();
				}
				var gameSeed = JsonConvert.DeserializeObject<GameSeed>(textAsset.text);
				EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding achievements", 0);
				CreateAchievements(gameSeed.Achievements);
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding leaderboards", 0);
				CreateLeaderboards(gameSeed.Leaderboards);
				EditorUtility.ClearProgressBar();
				SUGARManager.Client.Session.Logout();
			}
		}

		private static void CreateAchievements(EvaluationCreateRequest[] achievements)
		{
			var achievementClient = SUGARManager.Client.Achievement;
			var gameId = SUGARManager.GameId;

			foreach (var achieve in achievements)
			{
				achieve.GameId = gameId;
				foreach (var criteria in achieve.EvaluationCriterias)
				{
					criteria.EvaluationDataCategory = EvaluationDataCategory.GameData;
				}
				achievementClient.Create(achieve);
			}
		}

		private static void CreateLeaderboards(LeaderboardRequest[] leaderboards)
		{
			var leaderboardClient = SUGARManager.Client.Leaderboard;
			var gameId = SUGARManager.GameId;

			foreach (var leader in leaderboards)
			{
				leader.GameId = gameId;
				leaderboardClient.Create(leader);
			}
		}

		private static AccountResponse LoginAdmin(string username, string password)
		{
			try
			{
				return SUGARManager.Client.Session.Login(new AccountRequest()
				{
					Name = username,
					Password = password,
					SourceToken = "SUGAR"
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Error Logging in Admin");
				Debug.Log(ex.Message);
				return null;
			}
		}
	}

	public class AdminLogIn : EditorWindow
	{
		string username;
		string password;
		TextAsset textAsset;

		void OnGUI()
		{
			username = EditorGUILayout.TextField("Username", username, EditorStyles.textField);
			password = EditorGUILayout.PasswordField("Password", password);
			textAsset = (TextAsset)EditorGUILayout.ObjectField("Game Seed File", textAsset, typeof(TextAsset), false);
			if (textAsset)
			{
				if (GUILayout.Button("Sign-in"))
				{
					SeedAchievements.LogInUser(username, password, textAsset);
				}
			}
		}
	}

	internal class GameSeed
	{
		public EvaluationCreateRequest[] Achievements;
		public LeaderboardRequest[] Leaderboards;
	}
}
