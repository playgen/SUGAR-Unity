using System;
using System.Linq;

using PlayGen.SUGAR.Client;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

using Object = UnityEngine.Object;
using System.IO;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Unity.Editor
{
    public static class SeedGame
    {
        [MenuItem("SUGAR/Seed Game")]
        public static void SeedAchivements()
        {
            AdminLogIn window = ScriptableObject.CreateInstance<AdminLogIn>();
            window.titleContent.text = "Seed Game";
            window.Show();
        }

        public static void LogInUser(string username, string password, TextAsset textAsset)
        {
            var gameSeed = new GameSeed();
            try
            {
                gameSeed = JsonConvert.DeserializeObject<GameSeed>(textAsset.text);
            }
            catch (Exception ex)
            {
                Debug.LogError("Invalid game seed file. " + ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(gameSeed.game))
            {
                Debug.LogError("A game token must be provided in the seeding json");
                return;
            }

            var unityManager = Object.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
            if (unityManager == null)
            {
                Debug.LogError("A SUGAR Unity Manager object must be in the currently open scene");
                return;
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
                    Debug.LogError("A base address must be provided via the Config file in StreamingAssets or via the SUGAR Unity Manager");
                    return;
                }
            }
            Debug.Log(baseAddress);
            SUGARManager.client = new SUGARClient(baseAddress);

            var response = LoginAdmin(username, password);
            if (response != null)
            {
                Debug.Log("Developer Sign-In Successful");
                var game = SUGARManager.client.Game.Get(gameSeed.game).FirstOrDefault();
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
                        var gameResponse = SUGARManager.client.Game.Create(new GameRequest()
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
                            Debug.LogError("Unable to create game.");
                            EditorUtility.ClearProgressBar();
                            return;
                        }
                        EditorUtility.ClearProgressBar();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Unable to create game." + e.Message);
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                }
                if (gameSeed.achievements != null)
                {
                    EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding achievements", 0);
                    CreateAchievements(gameSeed.achievements);
                    EditorUtility.ClearProgressBar();
                }
                if (gameSeed.leaderboards != null)
                {
                    EditorUtility.DisplayProgressBar("SUGAR Seeding", "Seeding leaderboards", 0);
                    CreateLeaderboards(gameSeed.leaderboards);
                    EditorUtility.ClearProgressBar();
                }
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
				try
                {
                    EvaluationResponse response = achievementClient.Create(achieve);
				}
                catch (Exception e)
                {
                    Debug.LogError("Unable to create achievement " + achieve.Name + ". " + e.Message);
                }
            }
        }

        private static void CreateLeaderboards(LeaderboardRequest[] leaderboards)
        {
            var leaderboardClient = SUGARManager.client.Leaderboard;
            var gameId = SUGARManager.GameId;

            foreach (var leader in leaderboards)
            {
                leader.GameId = gameId;
                try
                {
                    leaderboardClient.Create(leader);
                }
                catch (Exception e)
                {
                    Debug.LogError("Unable to create leaderboard " + leader.Name + ". " + e.Message);
                }
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
                Debug.LogError("Error Logging in Admin. " + ex.Message);
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
        public string game;
        public EvaluationCreateRequest[] achievements;
        public LeaderboardRequest[] leaderboards;
    }
}
