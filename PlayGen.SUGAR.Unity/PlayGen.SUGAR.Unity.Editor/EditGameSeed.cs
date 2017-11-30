﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PlayGen.SUGAR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using UnityEditor;
using UnityEngine;

namespace PlayGen.SUGAR.Unity.Editor
{
	public static class EditGameSeed
	{
		[MenuItem("SUGAR/Edit Game Seed")]
		public static void ShowEditGameSeed()
		{
			var window = ScriptableObject.CreateInstance<EditGameSeedWindow>();
			window.titleContent.text = "Edit Game Seed";
			window.SetGameSeed(SeedGame.DefaultGameSeed);
			window.Show();
		}
	}

	public class EditGameSeedWindow : EditorWindow
	{
		private TextAsset _gameSeedLoadText;
		private TextAsset _gameSeedSaveText;
		private GameSeed _gameSeed;

		private bool _showAchievements;
		private readonly List<bool> _showAchievementList = new List<bool>();
		private readonly List<bool> _showAchievementCriteria = new List<bool>();
		private readonly List<bool> _showAchievementReward = new List<bool>();
		private readonly List<List<bool>> _showAchievementCriteriaList = new List<List<bool>>();
		private readonly List<List<bool>> _showAchievementRewardList = new List<List<bool>>();
		private bool _showLeaderboards;
		private readonly List<bool> _showLeaderboardList = new List<bool>();

