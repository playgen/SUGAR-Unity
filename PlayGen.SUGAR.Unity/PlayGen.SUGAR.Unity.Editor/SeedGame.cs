using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

using Object = UnityEngine.Object;
using System.IO;

using PlayGen.SUGAR.Client.Development;
using PlayGen.SUGAR.Common.Authorization;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Unity.Editor
{
	public static class SeedGame
	{
		public static TextAsset DefaultGameSeed => (TextAsset) AssetDatabase.LoadAssetAtPath("Assets/SUGAR/Editor/GameSeed.json", typeof(TextAsset));

		[MenuItem("Tools/SUGAR/Seed Game")]
		public static void ShowSeedGameWindow()
		{
			var window = ScriptableObject.CreateInstance<SeedGameWindow>();
			window.titleContent.text = "Seed Game";
			window.SetGameSeed(DefaultGameSeed);
			window.Show();
		}

		public static bool TryApplySeed(string username, string password, TextAsset gameSeedText, out List<string> errors)
		{
			errors = new List<string>();

			GameSeed gameSeed;
			try
			{
				gameSeed = JsonConvert.DeserializeObject<GameSeed>(gameSeedText.text);
			}
			catch (Exception ex)
			{
				errors.Add("Invalid game seed file. " + ex.Message);
				return false;
			}

			if (string.IsNullOrEmpty(gameSeed.game))
			{
				errors.Add("A game token must be provided in the seeding json");
				return false;
			}

			var unityManager = Object.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
			if (unityManager == null)
			{
				errors.Add("The SUGAR prefab must be in the currently open scene.");
				return false;
			}

			var baseAddress = string.Empty;
			if (File.Exists(Application.streamingAssetsPath + "/SUGAR.config.json"))
			{
				var filePath = "file:///" + Application.streamingAssetsPath + "/SUGAR.config.json";
				var www = new WWW(filePath);
				while (!www.isDone) { }
				baseAddress = JsonConvert.DeserializeObject<Config>(www.text).BaseUri;
			}
			if (string.IsNullOrEmpty(baseAddress))
			{
				if (string.IsNullOrEmpty(unityManager.baseAddress))
				{
					errors.Add("A base address must be provided via the Config file in StreamingAssets or via the SUGAR Unity Manager");
					return false;
				}
			}
			Debug.Log(baseAddress);
			var devClient = new SUGARDevelopmentClient(baseAddress);

			if (!TryLoginAdmin(devClient, username, password, out var response, out var loginError))
			{
				errors.Add(loginError);
			}
			else
			{
				Debug.Log("Developer Sign-In Successful");
				var game = devClient.Game.Get(gameSeed.game).FirstOrDefault();
				if (game != null)
				{
					Debug.Log(gameSeed.game + " Game Found");
					unityManager.gameToken = gameSeed.game;
					unityManager.gameId = game.Id;
					SUGARManager.GameId = game.Id;
				}
				else
				{
					Debug.Log("Creating Game " + gameSeed.game);
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding " + gameSeed.game, 0);
					try
					{
						var gameResponse = devClient.Development.CreateGame(new GameRequest
						{
							Name = gameSeed.game
						});
						if (gameResponse != null)
						{
							unityManager.gameToken = gameSeed.game;
							unityManager.gameId = gameResponse.Id;
							SUGARManager.GameId = gameResponse.Id;
						}
						else
						{
							errors.Add("Unable to create game.");
							EditorUtility.ClearProgressBar();
							return false;
						}
						EditorUtility.ClearProgressBar();
					}
					catch (Exception e)
					{
						errors.Add("Unable to create game." + e.Message);
						EditorUtility.ClearProgressBar();
						return false;
					}
				}
				if (gameSeed.achievements != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding achievements", 0);
					if (!TryCreateAchievements(devClient, gameSeed.achievements, out var achievementErrors))
					{
						errors.AddRange(achievementErrors);
					}
					EditorUtility.ClearProgressBar();
				}
				if (gameSeed.skills != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding skills", 0);
					if (!TryCreateSkills(devClient, gameSeed.skills, out var skillsErrors))
					{
						errors.AddRange(skillsErrors);
					}
					EditorUtility.ClearProgressBar();
				}
				if (gameSeed.leaderboards != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding leaderboards", 0);
					if (!TryCreateLeaderboards(devClient, gameSeed.leaderboards, out var leaderboardErrors))
					{
						errors.AddRange(leaderboardErrors);
					}
					EditorUtility.ClearProgressBar();
				}
				devClient.Session.Logout();
			}

			return !errors.Any();
		}

		private static bool TryCreateAchievements(SUGARDevelopmentClient devClient, EvaluationCreateRequest[] achievements, out List<string> errors)
		{
			errors = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var achieve in achievements)
			{
				achieve.GameId = gameId;
				try
				{
					devClient.Development.CreateAchievement(achieve);
				}
				catch (Exception ex)
				{
					errors.Add("Unable to create achievement " + achieve.Name + ". " + ex.Message);
				}
			}

			return !errors.Any();
		}

		private static bool TryCreateSkills(SUGARDevelopmentClient devClient, EvaluationCreateRequest[] skills, out List<string> errors)
		{
			errors = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var skill in skills)
			{
				skill.GameId = gameId;
				try
				{
					devClient.Development.CreateSkill(skill);
				}
				catch (Exception ex)
				{
					errors.Add("Unable to create skill " + skill.Name + ". " + ex.Message);
				}
			}

			return !errors.Any();
		}

		private static bool TryCreateLeaderboards(SUGARDevelopmentClient devClient, LeaderboardRequest[] leaderboards, out List<string> errors)
		{
			errors = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var leader in leaderboards)
			{
				leader.GameId = gameId;
				try
				{
					devClient.Development.CreateLeaderboard(leader);
				}
				catch (Exception ex)
				{
					errors.Add("Unable to create leaderboard " + leader.Name + ". " + ex.Message);
				}
			}

			return !errors.Any();
		}

		private static bool TryLoginAdmin(SUGARDevelopmentClient devClient, string username, string password, out AccountResponse response, out string error)
		{
			error = null;
			response = null;
			try
			{
				response = devClient.Session.Login(Platform.GlobalId, new AccountRequest
				{
					Name = username,
					Password = password,
					SourceToken = "SUGAR"
				});
				return true;
			}
			catch (Exception ex)
			{
				error = "Error Logging in Admin. " + ex.Message;
				return false;
			}
		}
	}

	public class SeedGameWindow : EditorWindow
	{
		private string _username;
		private string _password;
		private TextAsset _gameSeed;

		public void SetGameSeed(TextAsset gameSeed)
		{
			_gameSeed = gameSeed;
		}

		private void OnGUI()
		{
			_username = EditorGUILayout.TextField("Username", _username, EditorStyles.textField);
			_password = EditorGUILayout.PasswordField("Password", _password);
			_gameSeed = (TextAsset)EditorGUILayout.ObjectField("Game Seed File", _gameSeed, typeof(TextAsset), false);

			if (_gameSeed)
			{
				if (GUILayout.Button("Sign-in and Seed"))
				{
					string message;
					if (SeedGame.TryApplySeed(_username, _password, _gameSeed, out var errors))
					{
						message = "Success!";
					}
					else
					{
						message = $"Failed: \n\n{string.Join("\n", errors.ToArray())}";
						Debug.LogError($"Game Seed {message}");
					}

					EditorUtility.DisplayDialog("Seed Game", message, "OK");
				}
			}
		}
	}

	internal class GameSeed
	{
		public string game;
		public EvaluationCreateRequest[] achievements;
		public EvaluationCreateRequest[] skills;
		public LeaderboardRequest[] leaderboards;
	}
}
