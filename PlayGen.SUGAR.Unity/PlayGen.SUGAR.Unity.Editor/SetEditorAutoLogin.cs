using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlayGen.SUGAR.Unity.Editor
{
	[InitializeOnLoad]
	public static class SetEditorAutoLogin
	{
		public class AutoLoginOption
		{
			public string Label;
			public string Key;
			public string SugarRefName;
			public bool Required;
			public string AutoLoginPrefix;
			/// <summary>
			/// depends on value name must be the name of a boolean
			/// </summary>
			public string DependsOnValue;
		}

		public class StringValue : AutoLoginOption
		{
			public string Value;
			public bool Hidden;

			public StringValue(string label, string key, string sugarRefName, string autoLoginPrefix, string dependsOnValue = "", bool required = false, bool hidden = false, string value = "")
			{
				Label = label;
				Key = key;
				AutoLoginPrefix = autoLoginPrefix;
				SugarRefName = sugarRefName;
				Required = required;
				Value = value;
				Hidden = hidden;
				DependsOnValue = dependsOnValue;
			}

			public StringValue(string value)
			{
				Value = value;
			}
		}

		public class BoolValue : AutoLoginOption
		{
			public bool Value;

			public BoolValue(string label, string key, string sugarRefName, string autoLoginPrefix, bool required = false, bool value = false)
			{
				Label = label;
				Key = key;
				AutoLoginPrefix = autoLoginPrefix;
				SugarRefName = sugarRefName;
				Required = required;
				Value = value;
			}

			public BoolValue(bool value)
			{
				Value = value;
			}
		}

		public static List<AutoLoginOption> AutoLoginOptions = new List<AutoLoginOption> {
			new StringValue("Username", "AutoLoginUsername", "autoLoginUsername", "-u", required:true),
			new BoolValue("Password Required", "AutoLoginSourcePassRequired", "autoLoginSourcePassRequired", ""),
			new StringValue("Password", "AutoLoginPassword", "autoLoginPassword", "-p", hidden:true, dependsOnValue:"AutoLoginSourcePassRequired"),
			new StringValue("Group ID", "AutoLoginGroup", "autoLoginGroup", "-g"),
			new StringValue("Source Token", "AutoLoginSourceToken", "autoLoginSourceToken", "-s", required:true),
			new BoolValue("Auto Login", "AutoLoginAuto", "autoLoginAuto", "-a"),
			new StringValue("Custom Args", "AutoLoginCustomArgs", "autoLoginCustomArgs", "-c")
		};

		private static bool _accountSet;

		static SetEditorAutoLogin()
		{
			EditorApplication.update += Update;
		}
		private static bool UseAutoLogin()
		{
			var autoLogin = AutoLoginOptions.Find(l => l.Label == "Auto Login");
			var boolValue = (BoolValue)autoLogin;
			return boolValue.Value;
		}

		private static void Update()
		{
			if (SUGARManager.client != null && SUGARManager.account != null && !_accountSet && UseAutoLogin())
			{
				var sugarAccount = SUGARManager.account;
				foreach (var autoLoginOption in AutoLoginOptions)
				{
					if (autoLoginOption.GetType() == typeof(BoolValue))
					{
						var boolValue = (BoolValue)autoLoginOption;
						boolValue.Value = !EditorPrefs.HasKey(boolValue.Key) || EditorPrefs.GetBool(boolValue.Key);

						var prop = sugarAccount.GetType().GetField(boolValue.SugarRefName, BindingFlags.Instance | BindingFlags.NonPublic);
						prop?.SetValue(sugarAccount, boolValue.Value);
					}
					if (autoLoginOption.GetType() == typeof(StringValue))
					{
						var stringValue = (StringValue)autoLoginOption;
						stringValue.Value = EditorPrefs.HasKey(stringValue.Key) ? EditorPrefs.GetString(stringValue.Key) : string.Empty;

						var prop = sugarAccount.GetType().GetField(stringValue.SugarRefName, BindingFlags.Instance | BindingFlags.NonPublic);
						prop?.SetValue(sugarAccount, stringValue.Value);

						if (autoLoginOption.Required)
						{
							if (stringValue.Value == string.Empty)
							{
								Debug.LogError($"Auto Log-in Tool Error: {stringValue.Label} not provided");
							}
						}
					}
				}

				var args = new List<string>();
				foreach (var autoLoginOption in AutoLoginOptions)
				{
					if (autoLoginOption.GetType() == typeof(BoolValue))
					{
						var boolValue = (BoolValue)autoLoginOption;

						if (!string.IsNullOrEmpty(boolValue.AutoLoginPrefix) && boolValue.Value)
						{
							args.Add(boolValue.AutoLoginPrefix);
						}
					}
					if (autoLoginOption.GetType() == typeof(StringValue))
					{
						var stringValue = (StringValue) autoLoginOption;
						if (!string.IsNullOrEmpty(stringValue.DependsOnValue))
						{
							// this value is only used if the value it depends on is true
							if (DependentValue(stringValue.DependsOnValue) && !string.IsNullOrEmpty(stringValue.AutoLoginPrefix) && !string.IsNullOrEmpty(stringValue.Value))
							{
								args.Add(stringValue.AutoLoginPrefix + stringValue.Value);
							}
						}
						else if (!string.IsNullOrEmpty(stringValue.AutoLoginPrefix) && !string.IsNullOrEmpty(stringValue.Value))
						{
							args.Add(stringValue.AutoLoginPrefix + stringValue.Value);
						}
					}
				}
				SUGARManager.account.options = CommandLineUtility.ParseArgs(args.ToArray());
				
				_accountSet = true;
			}
			else if (SUGARManager.client == null && _accountSet)
			{
				_accountSet = false;
			}
		}

		public static bool DependentValue(string dependingValueKey)
		{
			if (string.IsNullOrEmpty(dependingValueKey))
				return false;
			var dependingValue = (BoolValue)AutoLoginOptions.First(a => a.Key == dependingValueKey);
			return dependingValue.Value;
		}

		[MenuItem("Tools/SUGAR/Set Auto Log-in Values")]
		public static void SetAutoLogIn()
		{
			var window = ScriptableObject.CreateInstance<AutoLogIn>();
			window.titleContent.text = "Set Auto Log-in Values";
			window.Show();
		}
	}

	public class AutoLogIn : EditorWindow
	{
		private void OnEnable()
		{
			foreach (var autoLoginOption in SetEditorAutoLogin.AutoLoginOptions)
			{
				if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.BoolValue))
				{
					var boolValue = (SetEditorAutoLogin.BoolValue)autoLoginOption;
					boolValue.Value = !EditorPrefs.HasKey(boolValue.Key) || EditorPrefs.GetBool(boolValue.Key);
				}
				if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.StringValue))
				{
					var stringValue = (SetEditorAutoLogin.StringValue)autoLoginOption;
					stringValue.Value = EditorPrefs.HasKey(stringValue.Key) ? EditorPrefs.GetString(stringValue.Key) : string.Empty;
				}
			}
		}

		private void OnGUI()
		{
			foreach (var autoLoginOption in SetEditorAutoLogin.AutoLoginOptions)
			{
				if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.BoolValue))
				{
					var boolValue = (SetEditorAutoLogin.BoolValue)autoLoginOption;
					boolValue.Value = EditorGUILayout.Toggle(boolValue.Label, boolValue.Value, EditorStyles.toggle);
				}
				if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.StringValue))
				{
					var stringValue = (SetEditorAutoLogin.StringValue)autoLoginOption;
					if ((!string.IsNullOrEmpty(stringValue.DependsOnValue) && SetEditorAutoLogin.DependentValue(stringValue.DependsOnValue)) ||
					    string.IsNullOrEmpty(stringValue.DependsOnValue))
					{
						if (stringValue.Hidden)
						{
							stringValue.Value = EditorGUILayout.PasswordField(stringValue.Label, stringValue.Value, EditorStyles.textField);
						}
						else
						{
							stringValue.Value = EditorGUILayout.TextField(stringValue.Label, stringValue.Value, EditorStyles.textField);
						}
					}
				}
			}
			if (GUILayout.Button("Save"))
			{
				foreach (var autoLoginOption in SetEditorAutoLogin.AutoLoginOptions)
				{
					if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.BoolValue))
					{
						var boolValue = (SetEditorAutoLogin.BoolValue)autoLoginOption;
						EditorPrefs.SetBool(boolValue.Key, boolValue.Value);
					}
					if (autoLoginOption.GetType() == typeof(SetEditorAutoLogin.StringValue))
					{
						var stringValue = (SetEditorAutoLogin.StringValue)autoLoginOption;
						EditorPrefs.SetString(stringValue.Key, stringValue.Value);
					}
				}
				Close();
			}
		}
	}
}

