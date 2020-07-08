﻿//#define DEBUG_ON_VALIDATE
//#define DEBUG_HIERARCHY_CHANGED

using UnityEngine;
using Sisus.Attributes;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;
using JetBrains.Annotations;
#endif

namespace Sisus.HierarchyFolders
{
	[AddComponentMenu("Hierarchy/Hierarchy Folder")]
	[HideTransformInInspector, HideComponentInInspector, OnlyComponent]
	#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	public class HierarchyFolder : MonoBehaviour
	{
		#if UNITY_EDITOR
		public static Scene playModeStrippingHandled;
		private static readonly List<Component> ReusableComponentsList = new List<Component>(2);

		[UsedImplicitly]
		private void Reset()
		{
			if(HasSupernumeraryComponents())
			{
				Debug.LogWarning("Can't add HierarchyFolder component to GameObject with existing components.");
				TurnIntoNormalGameObject();
				return;
			}

			ResetTransformStateWithoutAffectingChildren();

			transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

			hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
			EditorUtility.SetDirty(transform);
			gameObject.isStatic = true;
			EditorUtility.SetDirty(this);
			var preferences = HierarchyFolderPreferences.Get();
			if(preferences.autoNameOnAdd)
			{
				if(gameObject.name.Equals("GameObject", StringComparison.Ordinal) || gameObject.name.StartsWith("GameObject (", StringComparison.Ordinal))
				{
					gameObject.name = preferences.defaultName;
				}
				else
				{
					ApplyNamingPattern();
				}
			}

			EditorUtility.SetDirty(gameObject);
		}

		[UsedImplicitly]
		private void Awake()
		{
			if(playModeStrippingHandled == gameObject.scene)
			{
				return;
			}

			playModeStrippingHandled = gameObject.scene;

			var preferences = HierarchyFolderPreferences.Get();
			var playModeStripping = preferences.playModeBehaviour;
			if(playModeStripping != StrippingType.None)
			{
				HierarchyFolderUtility.ApplyStrippingType(gameObject.scene, playModeStripping);
			}
		}

		private void ResubscribeToHierarchyChanged(HierarchyFolderPreferences preferences)
		{
			#if DEV_MODE && DEBUG_HIERARCHY_CHANGED
			Debug.Log(name + ".ResubscribeToHierarchyChanged");
			#endif

			UnsubscribeToHierarchyChanged(preferences);
			
			preferences.onPreferencesChanged += ResubscribeToHierarchyChanged;

			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.hierarchyChanged += OnHierarchyChangedInEditMode;
			}
			else
			{
				if(preferences.playModeBehaviour == StrippingType.FlattenHierarchy)
				{
					EditorApplication.hierarchyChanged += OnHierarchyChangedInPlayModeFlattened;
				}
				else
				{
					EditorApplication.hierarchyChanged += OnHierarchyChangedInPlayModeGrouped;
				}
			}
		}

		private void UnsubscribeToHierarchyChanged(HierarchyFolderPreferences preferences)
		{
			preferences.onPreferencesChanged -= ResubscribeToHierarchyChanged;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInEditMode;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeFlattened;
			EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeGrouped;
		}