		public void SetGameSeed(TextAsset gameSeedText)
		{
			_gameSeedLoadText = gameSeedText;
			_gameSeedSaveText = gameSeedText;
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 225f;
			EditorGUILayout.BeginHorizontal();
			_gameSeedLoadText = (TextAsset)EditorGUILayout.ObjectField("Game Seed File", _gameSeedLoadText, typeof(TextAsset), false);
			if (_gameSeedLoadText)
			{
				if (GUILayout.Button("Load", GUILayout.ExpandWidth(false)))
				{
					try
					{
						var gameSeed = JsonConvert.DeserializeObject<GameSeed>(_gameSeedLoadText.text);
						_gameSeed = gameSeed;
						SetFoldOut();
					}
					catch (Exception ex)
					{
						_gameSeed = null;
						var error = "Invalid game seed file. " + ex.Message;
						Debug.LogError(error);
						EditorUtility.DisplayDialog("Edit Game Seed", error, "OK");
						return;
					}
				}
			}


			EditorGUILayout.EndHorizontal();
			if (_gameSeed != null)
			{
				_gameSeed.game = EditorGUILayout.TextField("Name", _gameSeed.game ?? string.Empty, EditorStyles.textField);
				_showAchievements = EditorGUILayout.Foldout(_showAchievements, "Achievements");
				if (_showAchievements)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginVertical();
					for (var i = 0; i < _gameSeed.achievements.Length; i++)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginVertical();
						_showAchievementList[i] = EditorGUILayout.Foldout(_showAchievementList[i], _gameSeed.achievements[i].Name);
						if (_showAchievementList[i])
						{
							_gameSeed.achievements[i].Name = EditorGUILayout.TextField("Name", _gameSeed.achievements[i].Name ?? string.Empty, EditorStyles.textField);
							_gameSeed.achievements[i].Description = EditorGUILayout.TextField("Description", _gameSeed.achievements[i].Description ?? string.Empty, EditorStyles.textField);
							_gameSeed.achievements[i].Token = EditorGUILayout.TextField("Token", _gameSeed.achievements[i].Token ?? string.Empty, EditorStyles.textField);
							_gameSeed.achievements[i].ActorType = (ActorType)EditorGUILayout.EnumPopup("ActorType", _gameSeed.achievements[i].ActorType, EditorStyles.popup);
							_showAchievementCriteria[i] = EditorGUILayout.Foldout(_showAchievementCriteria[i], "Evaluation Criteria");
							if (_showAchievementCriteria[i])
							{
								EditorGUI.indentLevel++;
								EditorGUILayout.BeginVertical();
								for (var j = 0; j < _gameSeed.achievements[i].EvaluationCriterias.Count; j++)
								{
									EditorGUI.indentLevel++;
									EditorGUILayout.BeginVertical();
									_showAchievementCriteriaList[i][j] = EditorGUILayout.Foldout(_showAchievementCriteriaList[i][j], _gameSeed.achievements[i].EvaluationCriterias[j].EvaluationDataKey);
									if (_showAchievementCriteriaList[i][j])
									{
										_gameSeed.achievements[i].EvaluationCriterias[j].EvaluationDataKey = EditorGUILayout.TextField("EvaluationDataKey", _gameSeed.achievements[i].EvaluationCriterias[j].EvaluationDataKey ?? string.Empty, EditorStyles.textField);
										_gameSeed.achievements[i].EvaluationCriterias[j].ComparisonType = (ComparisonType)EditorGUILayout.EnumPopup("ComparisonType", _gameSeed.achievements[i].EvaluationCriterias[j].ComparisonType, EditorStyles.popup);
										_gameSeed.achievements[i].EvaluationCriterias[j].CriteriaQueryType = (CriteriaQueryType)EditorGUILayout.EnumPopup("CriteriaQueryType", _gameSeed.achievements[i].EvaluationCriterias[j].CriteriaQueryType, EditorStyles.popup);
										_gameSeed.achievements[i].EvaluationCriterias[j].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _gameSeed.achievements[i].EvaluationCriterias[j].EvaluationDataType, EditorStyles.popup);
										_gameSeed.achievements[i].EvaluationCriterias[j].Scope = (CriteriaScope)EditorGUILayout.EnumPopup("Scope", _gameSeed.achievements[i].EvaluationCriterias[j].Scope, EditorStyles.popup);
										_gameSeed.achievements[i].EvaluationCriterias[j].Value = EditorGUILayout.TextField("Value", _gameSeed.achievements[i].EvaluationCriterias[j].Value ?? string.Empty, EditorStyles.textField);
										EditorGUILayout.BeginHorizontal();
										GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
										if (GUILayout.Button("Remove Criteria", GUILayout.ExpandWidth(false)))
										{
											_gameSeed.achievements[i].EvaluationCriterias.RemoveAt(j);
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
									_gameSeed.achievements[i].EvaluationCriterias.Add(new EvaluationCriteriaCreateRequest());
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
								for (var j = 0; j < _gameSeed.achievements[i].Rewards.Count; j++)
								{
									EditorGUI.indentLevel++;
									EditorGUILayout.BeginVertical();
									_showAchievementRewardList[i][j] = EditorGUILayout.Foldout(_showAchievementRewardList[i][j], _gameSeed.achievements[i].Rewards[j].EvaluationDataKey);
									if (_showAchievementRewardList[i][j])
									{
										_gameSeed.achievements[i].Rewards[j].EvaluationDataKey = EditorGUILayout.TextField("EvaluationDataKey", _gameSeed.achievements[i].Rewards[j].EvaluationDataKey ?? string.Empty, EditorStyles.textField);
										_gameSeed.achievements[i].Rewards[j].EvaluationDataCategory = (EvaluationDataCategory)EditorGUILayout.EnumPopup("EvaluationDataCategory", _gameSeed.achievements[i].Rewards[j].EvaluationDataCategory, EditorStyles.popup);
										_gameSeed.achievements[i].Rewards[j].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _gameSeed.achievements[i].Rewards[j].EvaluationDataType, EditorStyles.popup);
										_gameSeed.achievements[i].Rewards[j].Value = EditorGUILayout.TextField("Value", _gameSeed.achievements[i].Rewards[j].Value ?? string.Empty, EditorStyles.textField);
										EditorGUILayout.BeginHorizontal();
										GUILayout.Space((EditorGUI.indentLevel - 2) * 35);
										if (GUILayout.Button("Remove Reward", GUILayout.ExpandWidth(false)))
										{
											_gameSeed.achievements[i].Rewards.RemoveAt(j);
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
									_gameSeed.achievements[i].Rewards.Add(new RewardCreateRequest());
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
								var list = _gameSeed.achievements.ToList();
								list.RemoveAt(i);
								_showAchievementList.RemoveAt(i);
								_showAchievementCriteria.RemoveAt(i);
								_showAchievementReward.RemoveAt(i);
								_showAchievementCriteriaList.RemoveAt(i);
								_showAchievementRewardList.RemoveAt(i);
								_gameSeed.achievements = list.ToArray();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUI.indentLevel--;
						EditorGUILayout.EndVertical();
					}
					if (GUILayout.Button("Add Achievement", GUILayout.ExpandWidth(false)))
					{
						var list = _gameSeed.achievements.ToList();
						list.Add(new EvaluationCreateRequest());
						list.Last().EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>();
						list.Last().Rewards = new List<RewardCreateRequest>();
						_showAchievementList.Add(false);
						_showAchievementCriteria.Add(false);
						_showAchievementReward.Add(false);
						_showAchievementCriteriaList.Add(new List<bool>());
						_showAchievementRewardList.Add(new List<bool>());
						_gameSeed.achievements = list.ToArray();
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}
				_showLeaderboards = EditorGUILayout.Foldout(_showLeaderboards, "Leaderboards");
				if (_showLeaderboards)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginVertical();
					for (var i = 0; i < _gameSeed.leaderboards.Length; i++)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginVertical();
						_showLeaderboardList[i] = EditorGUILayout.Foldout(_showLeaderboardList[i], _gameSeed.leaderboards[i].Name);
						if (_showLeaderboardList[i])
						{
							_gameSeed.leaderboards[i].Token = EditorGUILayout.TextField("Token", _gameSeed.leaderboards[i].Token ?? string.Empty, EditorStyles.textField);
							_gameSeed.leaderboards[i].Name = EditorGUILayout.TextField("Name", _gameSeed.leaderboards[i].Name ?? string.Empty, EditorStyles.textField);
							_gameSeed.leaderboards[i].Key = EditorGUILayout.TextField("Description", _gameSeed.leaderboards[i].Key ?? string.Empty, EditorStyles.textField);
							_gameSeed.leaderboards[i].ActorType = (ActorType)EditorGUILayout.EnumPopup("ActorType", _gameSeed.leaderboards[i].ActorType, EditorStyles.popup);
							_gameSeed.leaderboards[i].EvaluationDataType = (EvaluationDataType)EditorGUILayout.EnumPopup("EvaluationDataType", _gameSeed.leaderboards[i].EvaluationDataType, EditorStyles.popup);
							_gameSeed.leaderboards[i].CriteriaScope = (CriteriaScope)EditorGUILayout.EnumPopup("CriteriaScope", _gameSeed.leaderboards[i].CriteriaScope, EditorStyles.popup);
							_gameSeed.leaderboards[i].LeaderboardType = (LeaderboardType)EditorGUILayout.EnumPopup("LeaderboardType", _gameSeed.leaderboards[i].LeaderboardType, EditorStyles.popup);
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space((EditorGUI.indentLevel - 1) * 35);
							if (GUILayout.Button("Remove Leaderboard", GUILayout.ExpandWidth(false)))
							{
								var list = _gameSeed.leaderboards.ToList();
								list.RemoveAt(i);
								_showLeaderboardList.RemoveAt(i);
								_gameSeed.leaderboards = list.ToArray();
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUI.indentLevel--;
						EditorGUILayout.EndVertical();
					}
					if (GUILayout.Button("Add Leaderboard", GUILayout.ExpandWidth(false)))
					{
						var list = _gameSeed.leaderboards.ToList();
						list.Add(new LeaderboardRequest());
						_showLeaderboardList.Add(false);
						_gameSeed.leaderboards = list.ToArray();
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginHorizontal();
				_gameSeedSaveText = (TextAsset)EditorGUILayout.ObjectField("Save File", _gameSeedSaveText, typeof(TextAsset), false);
				if (_gameSeedSaveText != null)
				{
					if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
					{
						string message;
						try
						{
							var seedJson = JsonConvert.SerializeObject(_gameSeed, Formatting.Indented, new StringEnumConverter());
							var seedPath = AssetDatabase.GetAssetPath(_gameSeedSaveText);
							File.WriteAllText(seedPath, seedJson);
							message = $"Success! \n\nSeed Saved to: \n{seedPath}";
						}
						catch (Exception ex)
						{
							message = "Failed: Could not save seed to file. " + ex.Message;
							Debug.LogError($"Edit Game Seed Save: {message}");
						}

						EditorUtility.DisplayDialog("Edit Game Seed", message, "OK");
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				if (GUILayout.Button("Create Game Seed", GUILayout.ExpandWidth(false)))
				{
					_gameSeed = new GameSeed
					{
						achievements = new EvaluationCreateRequest[0],
						leaderboards = new LeaderboardRequest[0]
					};
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
			foreach (var achieve in _gameSeed.achievements)
			{
				_showAchievementList.Add(false);
				_showAchievementCriteria.Add(false);
				_showAchievementReward.Add(false);
				_showAchievementCriteriaList.Add(new List<bool>());
				_showAchievementRewardList.Add(new List<bool>());
				if (achieve.EvaluationCriterias == null)
				{
					achieve.EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>();
				}
				foreach (var criteria in achieve.EvaluationCriterias)
				{
					_showAchievementCriteriaList[currentAchieve].Add(false);
				}
				if (achieve.Rewards == null)
				{
					achieve.Rewards = new List<RewardCreateRequest>();
				}
				foreach (var reward in achieve.Rewards)
				{
					_showAchievementRewardList[currentAchieve].Add(false);
				}
				currentAchieve++;
			}
			_showLeaderboardList.Clear();
			foreach (var leader in _gameSeed.leaderboards)
			{
				_showLeaderboardList.Add(false);
			}
		}
	}
}