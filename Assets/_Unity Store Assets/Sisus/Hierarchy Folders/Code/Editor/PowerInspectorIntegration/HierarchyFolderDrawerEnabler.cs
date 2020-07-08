#if POWER_INSPECTOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	[InitializeOnLoad]
	public static class HierarchyFolderDrawerEnabler
	{
		static HierarchyFolderDrawerEnabler()
		{
			HandleEnableHierarchyFolderDrawer();
		}

		private static void HandleEnableHierarchyFolderDrawer()
		{
			if(BuildPipeline.isBuildingPlayer)
			{
				return;
			}

			if(EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += HandleEnableHierarchyFolderDrawer;
			}

			var disabledDrawerGuidResults = AssetDatabase.FindAssets("HierarchyFolderDrawer.cs");
			if(disabledDrawerGuidResults.Length == 1)
			{
				var disabledDrawerLocalPath = AssetDatabase.GUIDToAssetPath(disabledDrawerGuidResults[0]);
				if(disabledDrawerLocalPath.EndsWith(".disabled", StringComparison.OrdinalIgnoreCase))
				{
					#if UNITY_2017_3_OR_NEWER //AssemblyDefinitionAsset did not exist before Unity version 2017.3

					AssetDatabase.StartAssetEditing();

					var assemblyDefinitionFileGuidResults = AssetDatabase.FindAssets("Sisus.HierarchyFolders");
					for(int n = assemblyDefinitionFileGuidResults.Length - 1; n >= 0; n--)
					{
						var assemblyDefinitionFileLocalPath = AssetDatabase.GUIDToAssetPath(assemblyDefinitionFileGuidResults[n]);
						if(string.Equals(Path.GetFileName(assemblyDefinitionFileLocalPath), "Sisus.HierarchyFolders.Editor.asmdef", StringComparison.OrdinalIgnoreCase))
						{
							var assemblyDefinitionFile = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(assemblyDefinitionFileLocalPath);
							if(assemblyDefinitionFile != null)
							{
								string json = assemblyDefinitionFile.text;
								int alreadyFoundAt = json.IndexOf("\"PowerInspector.Runtime\"");
								if(alreadyFoundAt == -1)
								{
									int insertAt = json.IndexOf("\"Sisus.HierarchyFolders\"");
									if(insertAt != -1)
									{
										json = string.Concat(json.Substring(0, insertAt), "\"PowerInspector.Runtime\",\n        ", json.Substring(insertAt));
									}
								}
								string assemblyDefinitionFileFullPath = Path.Combine(Application.dataPath, assemblyDefinitionFileLocalPath.Substring(7)).Replace('/', '\\');
								File.WriteAllText(assemblyDefinitionFileFullPath, json);
							}
						}
					}
					#endif						

					var enabledDrawerLocalPath = disabledDrawerLocalPath.Substring(0, disabledDrawerLocalPath.Length - ".disabled".Length);
					FileUtil.MoveFileOrDirectory(disabledDrawerLocalPath, enabledDrawerLocalPath);

					AssetDatabase.StopAssetEditing();
				}
			}
		}
		
	}
}
#endif