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

		public static void TryApplySeed(string username, string password, TextAsset gameSeedText, out List<string> messages)
		{
			messages = new List<string>();

			GameSeed gameSeed;
			try
			{
				gameSeed = JsonConvert.DeserializeObject<GameSeed>(gameSeedText.text);
			}
			catch (Exception ex)
			{
				messages.Add($"Invalid game seed file. {ex}");
				return;
			}

			if (string.IsNullOrEmpty(gameSeed.game))
			{
				messages.Add("A game token must be provided in the seeding json");
				return;
			}

			var unityManager = Object.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
			if (unityManager == null)
			{
				messages.Add("The SUGAR prefab must be in the currently open scene.");
				return;
			}

			var baseAddress = string.Empty;
			if (File.Exists($"{Application.streamingAssetsPath}/SUGAR.config.json"))
			{
				var filePath = $"file:///{Application.streamingAssetsPath}/SUGAR.config.json";
				var www = new WWW(filePath);
				while (!www.isDone) { }
				baseAddress = JsonConvert.DeserializeObject<Config>(www.text).BaseUri;
			}
			if (string.IsNullOrEmpty(baseAddress))
			{
				if (string.IsNullOrEmpty(unityManager.baseAddress))
				{
					messages.Add("A base address must be provided via the Config file in StreamingAssets or via the SUGAR Unity Manager");
					return;
				}
			}
			Debug.Log(baseAddress);
			var devClient = new SUGARDevelopmentClient(baseAddress);

			if (!TryLoginAdmin(devClient, username, password, out var response, out var loginError))
			{
				messages.Add(loginError);
			}
			else
			{
				Debug.Log("Developer Sign-In Successful");
				var game = devClient.Game.Get(gameSeed.game).FirstOrDefault();
				if (game != null)
				{
					Debug.Log($"{gameSeed.game} Game Found");
					messages.Add($"Existing game with name {gameSeed.game} found.");
					SetGame(unityManager, gameSeed.game, game.Id);
				}
				else
				{
					Debug.Log($"Creating Game {gameSeed.game}");
					EditorUtility.DisplayProgressBar("SUGAR Seeding", $"Seeding {gameSeed.game}", 0);
					try
					{
						var gameResponse = devClient.Development.CreateGame(new GameRequest
						{
							Name = gameSeed.game
						});
						if (gameResponse != null)
						{
							messages.Add($"Game with name {gameSeed.game} created.");
							SetGame(unityManager, gameSeed.game, gameResponse.Id);
						}
						else
						{
							messages.Add("Unable to create game.");
							EditorUtility.ClearProgressBar();
							return;
						}
						EditorUtility.ClearProgressBar();
					}
					catch (Exception e)
					{
						messages.Add($"Unable to create game.{e}");
						EditorUtility.ClearProgressBar();
						return;
					}
				}
				if (gameSeed.achievements != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding achievements", 0);
					TryCreateAchievements(devClient, gameSeed.achievements, out var achievementMessages);
					messages.AddRange(achievementMessages);
					EditorUtility.ClearProgressBar();
				}
				if (gameSeed.skills != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding skills", 0);
					TryCreateSkills(devClient, gameSeed.skills, out var skillsMessages);
					messages.AddRange(skillsMessages);
					EditorUtility.ClearProgressBar();
				}
				if (gameSeed.leaderboards != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding leaderboards", 0);
					TryCreateLeaderboards(devClient, gameSeed.leaderboards, out var leaderboardMessages);
					messages.AddRange(leaderboardMessages);
					EditorUtility.ClearProgressBar();
				}
				if (gameSeed.groups != null)
				{
					EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding groups", 0);
					TryCreateGroups(devClient, gameSeed.groups, out var groupMessages);
					messages.AddRange(groupMessages);
					EditorUtility.ClearProgressBar();
				}
				devClient.Session.Logout();
			}

			return;
		}

		private static void SetGame(SUGARUnityManager unityManager, string gameToken, int gameId)
		{
			unityManager.gameToken = gameToken;
			unityManager.gameId = gameId;
			EditorUtility.SetDirty(unityManager);
			SUGARManager.GameId = gameId;
		}

		private static void TryCreateAchievements(SUGARDevelopmentClient devClient, EvaluationCreateRequest[] achievements, out List<string> messages)
		{
			messages = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var achieve in achievements)
			{
				achieve.GameId = gameId;
				try
				{
					devClient.Development.CreateAchievement(achieve);
					messages.Add($"{achieve.Name} Achievement successfully created");
				}
				catch (Exception ex)
				{
					messages.Add($"Unable to create achievement {achieve.Name}. {ex}");
				}
			}

			return;
		}

		private static void TryCreateSkills(SUGARDevelopmentClient devClient, EvaluationCreateRequest[] skills, out List<string> messages)
		{
			messages = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var skill in skills)
			{
				skill.GameId = gameId;
				try
				{
					devClient.Development.CreateSkill(skill);
					messages.Add($"{skill.Name} Skill successfully created");
				}
				catch (Exception ex)
				{
					messages.Add($"Unable to create skill {skill.Name}. {ex}");
				}
			}

			return;
		}

		private static void TryCreateLeaderboards(SUGARDevelopmentClient devClient, LeaderboardRequest[] leaderboards, out List<string> messages)
		{
			messages = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var leader in leaderboards)
			{
				leader.GameId = gameId;
				try
				{
					devClient.Development.CreateLeaderboard(leader);
					messages.Add($"{leader.Name} Leaderboard successfully created");
				}
				catch (Exception ex)
				{
					messages.Add($"Unable to create leaderboard {leader.Name}. {ex}");
				}
			}

			return;
		}

		private static void TryCreateGroups(SUGARDevelopmentClient devClient, GroupRequest[] groups, out List<string> messages)
		{
			messages = new List<string>();
			var gameId = SUGARManager.GameId;

			foreach (var group in groups)
			{
				try
				{
					devClient.Group.Create(group);
					messages.Add($"{group.Name} Group successfully created");
				}
				catch (Exception ex)
				{
					messages.Add($"Unable to create group {group.Name}. {ex}");
				}
			}

			return;
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
				error = $"Error Logging in Admin. {ex}";
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
					SeedGame.TryApplySeed(_username, _password, _gameSeed, out var messages);
					messages.Add("End of Seeding");
					message = string.Join("\n", messages.ToArray());
					Debug.LogError($"Game Seed\n{message}");

					EditorUtility.DisplayDialog("Seed Game", message, "OK");
				}
			}
		}
	}

	internal class GameSeed
	{
		public string game = string.Empty;
		public EvaluationCreateRequest[] achievements = new EvaluationCreateRequest[0];
		public EvaluationCreateRequest[] skills = new EvaluationCreateRequest[0];
		public LeaderboardRequest[] leaderboards = new LeaderboardRequest[0];
		public GroupRequest[] groups = new GroupRequest[0];
	}
}