		[UsedImplicitly]
		private void OnValidate()
		{
			#if DEV_MODE && DEBUG_ON_VALIDATE
			Debug.Log(name + ".OnValidate");
			#endif

			if(this == null)
			{
				return;
			}

			ResubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());
		}

		private void ResetTransformStateWithoutAffectingChildren()
		{
			#if DEV_MODE && DEBUG_RESET_STATE
			Debug.Log(name + ".ResetTransformStateWithoutAffectingChildren");
			#endif

			var transform = this.transform;
			var rectTransform = transform as RectTransform;

			if(transform.localPosition != Vector3.zero || transform.localEulerAngles != Vector3.zero || transform.localScale != Vector3.one || (rectTransform != null && (rectTransform.anchorMin != Vector2.zero || rectTransform.anchorMax != Vector2.one || rectTransform.pivot != new Vector2(0.5f, 0.5f) || rectTransform.offsetMin != Vector2.zero || rectTransform.offsetMax != Vector2.zero)))
			{
				ForceResetTransformStateWithoutAffectingChildren(false);
			}
		}

		private void ForceResetTransformStateWithoutAffectingChildren(bool alsoConvertToRectTransform)
		{
			var transform = this.transform;

			// For prefab instances use a method where children are not unparented temporarily
			// because this would require the prefab instance to get unpacked.
			#if UNITY_2018_3_OR_NEWER
			if(PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.Connected)
			#else
			if(PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
			#endif
			{
				int childCount = transform.childCount;
				var positions = new Vector3[childCount];
				var rotations = new Quaternion[childCount];
				var scales = new Vector3[childCount];

				for(int n = childCount - 1; n >= 0; n--)
				{
					var child = transform.GetChild(n);
					positions[n] = child.position;
					rotations[n] = child.rotation;
					scales[n] = child.lossyScale;
				}

				RectTransform rectTransform;
				if(alsoConvertToRectTransform)
				{
					rectTransform = gameObject.AddComponent<RectTransform>();
					rectTransform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
					transform = rectTransform;
				}
				else
				{
					rectTransform = transform as RectTransform;
				}

				if(rectTransform != null)
				{
					rectTransform.anchorMin = Vector2.zero;
					rectTransform.anchorMax = Vector2.one;
					rectTransform.pivot = new Vector2(0.5f, 0.5f);
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.offsetMax = Vector2.zero;
				}

				transform.localPosition = Vector3.zero;
				transform.localEulerAngles = Vector3.zero;
				transform.localScale = Vector3.one;

				for(int n = childCount - 1; n >= 0; n--)
				{
					var child = transform.GetChild(n);
					child.position = positions[n];
					child.rotation = rotations[n];
					child.localScale = Vector3.one;
					var setWorldScale = scales[n];
					child.localScale = new Vector3(setWorldScale.x / transform.lossyScale.x, setWorldScale.y / transform.lossyScale.y, setWorldScale.z / transform.lossyScale.z);
				}
			}
			// For non-prefab instances use a method where children are unparented temporarily.
			// This has the benefit the the world position of all children remains stable throughout the whole process.
			else
			{
				var parent = transform.parent;
				int childCount = transform.childCount;
				var children = new Transform[childCount];
				for(int n = childCount - 1; n >= 0; n--)
				{
					children[n] = transform.GetChild(n);

					// NOTE: Using SetParent with worldPositionStays true is very important (even with RectTransforms).
					children[n].SetParent(parent, true);
				}

				RectTransform rectTransform;
				if(alsoConvertToRectTransform)
				{
					rectTransform = gameObject.AddComponent<RectTransform>();
					rectTransform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
					transform = rectTransform;
				}
				else
				{
					rectTransform = transform as RectTransform;
				}

				if(rectTransform != null)
				{
					rectTransform.anchorMin = Vector2.zero;
					rectTransform.anchorMax = Vector2.one;
					rectTransform.pivot = new Vector2(0.5f, 0.5f);
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.offsetMax = Vector2.zero;
				}

				transform.localPosition = Vector3.zero;
				transform.localEulerAngles = Vector3.zero;
				transform.localScale = Vector3.one;

				for(int n = 0; n < childCount; n++)
				{
					children[n].SetParent(transform, true);
					children[n].SetAsLastSibling();
				}
			}

			EditorUtility.SetDirty(transform);
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			UnsubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());
		}

		private void OnHierarchyChangedInEditMode()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInEditMode;
				return;
			}

			// If has RectTransform child convert Transform component into RectTransform 
			// to avoid child RectTransform values being affected by the parent hierarchy folders.
			// For performance reasons only first child is checkd.
			if(transform.GetFirstChild(true) is RectTransform && !(transform is RectTransform))
			{
				#if DEV_MODE
				Debug.LogWarning("Converting Hierarchy Folder " + name + " Transform into RectTransform because it had a RectTransform child.", gameObject);
				#endif

				ForceResetTransformStateWithoutAffectingChildren(true);
			}

			OnHierarchyChangedShared();

			ApplyNamingPattern();
		}

		private void OnHierarchyChangedInPlayModeFlattened()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeFlattened;
				return;
			}

			OnHierarchyChangedShared();

			#if DEV_MODE
			if(transform.childCount > 0)
			{
				if(HierarchyFolderUtility.NowStripping) { Debug.LogWarning(name + " child count is "+ transform.childCount+" but won't flatten because HierarchyFolderUtility.NowStripping already true."); }
				else { Debug.Log(name + " child count " + transform.childCount+". Flattening now..."); }
			}
			#endif

			if(transform.childCount > 0 && !HierarchyFolderUtility.NowStripping)
			{
				int moveToIndex = HierarchyFolderUtility.GetLastChildIndexInFlatMode(gameObject);
				for(int n = transform.childCount - 1; n >= 0; n--)
				{
					var child = transform.GetChild(n);
					child.SetParent(null, true);
					child.SetSiblingIndex(moveToIndex);
				}
			}
		}

		private void OnHierarchyChangedInPlayModeGrouped()
		{
			if(this == null)
			{
				EditorApplication.hierarchyChanged -= OnHierarchyChangedInPlayModeGrouped;
				return;
			}

			OnHierarchyChangedShared();
		}

		private void OnHierarchyChangedShared()
		{
			if(HasSupernumeraryComponents())
			{
				Debug.LogWarning("GameObject " + name + " contained the HierarchyFolder component and other components besides transforms.\nThis is not supported since HierarchyFolder GameObjects are stripped from builds. Removing the HierarchyFolder component now.");
				TurnIntoNormalGameObject();
				return;
			}

			ResetTransformStateWithoutAffectingChildren();
		}

		private bool HasSupernumeraryComponents()
		{
			GetComponents(ReusableComponentsList);
			// A hierarchy folder GameObject should only have Transform (or RectTransform) and HierarchyFolder components.
			return ReusableComponentsList.Count > 2;
		}

		[ContextMenu("Turn Into Normal GameObject")]
		private void TurnIntoNormalGameObject()
		{
			UnsubscribeToHierarchyChanged(HierarchyFolderPreferences.Get());

			// Can help avoid NullReferenceExceptions via hierarchyChanged callback
			// by adding a delay between the unsubscribing and the destroying of the HierarchyFolder component
			EditorApplication.delayCall += UnmakeHierarchyFolder;
		}

		private void UnmakeHierarchyFolder()
		{
			HierarchyFolderUtility.UnmakeHierarchyFolder(gameObject, this);
		}

		private void ApplyNamingPattern()
		{
			var preferences = HierarchyFolderPreferences.Get();
			if(!preferences.enableNamingRules)
			{
				return;
			}

			string setName = gameObject.name;
			bool possiblyChanged = false;

			if(preferences.forceNamesUpperCase)
			{
				setName = setName.ToUpper();
				possiblyChanged = true;
			}

			string prefix = preferences.namePrefix;
			if(!setName.StartsWith(prefix, StringComparison.Ordinal))
			{
				possiblyChanged = true;

				if(setName.StartsWith(preferences.previousNamePrefix, StringComparison.Ordinal))
				{
					setName = setName.Substring(preferences.previousNamePrefix.Length);
				}

				for(int c = prefix.Length - 1; c >= 0 && !setName.StartsWith(prefix, StringComparison.Ordinal); c--)
				{
					setName = prefix[c] + setName;
				}
			}

			string suffix = preferences.nameSuffix;
			if(!setName.EndsWith(suffix, StringComparison.Ordinal))
			{
				possiblyChanged = true;

				// Handle situation where a hierarchy folder has been duplicated and a string like "(1)"
				// has been added to the end of the name.
				if(setName.EndsWith(")", StringComparison.Ordinal))
				{
					int openParenthesis = setName.LastIndexOf(" (", StringComparison.Ordinal);					
					if(openParenthesis != -1)
					{
						string ending = setName.Substring(openParenthesis);
						if(ending.Length <= 5 && setName.EndsWith(suffix + ending, StringComparison.Ordinal))
						{
							int from = openParenthesis + 2;
							int to = setName.Length - 1;
							string nthString = setName.Substring(from, to - from);
							int nthInt;
							if(int.TryParse(nthString, out nthInt))
							{
								setName = setName.Substring(0, openParenthesis - suffix.Length) + suffix;
							}
						}
					}
				}

				if(setName.EndsWith(preferences.previousNameSuffix, StringComparison.Ordinal))
				{
					setName = setName.Substring(0, setName.Length - preferences.previousNameSuffix.Length);
				}

				for(int c = 0, count = suffix.Length; c < count && !setName.EndsWith(suffix, StringComparison.Ordinal); c++)
				{
					setName += suffix[c];
				}
			}

			if(possiblyChanged && !string.Equals(setName, gameObject.name))
			{
				gameObject.name = setName;
			}
		}
		#endif
	}
}