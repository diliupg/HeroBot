using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;

namespace Sisus.HierarchyFolders
{
	public static class FileUtility
	{
		#if UNITY_EDITOR
		[CanBeNull]
		public static MonoScript FindScriptFile([NotNull]Type classType)
		{
			string name = classType.Name;

			if(classType.IsGenericType)
			{
				// Parse out generic type information from generic type name
				int i = name.IndexOf('`');
				if(i != -1)
				{
					name = name.Substring(0, i);
				}

				// Additionally, convert generic types to their generic type defitions.
				// E.g. List<string> to List<>.
				if(!classType.IsGenericTypeDefinition)
				{
					classType = classType.GetGenericTypeDefinition();
				}
			}

			var guids = AssetDatabase.FindAssets(name + " t:MonoScript");

			int count = guids.Length;
			if(count == 0)
			{
				return null;
			}

			MonoScript fallback = null;

			for(int n = count - 1; n >= 0; n--)
			{
				var guid = guids[n];
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var filename = Path.GetFileNameWithoutExtension(path);
				if(string.Equals(filename, name, StringComparison.OrdinalIgnoreCase))
				{
					var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
					var scriptClassType = scriptAsset.GetClass();
					if(scriptClassType == classType)
					{
						return scriptAsset;
					}

					if(scriptClassType == null)
					{
						fallback = scriptAsset;
					}
					
					#if DEV_MODE
					Debug.LogWarning("FindScriptFile(" + classType.FullName + ") ignoring file @ \"" + path + "\" because MonoScript.GetClass() result " + (scriptClassType == null ? "null" : scriptClassType.FullName) + " did not match classType.");
					#endif
				}
			}

			// Second pass: test files where filename is only a partial match for class name.
			// E.g. class Header could be defined in file HeaderAttribute.cs.
			if(count > 1)
			{
				for(int n = count - 1; n >= 0; n--)
				{
					var guid = guids[n];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var filename = Path.GetFileNameWithoutExtension(path);
					if(!string.Equals(filename, name, StringComparison.OrdinalIgnoreCase))
					{
						var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
						var scriptClassType = scriptAsset.GetClass();
						if(scriptClassType == classType)
						{
							return scriptAsset;
						}

						if(fallback == null && scriptClassType == null) 
						{
							fallback = scriptAsset;
						}

						#if DEV_MODE
						Debug.LogWarning("FindScriptFile(" + classType.FullName + ") second pass: ignoring file @ \"" + path + "\" because MonoScript.GetClass() result " + (scriptClassType == null ? "null" : scriptClassType.FullName) + " did not match classType.");
						#endif
					}
				}
			}

			// If was unable to verify correct script class type using MonoScript.GetClass()
			// but there was a probable match whose GetClass() returned null (seems to happen
			// with all generic types), then return that.
			if(fallback != null)
			{
				#if DEV_MODE
				Debug.LogWarning("FindScriptFile(" + classType.FullName + ") returning fallback result @ \"" + AssetDatabase.GetAssetPath(fallback) + "\".");
				#endif
				return fallback;
			}

			#if DEV_MODE
			Debug.LogWarning("FindScriptFile(" + classType.FullName + ") failed to find MonoScript for class. AssetDatabase.FindAssets(\""+name + " t:MonoScript\") returned "+count+" results.");
			#endif

			return null;
		}
		#endif
	}
}