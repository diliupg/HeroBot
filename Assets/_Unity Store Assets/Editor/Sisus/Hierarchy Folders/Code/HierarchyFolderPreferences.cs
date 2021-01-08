﻿#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Diagnostics;

namespace Sisus.HierarchyFolders
{
	/// <summary>
	/// Contains user preferences for Hierarchy Folders and handles saving and loading them to EditorPrefs.
	/// </summary>
	[Serializable]
	public class HierarchyFolderPreferences : ScriptableObject
	{
		private const string EditorPrefsKey = "HierarchyFolderPreferences";

		private static HierarchyFolderPreferences instance;

		public Action<HierarchyFolderPreferences> onPreferencesChanged;

		[Header("Naming")]
		public bool enableNamingRules = true;
		public string defaultName = "--- New Folder ---";
		[Tooltip("If true then default GameObject names will be replaced with HierarchyFolderPreferences.defaultName when the HierarchyFolder component is added to an existing GameObject.")]
		public bool autoNameOnAdd = true;
		public string namePrefix = "--- ";
		public string nameSuffix = " ---";
		public bool forceNamesUpperCase = false;

		[Header("Build Stripping")]
		[Tooltip("If true then all hierarchy folders will be removed from all scenes during build post processing.\n\nAny child GameObjects will be moved upwards the parent chain.")]
		public bool removeFromBuild = true;
		public bool warnWhenNotRemovedFromBuild = true;

		[Header("Play Mode Stripping")]
		[Tooltip("If true then hierarchy folders will be removed from loaded scenes when their Awake method is called.\n\nAny members of the HierarchyGroup will be moved upwards the parent chain.")]
		public StrippingType playModeBehaviour = StrippingType.None;
		[Tooltip("Entire Scene Immediate : All Hierarchy Folders are stripped at the very beginning the scene loading process. Hierarchy order is preserved.\n\nIndividuallyDuringAwake: Hierarchy Folders are stripped in the order that Awake is called for them. Hierarchy order is not preserved.\n\nAll Hierarchy Folders are stripped once scene has fully finished loading and all scene scene objects have been initialized. Hierarchy order is preserved.\n\nIf you encounter ArgumentException: The scene is not loaded then switch to using a stripping method other than Entire Scene Immediate.")]
		public PlayModeStrippingMethod playModeStrippingMethod = PlayModeStrippingMethod.EntireSceneWhenLoaded;

		[Header("Drawer")]
		[TextArea(2, 8)]
		public string infoBoxText = "This is a hierarchy folder and can be used for organizing objects in the hierarchy.\n\nWhen a build is being made all members will be moved up the parent chain and the folder itself will be removed.";

		[Header("Hierarchy View")]
		[Tooltip("Enable folder icons in the hierarchy view.")]
		public bool enableHierarchyIcons = true;
		public bool doubleClickSelectsChildrens = true;

		[Header("Editor Integration")]
		[Tooltip("Enable menu items GameObject > Hierarchy Folder and GameObject > Hierarchy Folder Parent.")]
		public bool enableMenuItems = true;
		public bool enableToolbarIcon = true;

		[Header("Scripting")]
		[Tooltip("Should extension methods such as GameObject.IsHierarchyFolder and Transform.GetParent always be shown, or only when you add \"using Sisus.HierarchyFolders;\" at the top of your class?\n\nNOTE: you will also need an Assembly Definition File with a reference to Sisus.HierarchyFolders for the methods to show up.")]
		public bool extensionMethodsInGlobalNamespace = true;

		[Header("Icons")]
		public Icon modernLight = new Icon();
		public Icon modernDark = new Icon();
		public Icon classicLight = new Icon();
		public Icon classicDark = new Icon();

		[SerializeField, HideInInspector]
		public string previousNamePrefix = "--- ";
		[SerializeField, HideInInspector]
		public string previousNameSuffix = " ---";

		[SerializeField, HideInInspector]
		private bool defaultIconsFetched;

		public static bool FlattenHierarchy
		{
			get
			{
#if UNITY_EDITOR
				return EditorApplication.isPlayingOrWillChangePlaymode && Get().playModeBehaviour == StrippingType.FlattenHierarchy;
#else
				return false;
#endif
			}
		}

		public static HierarchyFolderPreferences Get()
		{
			if(instance == null)
			{
				var assetPath = GetProjectSettingsAssetPath(false);
				var asset = AssetDatabase.LoadAssetAtPath<HierarchyFolderPreferences>(assetPath);
				if(asset != null)
				{
					instance = Instantiate(asset);
				}
				else
				{
					instance = CreateInstance<HierarchyFolderPreferences>();
					instance.LoadStateFromEditorPrefs();
				}
			}
			return instance;
		}

		[NotNull]
		public Icon Icon()
		{
#if UNITY_2019_3_OR_NEWER
			var icon =  EditorGUIUtility.isProSkin ? modernDark : modernLight;
#else
			var icon =  EditorGUIUtility.isProSkin ? classicDark : classicLight;
#endif

			if(icon.closed == null)
			{
				icon = GetDefaultIcon();
			}

			return icon;
		}

