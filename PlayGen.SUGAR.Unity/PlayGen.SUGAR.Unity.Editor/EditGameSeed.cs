using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlayGen.SUGAR.Common.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PlayGen.SUGAR.Unity.Editor
{
    public static class EditGameSeed
    {
        [MenuItem("SUGAR/Edit Game Seed")]
        public static void EditSeed()
        {
            GameSeedWindow window = ScriptableObject.CreateInstance<GameSeedWindow>();
            window.titleContent.text = "Edit Game Seed";
            window.Show();
        }
    }

    public class GameSeedWindow : EditorWindow
    {
        private TextAsset _textAsset;
        private TextAsset _saveAsset;
        private GameSeed _seed;

        private bool _showAchievements;
        private List<bool> _showAchievementList = new List<bool>();
        private List<bool> _showAchievementCriteria = new List<bool>();
        private List<bool> _showAchievementReward = new List<bool>();
        private List<List<bool>> _showAchievementCriteriaList = new List<List<bool>>();
        private List<List<bool>> _showAchievementRewardList = new List<List<bool>>();
        private bool _showLeaderboards;
        private List<bool> _showLeaderboardList = new List<bool>();

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 225f;
            EditorGUILayout.BeginHorizontal();
            _textAsset = (TextAsset)EditorGUILayout.ObjectField("Game Seed File", _textAsset, typeof(TextAsset), false);
            if (_textAsset)
            {
                if (GUILayout.Button("Load", GUILayout.ExpandWidth(false)))
                {
                    var gameSeed = new GameSeed();
                    try
                    {
                        gameSeed = JsonConvert.DeserializeObject<GameSeed>(_textAsset.text);
                        _seed = gameSeed;
                        SetFoldOut();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Invalid game seed file. " + ex.Message);
                        return;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (_seed != null)
            {
                _seed.game = EditorGUILayout.TextField("Name", _seed.game ?? string.Empty, EditorStyles.textField);
                _showAchievements = EditorGUILayout.Foldout(_showAchievements, "Achievements");
                if (_showAchievements)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < _seed.achievements.Length; i++)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical();
                        _showAchievementList[i] = EditorGUILayout.Foldout(_showAchievementList[i], _seed.achievements[i].Name);
                        if (_showAchievementList[i])
                        {
                            _seed.achievements[i].Name = EditorGUILayout.TextField("Name", _seed.achievements[i].Name ?? string.Empty, EditorStyles.textField);
                            _seed.achievements[i].Description = EditorGUILayout.TextField("Description", _seed.achievements[i].Description ?? string.Empty, EditorStyles.textField);
                            _seed.achievements[i].Token = EditorGUILayout.TextField("Token", _seed.achievements[i].Token ?? string.Empty, EditorStyles.textField);
                            _seed.achievements[i].ActorType = (ActorType)EditorGUILayout.EnumPopup("ActorType", _seed.achievements[i].ActorType, EditorStyles.popup);
                            _showAchievementCriteria[i] = EditorGUILayout.Foldout(_showAchievementCriteria[i], "Evaluation Criteria");
                            if (_showAchievementCriteria[i])
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.BeginVertical();
                                for (int j = 0; j < _seed.achievements[i].EvaluationCriterias.Count; j++)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.BeginVertical();
                                    _showAchievementCriteriaList[i][j] = EditorGUILayout.Foldout(_showAchievementCriteriaList[i][j], _seed.achievements[i].EvaluationCriterias[j].EvaluationDataKey);
                                    if (_showAchievementCriteriaList[i][j])
                                    {
                                        _seed.achievements[i].EvaluationCriterias[j].EvaluationDataKey = EditorGUILayout.TextField("EvaluationDataKey", _seed.achievements[i].EvaluationCriterias[j].EvaluationDataKey ?? string.Empty, EditorStyles.textField);
                                        _seed.achievements[i].EvaluationCriterias[j].ComparisonType = (ComparisonType)EditorGUILayout.EnumPopup("ComparisonType", _seed.achievements[i].EvaluationCriterias[j].ComparisonType, EditorStyles.popup);
                                        _seed.achievements[i].EvaluationCriterias[j].CriteriaQueryType = (CriteriaQueryType)EditorGUILayout.EnumPopup("CriteriaQueryType", _seed.achievements[i].EvaluationCriterias[j].CriteriaQueryType, EditorStyles.popup);
                                        _seed.achievements[i].EvaluationCriterias[j].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _seed.achievements[i].EvaluationCriterias[j].EvaluationDataType, EditorStyles.popup);
                                        _seed.achievements[i].EvaluationCriterias[j].Scope = (CriteriaScope)EditorGUILayout.EnumPopup("Scope", _seed.achievements[i].EvaluationCriterias[j].Scope, EditorStyles.popup);
                                        _seed.achievements[i].EvaluationCriterias[j].Value = EditorGUILayout.TextField("Value", _seed.achievements[i].EvaluationCriterias[j].Value ?? string.Empty, EditorStyles.textField);
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
                                        if (GUILayout.Button("Remove Criteria", GUILayout.ExpandWidth(false)))
                                        {
                                            _seed.achievements[i].EvaluationCriterias.RemoveAt(j);
                                            _showAchievementCriteriaList[i].RemoveAt(j);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUI.indentLevel--;
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
                                if (GUILayout.Button("Add Criteria", GUILayout.ExpandWidth(false)))
                                {
                                    _seed.achievements[i].EvaluationCriterias.Add(new Contracts.Shared.EvaluationCriteriaCreateRequest());
                                    _showAchievementCriteriaList[i].Add(false);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                            }
                            _showAchievementReward[i] = EditorGUILayout.Foldout(_showAchievementReward[i], "Rewards");
                            if (_showAchievementReward[i])
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.BeginVertical();
                                for (int j = 0; j < _seed.achievements[i].Rewards.Count; j++)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.BeginVertical();
                                    _showAchievementRewardList[i][j] = EditorGUILayout.Foldout(_showAchievementRewardList[i][j], _seed.achievements[i].Rewards[j].EvaluationDataKey);
                                    if (_showAchievementRewardList[i][j])
                                    {
                                        _seed.achievements[i].Rewards[j].EvaluationDataKey = EditorGUILayout.TextField("EvaluationDataKey", _seed.achievements[i].Rewards[j].EvaluationDataKey ?? string.Empty, EditorStyles.textField);
                                        _seed.achievements[i].Rewards[j].EvaluationDataCategory = (EvaluationDataCategory)EditorGUILayout.EnumPopup("EvaluationDataCategory", _seed.achievements[i].Rewards[j].EvaluationDataCategory, EditorStyles.popup);
                                        _seed.achievements[i].Rewards[j].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _seed.achievements[i].Rewards[j].EvaluationDataType, EditorStyles.popup);
                                        _seed.achievements[i].Rewards[j].Value = EditorGUILayout.TextField("Value", _seed.achievements[i].Rewards[j].Value ?? string.Empty, EditorStyles.textField);
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
                                        if (GUILayout.Button("Remove Reward", GUILayout.ExpandWidth(false)))
                                        {
                                            _seed.achievements[i].Rewards.RemoveAt(j);
                                            _showAchievementRewardList[i].RemoveAt(j);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUI.indentLevel--;
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
                                if (GUILayout.Button("Add Reward", GUILayout.ExpandWidth(false)))
                                {
                                    _seed.achievements[i].Rewards.Add(new Contracts.Shared.RewardCreateRequest());
                                    _showAchievementRewardList[i].Add(false);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space((EditorGUI.indentLevel - 1) * 35);
                            if (GUILayout.Button("Remove Achievement", GUILayout.ExpandWidth(false)))
                            {
                                var list = _seed.achievements.ToList();
                                list.RemoveAt(i);
                                _showAchievementList.RemoveAt(i);
                                _showAchievementCriteria.RemoveAt(i);
                                _showAchievementReward.RemoveAt(i);
                                _showAchievementCriteriaList.RemoveAt(i);
                                _showAchievementRewardList.RemoveAt(i);
                                _seed.achievements = list.ToArray();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }
                    if (GUILayout.Button("Add Achievement", GUILayout.ExpandWidth(false)))
                    {
                        var list = _seed.achievements.ToList();
                        list.Add(new Contracts.Shared.EvaluationCreateRequest());
                        list.Last().EvaluationCriterias = new List<Contracts.Shared.EvaluationCriteriaCreateRequest>();
                        list.Last().Rewards = new List<Contracts.Shared.RewardCreateRequest>();
                        _showAchievementList.Add(false);
                        _showAchievementCriteria.Add(false);
                        _showAchievementReward.Add(false);
                        _showAchievementCriteriaList.Add(new List<bool>());
                        _showAchievementRewardList.Add(new List<bool>());
                        _seed.achievements = list.ToArray();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                _showLeaderboards = EditorGUILayout.Foldout(_showLeaderboards, "Leaderboards");
                if (_showLeaderboards)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < _seed.leaderboards.Length; i++)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical();
                        _showLeaderboardList[i] = EditorGUILayout.Foldout(_showLeaderboardList[i], _seed.leaderboards[i].Name);
                        if (_showLeaderboardList[i])
                        {
                            _seed.leaderboards[i].Token = EditorGUILayout.TextField("Token", _seed.leaderboards[i].Token ?? string.Empty, EditorStyles.textField);
                            _seed.leaderboards[i].Name = EditorGUILayout.TextField("Name", _seed.leaderboards[i].Name ?? string.Empty, EditorStyles.textField);
                            _seed.leaderboards[i].Key = EditorGUILayout.TextField("Description", _seed.leaderboards[i].Key ?? string.Empty, EditorStyles.textField);
                            _seed.leaderboards[i].ActorType = (ActorType)EditorGUILayout.EnumPopup("ActorType", _seed.leaderboards[i].ActorType, EditorStyles.popup);
                            _seed.leaderboards[i].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _seed.leaderboards[i].EvaluationDataType, EditorStyles.popup);
                            _seed.leaderboards[i].CriteriaScope = (CriteriaScope)EditorGUILayout.EnumPopup("CriteriaScope", _seed.leaderboards[i].CriteriaScope, EditorStyles.popup);
                            _seed.leaderboards[i].LeaderboardType = (LeaderboardType)EditorGUILayout.EnumPopup("LeaderboardType", _seed.leaderboards[i].LeaderboardType, EditorStyles.popup);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space((EditorGUI.indentLevel - 1) * 35);
                            if (GUILayout.Button("Remove Leaderboard", GUILayout.ExpandWidth(false)))
                            {
                                var list = _seed.leaderboards.ToList();
                                list.RemoveAt(i);
                                _showLeaderboardList.RemoveAt(i);
                                _seed.leaderboards = list.ToArray();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }
                    if (GUILayout.Button("Add Leaderboard", GUILayout.ExpandWidth(false)))
                    {
                        var list = _seed.leaderboards.ToList();
                        list.Add(new Contracts.Shared.LeaderboardRequest());
                        _showLeaderboardList.Add(false);
                        _seed.leaderboards = list.ToArray();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.BeginHorizontal();
                _saveAsset = (TextAsset)EditorGUILayout.ObjectField("Save File", _saveAsset, typeof(TextAsset), false);
                if (_saveAsset != null)
                {
                    if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                    {
                        try
                        {
                            var seedJson = JsonConvert.SerializeObject(_seed, Formatting.Indented, new StringEnumConverter());
                            File.WriteAllText(AssetDatabase.GetAssetPath(_saveAsset), seedJson);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Could not save seed to file. " + ex.Message);
                            return;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Create Game Seed", GUILayout.ExpandWidth(false)))
                {
                    _seed = new GameSeed();
                    _seed.achievements = new Contracts.Shared.EvaluationCreateRequest[0];
                    _seed.leaderboards = new Contracts.Shared.LeaderboardRequest[0];
                    SetFoldOut();
                }
            }
        }

        private void SetFoldOut()
        {
            _showAchievementList.Clear();
            _showAchievementCriteria.Clear();
            _showAchievementReward.Clear();
            _showAchievementCriteriaList.Clear();
            _showAchievementRewardList.Clear();
            var currentAchieve = 0;
            foreach (var achieve in _seed.achievements)
            {
                _showAchievementList.Add(false);
                _showAchievementCriteria.Add(false);
                _showAchievementReward.Add(false);
                _showAchievementCriteriaList.Add(new List<bool>());
                _showAchievementRewardList.Add(new List<bool>());
                if (achieve.EvaluationCriterias == null)
                {
                    achieve.EvaluationCriterias = new List<Contracts.Shared.EvaluationCriteriaCreateRequest>();
                }
                foreach (var criteria in achieve.EvaluationCriterias)
                {
                    _showAchievementCriteriaList[currentAchieve].Add(false);
                }
                if (achieve.Rewards == null)
                {
                    achieve.Rewards = new List<Contracts.Shared.RewardCreateRequest>();
                }
                foreach (var reward in achieve.Rewards)
                {
                    _showAchievementRewardList[currentAchieve].Add(false);
                }
                currentAchieve++;
            }
            _showLeaderboardList.Clear();
            foreach (var leader in _seed.leaderboards)
            {
                _showLeaderboardList.Add(false);
            }
        }
    }
}