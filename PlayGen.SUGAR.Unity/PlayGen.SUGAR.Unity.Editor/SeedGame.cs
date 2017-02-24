using System;
using System.Linq;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

using Object = UnityEngine.Object;

namespace PlayGen.SUGAR.Unity.Editor
{
	public static class SeedGame
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
			var unityManager = Object.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
			if (unityManager == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(unityManager.gameToken))
			{
				Debug.LogError("A game token must be provided in the SUGAR Unity Manager");
				return;
			}
			SUGARManager.client = new SUGARClient(unityManager.baseAddress);
			var response = LoginAdmin(username, password);
			if (response != null)
			{
				Debug.Log("Admin Login SUCCESS");
				var game = SUGARManager.client.Game.Get(unityManager.gameToken).FirstOrDefault();
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
					var gameResponse = SUGARManager.client.Game.Create(new GameRequest()
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
				CreateAchievements(gameSeed.achievements);
				EditorUtility.ClearProgressBar();
				EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding leaderboards", 0);
				CreateLeaderboards(gameSeed.leaderboards);
				EditorUtility.ClearProgressBar();
				SUGARManager.client.Session.Logout();
			}
		}

		private static void CreateAchievements(EvaluationCreateRequest[] achievements)
		{
			var achievementClient = SUGARManager.client.Achievement;
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
			var leaderboardClient = SUGARManager.client.Leaderboard;
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
				return SUGARManager.client.Session.Login(new AccountRequest()
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
		private string _username;
		private string _password;
		private TextAsset _textAsset;

		void OnGUI()
		{
			_username = EditorGUILayout.TextField("Username", _username, EditorStyles.textField);
			_password = EditorGUILayout.PasswordField("Password", _password);
			_textAsset = (TextAsset)EditorGUILayout.ObjectField("Game Seed File", _textAsset, typeof(TextAsset), false);
			if (_textAsset)
			{
				if (GUILayout.Button("Sign-in"))
				{
					SeedGame.LogInUser(_username, _password, _textAsset);
				}
			}
		}
	}

	internal class GameSeed
	{
		public EvaluationCreateRequest[] achievements;
		public LeaderboardRequest[] leaderboards;
	}
}
