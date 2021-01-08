#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	public class PlayModeStripper
	{
		private static PlayModeStripper instance;

		private readonly HashSet<Scene> playModeStrippingHandledForScenes = new HashSet<Scene>();
		private readonly Dictionary<Scene, HashSet<Transform>> playModeStrippingHandledForSceneRootObjects = new Dictionary<Scene, HashSet<Transform>>(1);
		private readonly StrippingType playModeStripping = StrippingType.None;
		private readonly PlayModeStrippingMethod playModeStrippingMethod = PlayModeStrippingMethod.EntireSceneImmediate;

		public PlayModeStripper(StrippingType setStrippingType, PlayModeStrippingMethod setStrippingMethod)
		{
			playModeStripping = setStrippingType;
			playModeStrippingMethod = setStrippingMethod;

			if(playModeStripping == StrippingType.None)
			{
				return;
			}

			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;

			for(int s = 0, scount = SceneManager.sceneCount; s < scount; s++)
			{
				var scene = SceneManager.GetSceneAt(s);
				if(scene.isLoaded && playModeStrippingHandledForScenes.Add(scene))
				{
					HierarchyFolderUtility.ApplyStrippingType(scene, playModeStripping);
				}
			}
		}

		private static PlayModeStripper Instance()
		{
			if(instance == null)
			{
				if(!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					instance = new PlayModeStripper(StrippingType.None, default(PlayModeStrippingMethod));
				}
				else
				{
					var preferences = HierarchyFolderPreferences.Get();
					instance = new PlayModeStripper(preferences.playModeBehaviour, preferences.playModeStrippingMethod);
				}
			}
			return instance;
		}

		public static void OnSceneObjectAwake(GameObject gameObject)
		{
			Instance().HandleOnSceneObjectAwake(gameObject);
		}

		private void HandleOnSceneObjectAwake(GameObject gameObject)
		{
			if(playModeStripping == StrippingType.None)
			{
				return;
			}

			var scene = gameObject.scene;
			if(playModeStrippingHandledForScenes.Contains(scene))
			{
				return;
			}

			switch(playModeStrippingMethod)
			{
				case PlayModeStrippingMethod.EntireSceneWhenLoaded:
					if(!scene.isLoaded)
					{
						return;
					}
					break;
				case PlayModeStrippingMethod.IndividuallyDuringAwake:
					if(!scene.isLoaded)
					{
						var rootTransform = gameObject.transform.transform.root;
						HashSet<Transform> handledRootObjects;
						if(!playModeStrippingHandledForSceneRootObjects.TryGetValue(gameObject.scene, out handledRootObjects))
						{
							playModeStrippingHandledForSceneRootObjects.Add(gameObject.scene, new HashSet<Transform>(){ rootTransform });
						}
						else if(!handledRootObjects.Add(rootTransform))
						{
							return;
						}
						HierarchyFolderUtility.CheckForAndRemoveHierarchyFoldersInChildren(gameObject.transform.root, playModeStripping);
						return;
					}
					break;
			}

			playModeStrippingHandledForScenes.Add(gameObject.scene);
			HierarchyFolderUtility.ApplyStrippingType(gameObject.scene, playModeStripping);
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			#if DEV_MODE
			Debug.Assert(playModeStripping != StrippingType.None);
			Debug.Assert(scene.isLoaded);
			#endif

			if(!playModeStrippingHandledForScenes.Add(scene))
			{
				return;
			}

			playModeStrippingHandledForScenes.Add(scene);
			HierarchyFolderUtility.ApplyStrippingType(scene, playModeStripping);
		}

		private void OnSceneUnloaded(Scene scene)
		{
			playModeStrippingHandledForScenes.Remove(scene);
			playModeStrippingHandledForSceneRootObjects.Remove(scene);
		}
	}
}
#endif