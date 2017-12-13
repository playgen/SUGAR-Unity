using UnityEditor;
using UnityEngine;

namespace PlayGen.SUGAR.Unity.Editor
{
	[InitializeOnLoad]
	public static class SetEditorAutoLogin
	{
		private static bool _accountSet;

		static SetEditorAutoLogin()
		{
			EditorApplication.update += Update;
		}

		private static void Update()
		{
			if (SUGARManager.client != null && SUGARManager.account != null && !_accountSet)
			{
				SUGARManager.account.autoLoginSourcePassRequired = !EditorPrefs.HasKey("AutoLoginSourcePassRequired") || EditorPrefs.GetBool("AutoLoginSourcePassRequired");
				SUGARManager.account.autoLoginAuto = !EditorPrefs.HasKey("AutoLoginAuto") || EditorPrefs.GetBool("AutoLoginAuto");
				SUGARManager.account.autoLoginUsername = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginUsername") : string.Empty;
				SUGARManager.account.autoLoginGroup = EditorPrefs.HasKey("AutoLoginGroup") ? EditorPrefs.GetString("AutoLoginGroup") : string.Empty;
				SUGARManager.account.autoLoginPassword = EditorPrefs.HasKey("AutoLoginPassword") ? EditorPrefs.GetString("AutoLoginPassword") : string.Empty;
				SUGARManager.account.autoLoginSourceToken = EditorPrefs.HasKey("AutoLoginSourceToken") ? EditorPrefs.GetString("AutoLoginSourceToken") : string.Empty;
				SUGARManager.account.autoLoginCustomArgs = EditorPrefs.HasKey("AutoLoginCustomArgs") ? EditorPrefs.GetString("AutoLoginCustomArgs") : string.Empty;
				if (!EditorPrefs.HasKey("AutoLoginUsername") || string.IsNullOrEmpty(EditorPrefs.GetString("AutoLoginUsername")))
				{
					Debug.LogError("Auto Log-in Tool Error: Username not provided");
				}
				if (!EditorPrefs.HasKey("AutoLoginSourceToken") || string.IsNullOrEmpty(EditorPrefs.GetString("AutoLoginSourceToken")))
				{
					Debug.LogError("Auto Log-in Tool Error: Source token not provided");
				}
				if (SUGARManager.account.autoLoginSourcePassRequired && (!EditorPrefs.HasKey("AutoLoginPassword") || string.IsNullOrEmpty(EditorPrefs.GetString("AutoLoginPassword"))))
				{
					Debug.LogError("Auto Log-in Tool Error: Password not provided. If no password is needed, 'password required' setting needs to be set to false");
				}
				if (SUGARManager.account.autoLoginSourcePassRequired)
				{
					SUGARManager.account.options = CommandLineUtility.ParseArgs(new[] { "-u" + SUGARManager.account.autoLoginUsername, "-p" + SUGARManager.account.autoLoginPassword, "-s" + SUGARManager.account.autoLoginSourceToken, SUGARManager.account.autoLoginGroup != string.Empty ? "-g" + SUGARManager.account.autoLoginGroup : string.Empty, SUGARManager.account.autoLoginAuto ? "-a" : string.Empty, SUGARManager.account.autoLoginCustomArgs != string.Empty ? "-c" + SUGARManager.account.autoLoginCustomArgs : string.Empty });
				}
				else
				{
					SUGARManager.account.options = CommandLineUtility.ParseArgs(new[] { "-u" + SUGARManager.account.autoLoginUsername, "-s" + SUGARManager.account.autoLoginSourceToken, SUGARManager.account.autoLoginGroup != string.Empty ? "-g" + SUGARManager.account.autoLoginGroup : string.Empty, SUGARManager.account.autoLoginAuto ? "-a" : string.Empty, SUGARManager.account.autoLoginCustomArgs != string.Empty ? "-c" + SUGARManager.account.autoLoginCustomArgs : string.Empty });
				}
				_accountSet = true;
			}
			else if (SUGARManager.client == null && _accountSet)
			{
				_accountSet = false;
			}
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
		private string _username;
		private string _password;
		private string _group;
		private string _source;
		private bool _auto;
		private bool _passRequired;
		private string _customArgs;

		private void OnEnable()
		{
			_passRequired = !EditorPrefs.HasKey("AutoLoginSourcePassRequired") || EditorPrefs.GetBool("AutoLoginSourcePassRequired");
			_auto = !EditorPrefs.HasKey("AutoLoginAuto") || EditorPrefs.GetBool("AutoLoginAuto");
			_username = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginUsername") : string.Empty;
			_password = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginPassword") : string.Empty;
			_group = EditorPrefs.HasKey("AutoLoginGroup") ? EditorPrefs.GetString("AutoLoginGroup") : string.Empty;
			_source = EditorPrefs.HasKey("AutoLoginUsername") ? EditorPrefs.GetString("AutoLoginSourceToken") : string.Empty;
			_customArgs = EditorPrefs.HasKey("AutoLoginCustomArgs") ? EditorPrefs.GetString("AutoLoginCustomArgs") : string.Empty;
		}

		private void OnGUI()
		{
			_username = EditorGUILayout.TextField("Username", _username, EditorStyles.textField);
			_passRequired = EditorGUILayout.Toggle("Password Required", _passRequired, EditorStyles.toggle);
			if (_passRequired)
			{
				_password = EditorGUILayout.PasswordField("Password", _password, EditorStyles.textField);
			}
			_group = EditorGUILayout.TextField("Group Id", _group, EditorStyles.textField);
			_source = EditorGUILayout.TextField("Account Source", _source, EditorStyles.textField);
			_auto = EditorGUILayout.Toggle("Auto Log-in", _auto, EditorStyles.toggle);
			_customArgs = EditorGUILayout.TextField("Custom Args. key=value key=value etc.", _customArgs, EditorStyles.textField);
			if (GUILayout.Button("Save"))
			{
				EditorPrefs.SetBool("AutoLoginSourcePassRequired", _passRequired);
				EditorPrefs.SetBool("AutoLoginAuto", _auto);
				EditorPrefs.SetString("AutoLoginUsername", _username);
				EditorPrefs.SetString("AutoLoginPassword", _password);
				EditorPrefs.SetString("AutoLoginGroup", _group);
				EditorPrefs.SetString("AutoLoginSourceToken", _source);
				EditorPrefs.SetString("AutoLoginCustomArgs", _customArgs);
				Close();
			}
		}
	}
}
