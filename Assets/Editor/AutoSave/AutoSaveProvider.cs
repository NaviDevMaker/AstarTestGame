#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AutoSave
{
	[InitializeOnLoad]
	public class AutoSaveProvider
	{
		static double nextTime = 0;

		/**
		 * AutoSaveProvider
		 */
		static AutoSaveProvider()
		{
			// Timer
			nextTime = EditorApplication.timeSinceStartup + m_Interval * 60;
			EditorApplication.update += () =>
			{
				if (nextTime < EditorApplication.timeSinceStartup)
				{
					nextTime = EditorApplication.timeSinceStartup + m_Interval * 60;

					if (m_IsAutoSave && !EditorApplication.isPlaying)
					{
						EditorSceneManager.SaveOpenScenes();
						Backup();
						Debug.Log("SaveScene : " + DateTime.Now);
					}
				}
			};
		}

		//! IsAutoSave Flag
		static readonly string c_AUTO_SAVE_VALUE = "AutoSave";
		static bool m_IsAutoSave
		{
			get
			{
				string value = EditorUserSettings.GetConfigValue(c_AUTO_SAVE_VALUE);
				return !string.IsNullOrEmpty(value) && value.Equals("True");
			}
			set
			{
				EditorUserSettings.SetConfigValue(c_AUTO_SAVE_VALUE, value.ToString());
			}
		}

		//! Interval Time
		static readonly string c_INTERVAL_VALUE = "Interval";
		static readonly int c_INTERVAL_DEFAULT = 1;
		static int m_Interval
		{
			get
			{
				string value = EditorUserSettings.GetConfigValue(c_INTERVAL_VALUE);
				if (value == null) value = c_INTERVAL_DEFAULT.ToString();
				return int.Parse(value);
			}
			set
			{
				if (value < c_INTERVAL_DEFAULT) value = c_INTERVAL_DEFAULT;
				EditorUserSettings.SetConfigValue(c_INTERVAL_VALUE, value.ToString());
			}
		}

		//! History Quantity
		static readonly string c_QUANTITY_VALUE = "Quantity";
		static readonly int c_QUANTITY_DEFAULT = 1;
		static int m_Quantity
		{
			get
			{
				string value = EditorUserSettings.GetConfigValue(c_QUANTITY_VALUE);
				if (value == null) value = c_QUANTITY_DEFAULT.ToString();
				return int.Parse(value);
			}
			set
			{
				if (value < c_QUANTITY_DEFAULT) value = c_QUANTITY_DEFAULT;
				EditorUserSettings.SetConfigValue(c_QUANTITY_VALUE, value.ToString());
			}
		}

		//! AutoSave
		static readonly string c_AUTO_SAVE_TITLE = "Auto Save";
		static readonly string c_SETTING_PATH = "Preferences/" + c_AUTO_SAVE_TITLE;
		static readonly string[] c_SEARCH_WORDs = { c_AUTO_SAVE_TITLE, "Auto", "Save", "AutoSave", "Scene" };

		/**
         * @fn			CreateCustomProvider
         * @brief		Preferences(SettingsScope.User)のc_SETTING_PATHに表示させる
         * @return		SettingsProvider
         */
		[SettingsProvider]
		public static SettingsProvider CreateCustomProvider()
		{
			SettingsProvider tmpProvider = new SettingsProvider(c_SETTING_PATH, SettingsScope.User)
			{
				/** Title */
				label = c_AUTO_SAVE_TITLE,

				/** Menu */
				guiHandler = searchContext =>
				{
					/** Auto Save */
					m_IsAutoSave = EditorGUILayout.BeginToggleGroup("Auto Save", m_IsAutoSave);   // Group-AutoSave

					EditorGUILayout.Space();    // Space

					/** Timer */
					EditorGUILayout.LabelField("<Save Scene Interval>");
					m_Interval = EditorGUILayout.IntField("Interval(min) 1～", m_Interval);

					EditorGUILayout.Space();    // Space

					/** History Quantity */
					EditorGUILayout.LabelField("<History Quantity>");
					m_Quantity = EditorGUILayout.IntField("Max History 1～", m_Quantity);

					EditorGUILayout.EndToggleGroup();   // Group-AutoSave-End
				},

				/** Search Key Word */
				keywords = new HashSet<string>(c_SEARCH_WORDs)
			};

			return tmpProvider;
		}

		/**
         * @fn		Backup
         * @brief	バックアップを保存する
         */
		[MenuItem("File/Backup/StartBackup")]
		public static void Backup()
		{
			string exportPath = "_Backup/" + Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path) + "_" + DateTime.Now.ToString("MMddHHmm") + Path.GetExtension(EditorSceneManager.GetActiveScene().path);

			Directory.CreateDirectory(Path.GetDirectoryName(exportPath));

			if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path)) { return; }

			byte[] data = File.ReadAllBytes(EditorSceneManager.GetActiveScene().path);
			File.WriteAllBytes(exportPath, data);

			string[] files = Directory.GetFiles(Path.GetDirectoryName(exportPath));
			if (files.Length <= m_Quantity) { return; }

			for (int i = 0; i < files.Length - m_Quantity; i++)
			{
				File.Delete(files[i]);
			}
		}
	}
}
#endif