		[NotNull]
		public Icon GetDefaultIcon(int size = 16)
		{
			var iconSizeWas = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(size, size));
			var icon = new Icon();
			icon.open = EditorGUIUtility.IconContent("Folder Icon").image;
			icon.closed = icon.open;
			EditorGUIUtility.SetIconSize(iconSizeWas);
			return icon;
		}

		private void OnEnable()
		{
			if(defaultIconsFetched)
			{
				return;
			}

			var defaultIconGuids = AssetDatabase.FindAssets("hierarchy-folder-icon-");
			for(int n = defaultIconGuids.Length - 1; n >= 0; n--)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(defaultIconGuids[n]);
				switch(Path.GetFileNameWithoutExtension(assetPath))
				{
					case "hierarchy-folder-icon-modern-dark-closed":
						modernDark.closed = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
						break;
					case "hierarchy-folder-icon-modern-dark-open":
						modernDark.open = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
						break;
					case "hierarchy-folder-icon-modern-light-closed":
						modernLight.closed = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
						break;
					case "hierarchy-folder-icon-modern-light-open":
						modernLight.open = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
						break;
				}
			}

			defaultIconsFetched = true;

			EditorUtility.SetDirty(this);
		}

		public void ResetToDefaults()
		{
			var newInstance = CreateInstance<HierarchyFolderPreferences>();
			string serializedState = EditorJsonUtility.ToJson(newInstance);
			EditorJsonUtility.FromJsonOverwrite(serializedState, this);

			ClearSavedState();

			if(onPreferencesChanged != null)
			{
				onPreferencesChanged(this);
			}
		}

		public void SaveState()
		{
			if(IsDefaultState())
			{
				ClearSavedState();
			}
			else
			{
				if(forceNamesUpperCase)
				{
					defaultName = defaultName.ToUpper();
				}

				if(!defaultName.StartsWith(namePrefix, StringComparison.Ordinal))
				{
					for(int c = namePrefix.Length - 1; c >= 0 && !defaultName.StartsWith(namePrefix, StringComparison.Ordinal); c--)
					{
						defaultName = namePrefix[c] + defaultName;
					}
				}

				if(!defaultName.EndsWith(nameSuffix, StringComparison.Ordinal))
				{
					for(int c = 0, count = nameSuffix.Length; c < count && !defaultName.EndsWith(nameSuffix, StringComparison.Ordinal); c++)
					{
						defaultName += nameSuffix[c];
					}
				}

				string serializedState = EditorJsonUtility.ToJson(this);
				EditorPrefs.SetString(EditorPrefsKey, serializedState);

				var assetPath = GetProjectSettingsAssetPath(true);
				AssetDatabase.CreateAsset(Instantiate(this), assetPath);
				AssetDatabase.SaveAssets();
			}

			if(onPreferencesChanged != null)
			{
				onPreferencesChanged(this);
			}
		}

		public void DiscardChanges()
		{
			instance = null;
			Get();
		}

		public void LoadStateFromEditorPrefs()
		{
			if(EditorPrefs.HasKey(EditorPrefsKey))
			{
				string serializedState = EditorPrefs.GetString(EditorPrefsKey);
				EditorJsonUtility.FromJsonOverwrite(serializedState, this);
			}
		}

		public bool IsDefaultState()
		{
			var newInstance = CreateInstance<HierarchyFolderPreferences>();

			var fields = GetType().GetFields();
			foreach(var field in fields)
			{
				if(string.Equals(field.Name, "onPreferencesChanged"))
				{
					continue;
				}

				var currentValue = field.GetValue(this);
				var defaultValue = field.GetValue(newInstance);
				if(currentValue == null)
				{
					if(defaultValue == null)
					{
						return false;
					}
				}
				else if(!currentValue.Equals(defaultValue))
				{
					return false;
				}
			}
			return true;
		}

		public static void ClearSavedState()
		{
			var assetPath = GetProjectSettingsAssetPath(false);
			AssetDatabase.DeleteAsset(assetPath);

			EditorPrefs.DeleteKey(EditorPrefsKey);
		}

		/// <summary>
		/// Gets local asset path to project settings
		/// </summary>
		/// <returns> "Assets/Sisus/Hierarchy Folders/Project Settings/HierarchyFoldersSettings.asset" </returns>
		private static string GetProjectSettingsAssetPath(bool createFolderIfMissing)
		{
			var path = new StackTrace(true).GetFrame(0).GetFileName();
			path = Path.GetDirectoryName(path);
			path = Path.GetDirectoryName(path);
			path = Path.Combine(path, "Project Settings");

			if(createFolderIfMissing && !Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			path = Path.Combine(path, "HierarchyFoldersSettings.asset");

			path = "Assets" + path.Substring(Application.dataPath.Length);

			return path;
		}
	}
}
#endif