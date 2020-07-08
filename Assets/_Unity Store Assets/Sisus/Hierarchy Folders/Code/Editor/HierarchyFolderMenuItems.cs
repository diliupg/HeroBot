// PreferencesApplier will make changes to this region based on preferences
#region ApplyPreferences
#define ENABLE_HIERARCHY_FOLDER_MENU_ITEMS
#endregion

using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	public static class HierarchyFolderMenuItems
	{
		private static readonly SortTransformsByHierarchyOrder SortByHierarchyOrder = new SortTransformsByHierarchyOrder();

		private static bool alreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame;

		/// <summary>
		/// This is initialized on load due to the usage of the InitializeOnLoad attribute.
		/// </summary>
		static HierarchyFolderMenuItems()
		{
			EditorApplication.delayCall += ApplyPreferencesWhenAssetDatabaseReady;
		}

		private static void ApplyPreferencesWhenAssetDatabaseReady()
		{
			if(!PreferencesApplier.ReadyToApplyPreferences())
			{
				EditorApplication.delayCall += ApplyPreferencesWhenAssetDatabaseReady;
				return;
			}

			var classType = typeof(HierarchyFolderMenuItems);
			var preferences = HierarchyFolderPreferences.Get();
			bool enabled = preferences.enableMenuItems;

			PreferencesApplier.ApplyPreferences(classType,
			new[] { "#define ENABLE_HIERARCHY_FOLDER_MENU_ITEMS" },
			new[] { enabled });

			preferences.onPreferencesChanged += (changedPreferences) =>
			{
				if(changedPreferences.enableMenuItems != enabled)
				{
					var script = PreferencesApplier.FindScriptFile(classType);
					if(script != null)
					{
						AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(script));
					}
					#if DEV_MODE
					else { Debug.LogWarning("Could not find script asset "+classType.Name+".cs"); }
					#endif
				}
			};
		}

		internal static void CreateHierarchyFolder()
		{
			if(Selection.transforms.Length > 1)
			{
				CreateHierarchyFolderParent();
			}
			else
			{
				CreateHierarchyFolderSibling();
			}
		}

		#if ENABLE_HIERARCHY_FOLDER_MENU_ITEMS
		[UsedImplicitly, MenuItem("GameObject/Hierarchy Folder %#g", false, -51)]
		#endif
		private static void CreateHierarchyFolderFromMainMenuOrContextMenu(MenuCommand command)
		{
			if(alreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame)
			{
				return;
			}

			// If more than one GameObjects are selected in the hierarchy, create a new folder and move all selected GameObjects under it.
			if(Selection.transforms.Length > 1)
			{
				var rightClickedGameObject = command.context as GameObject;
				if(rightClickedGameObject != null)
				{
					alreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame = true;
					EditorApplication.delayCall += ResetAlreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame;
				}

				CreateHierarchyFolderParent();
			}
			else
			{
				// If creating HierarchyFolder from context menu, add it as child of right-clicked GameObject.
				// This is how most existing context menu items in Unity function.
				var rightClickedGameObject = command.context as GameObject;
				if(rightClickedGameObject != null)
				{
					CreateHierarchyFolderChild(rightClickedGameObject.transform);
				}
				else
				{
					CreateHierarchyFolderSibling();
				}
			}
		}

		private static void ResetAlreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame()
		{
			alreadyCreatedHierarchyFolderFromContextMenuForMultipleTargetsThisFrame = false;
		}

		internal static void CreateHierarchyFolderChild([CanBeNull]Transform parent)
		{
			var folder = CreateHierarchyFolderInternal(parent is RectTransform);

			if(parent != null)
			{
				folder.transform.UndoableSetParent(parent, "Hierarchy Folder");
			}

			Undo.RegisterCreatedObjectUndo(folder, "Hierarchy Folder");

			Selection.activeGameObject = folder;

			var hierarchyWindow = HierarchyWindowUtility.GetHierarchyWindow();
			if(hierarchyWindow != null)
			{
				hierarchyWindow.Focus();
			}
		}

		internal static void CreateHierarchyFolderParent()
		{
			int count = Selection.transforms.Length;

			var members = new Transform[count];
			Array.Copy(Selection.transforms, 0, members, 0, count);
			Array.Sort(members, SortByHierarchyOrder);

			var firstMember = members[0];
			var hierarchyFolderParent = firstMember.parent;

			var folder = CreateHierarchyFolderInternal(firstMember is RectTransform);

			// if not all selected have the same parent, then create folders as last item in hierarchy
			for(int n = 1; n < count; n++)
			{
				if(members[n].parent != hierarchyFolderParent)
				{
					hierarchyFolderParent = null;
					break;
				}
			}
			
			if(hierarchyFolderParent != null)
			{
				folder.transform.UndoableSetParent(hierarchyFolderParent, "Hierarchy Folder Parent");
			}
			int hierarchyFolderSiblingIndex = firstMember.GetSiblingIndex();
			folder.transform.SetSiblingIndex(hierarchyFolderSiblingIndex);

			Undo.RegisterCreatedObjectUndo(folder, "Hierarchy Folder Parent");

			#if UNITY_EDITOR
			if(EditorApplication.isPlayingOrWillChangePlaymode && HierarchyFolderPreferences.Get().playModeBehaviour == StrippingType.FlattenHierarchy)
			{
				int moveToIndex = HierarchyFolderUtility.GetLastChildIndexInFlatMode(folder);
				for(int n = count - 1; n >= 0; n--)
				{
					Undo.SetTransformParent(members[n], hierarchyFolderParent, "Hierarchy Folder Parent");
					members[n].SetSiblingIndex(moveToIndex);
				}
				return;
			}
			#endif

			for(int n = 0; n < count; n++)
			{
				Undo.SetTransformParent(members[n], folder.transform, "Hierarchy Folder Parent");
				members[n].SetAsLastSibling();
			}
			
			Selection.activeGameObject = folder;
		}

		internal static void CreateHierarchyFolderSibling()
		{
			var selected = Selection.activeTransform;

			var folder = CreateHierarchyFolderInternal(selected is RectTransform);

			if(selected != null)
			{
				int moveToIndex = selected.GetSiblingIndex();
				folder.transform.UndoableSetParent(selected.parent, "Hierarchy Folder");
				folder.transform.SetSiblingIndex(moveToIndex);
			}

			Undo.RegisterCreatedObjectUndo(folder, "Hierarchy Folder");

			Selection.activeGameObject = folder;

			var hierarchyWindow = HierarchyWindowUtility.GetHierarchyWindow();
			if(hierarchyWindow != null)
			{
				hierarchyWindow.Focus();
			}
		}

		#if ENABLE_HIERARCHY_FOLDER_MENU_ITEMS
		[UsedImplicitly, MenuItem("Tools/Hierarchy/Convert Selected to Hierarchy Folders %&#g", false, -31)]
		#endif
		private static void ConvertToHierarchyFolder()
		{
			var selectedTransforms = Selection.transforms;
			int selectedCount = selectedTransforms.Length;
			var transforms = new Transform[selectedCount];
			Array.Copy(selectedTransforms, 0, transforms, 0, selectedCount);
			Array.Sort(transforms, SortByHierarchyOrder);
			for(int n = transforms.Length - 1; n >= 0; n--)
			{
				var transform = transforms[n];
				var gameObject = transform.gameObject;
				
				// Skip GameObjects that already are hierarchy folders
				if(gameObject.IsHierarchyFolder())
				{
					continue;
				}


				Undo.RegisterFullObjectHierarchyUndo(gameObject, "Convert To Hierarchy Folder");

				var components = gameObject.GetComponents<Component>();
				
				// if target has no components besides the Transform component, then converting them is
				// as simple as adding the HierarchyFolder component to them - the component will handle the rest.
				if(components.Length == 1)
				{
					Undo.AddComponent<HierarchyFolder>(gameObject);
				}
				// If the target has supernumerary components, then we can't convert it into a hierarchy folder directly.
				// Instead we will create a new hierarchy folder parent for it, and move all the children of the target
				// underneath the folder.
				else
				{
					#if UNITY_2018_3_OR_NEWER
					if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
					#else
					if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
					#endif
					{
						Debug.LogWarning("Unpacking prefab root so can move children under new hierarchy folder.");
						#if UNITY_2018_3_OR_NEWER
						PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
						#else
						PrefabUtility.DisconnectPrefabInstance(gameObject);
						#endif
					}

					var folder = CreateHierarchyFolderInternal(transform is RectTransform);
					folder.name = gameObject.name;
					Undo.RegisterCreatedObjectUndo(folder, "Convert To Hierarchy Folder");

					var folderTransform = folder.transform;
					folderTransform.UndoableSetParent(transform.parent, "Convert To Hierarchy Folder");
					folderTransform.SetSiblingIndex(transform.GetSiblingIndex());

					transform.UndoableSetParent(folderTransform, "Convert To Hierarchy Folder");

					// Don't unparent children of a Canvas
					if(gameObject.GetComponent<Canvas>() != null)
					{
						continue;
					}

					int childCount = transform.childCount;
					if(childCount > 0)
					{
						for(int c = 0; c < childCount; c++)
						{
							var child = transform.GetChild(0);
							child.UndoableSetParent(folderTransform, "Convert To Hierarchy Folder");
						}
					}
				}
			}
		}

		#if ENABLE_HIERARCHY_FOLDER_MENU_ITEMS
		[UsedImplicitly, MenuItem("Tools/Hierarchy/Convert Scene Root to Hierarchy Folders", false, -30)]
		#endif
		private static void ConvertAllEmptyRootGameObjectsToHierarchyFolder()
		{
			bool askAboutPrefabs = true;

			for(int s = UnityEngine.SceneManagement.SceneManager.sceneCount - 1; s >= 0; s--)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(s);
				var root = scene.GetRootGameObjects();

				for(int n = root.Length - 1; n >= 0; n--)
				{
					var gameObject = root[n];
				
					// Skip GameObjects that already are hierarchy folders
					if(gameObject.IsHierarchyFolder())
					{
						continue;
					}

					Undo.RegisterFullObjectHierarchyUndo(gameObject, "Convert To Hierarchy Folder");

					var components = gameObject.GetComponents<Component>();
				
					// if target has no components besides the Transform component, then converting them is
					// as simple as adding the HierarchyFolder component to them - the component will handle the rest.
					if(components.Length == 1)
					{
						Undo.AddComponent<HierarchyFolder>(gameObject);
					}
					// If the target has supernumerary components, then we can't convert it into a hierarchy folder directly.
					// Instead we will create a new hierarchy folder parent for it, and move all the children of the target
					// underneath the folder.
					else
					{
						#if UNITY_2018_3_OR_NEWER
						if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
						#else
						if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
						#endif
						{
							if(askAboutPrefabs)
							{
								bool skip;
								switch(EditorUtility.DisplayDialogComplex("Prefab Instance Detected", "Unpack prefab instance " + gameObject.name + "?\n\nThis is necessary in order to convert it into a hierarchy folder.", "Unpack", "Unpack All In Scene", "Skip"))
								{
									case 0:
										skip = false;
										break;
									case 1:
										skip = false;
										askAboutPrefabs = false;
										break;
									default:
										skip = true;
										break;
								}
								if(skip)
								{
									continue;
								}
							}

							Debug.LogWarning("Unpacking prefab root so can move children under new hierarchy folder.");
							#if UNITY_2018_3_OR_NEWER
							PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
							#else
							PrefabUtility.DisconnectPrefabInstance(gameObject);
							#endif
						}

						var folder = CreateHierarchyFolderInternal(gameObject.transform is RectTransform);
						folder.name = gameObject.name;
						Undo.RegisterCreatedObjectUndo(folder, "Convert To Hierarchy Folder");

						var folderTransform = folder.transform;
						var transform = gameObject.transform;
						folderTransform.UndoableSetParent(transform.parent, "Convert To Hierarchy Folder");
						folderTransform.SetSiblingIndex(transform.GetSiblingIndex());

						transform.UndoableSetParent(folderTransform, "Convert To Hierarchy Folder");

						// Don't unparent children of a Canvas
						if(gameObject.GetComponent<Canvas>() != null)
						{
							continue;
						}

						int childCount = transform.childCount;
						if(childCount > 0)
						{
							for(int c = 0; c < childCount; c++)
							{
								var child = transform.GetChild(0);
								child.UndoableSetParent(folderTransform, "Convert To Hierarchy Folder");
							}
						}
					}
				}
			}
		}

		private static GameObject CreateHierarchyFolderInternal(bool useRectTransform)
		{
			string name = HierarchyFolderPreferences.Get().defaultName;
			var folder = useRectTransform ? new GameObject(name, typeof(RectTransform), typeof(HierarchyFolder)) : new GameObject(name, typeof(HierarchyFolder));
			return folder;
		}

		[UsedImplicitly, MenuItem("Tools/Hierarchy/Convert Selected to Hierarchy Folders %&#g", true)]
		internal static bool ShouldDisplayConvertToHierarchyFolder()
		{
			for(int n = Selection.transforms.Length - 1; n >= 0; n--)
			{
				if(!Selection.transforms[n].gameObject.IsHierarchyFolder())
				{
					return true;
				}
			}
			return false;
		}

		#if DEV_MODE
		[UsedImplicitly, MenuItem("Tools/Reveal Hidden Components")]
		private static void RevealHiddenComponents()
		{
			foreach(var gameObject in Selection.gameObjects)
			{
				foreach(var component in gameObject.GetComponents<Component>())
				{
					component.hideFlags = HideFlags.None;
				}
			}
		}
		#endif
	}
}