/********************************************************************
*	created:	29/4/2018   2:36
*	filename: 	OldReferenceData
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OldReferencesChecker
{
	public class OldReferenceFileData
	{
		public string RootPrefabRelativePath;
		public string Reference;
		public Action<string> OnClickEvent;

		public bool IsChoosed;

		public void Draw()
		{
			bool newChoosed = EditorGUILayout.ToggleLeft(Reference, IsChoosed);
			if (newChoosed != IsChoosed)
			{
				if (OnClickEvent != null)
				{
					OnClickEvent(Reference);
				}
			}
		}
	}

	public class OldReferenceFileGroupData
	{
		public string GroupName;
		public string GroupPrefabPath;
		public List<OldReferenceFileData> OldFilesList;
		public Action<string> OnClickEvent;

		public OldReferenceFileGroupData(string groupName)
		{
			GroupName = groupName;
			OldFilesList = new List<OldReferenceFileData>();
		}

		public void Draw()
		{
			if (GUILayout.Button(GroupName, GUILayout.MinWidth(300)))
			{
				if (OnClickEvent != null)
				{
					OnClickEvent(GroupPrefabPath);
				}
			}
			EditorGUI.indentLevel++;
			for (int i = 0; i < OldFilesList.Count; i++)
			{
				OldFilesList[i].Draw();
			}
			EditorGUI.indentLevel--;
		}
	}

	public class OldReferenceData
	{
		public Dictionary<string, OldReferenceFileGroupData> OldReferenceFileGroupDic;

		protected string m_CurrentPreviousShowPath;
		protected List<Object> m_PrefabObjs;

		public OldReferenceData()
		{
			OldReferenceFileGroupDic = new Dictionary<string, OldReferenceFileGroupData>();
			m_PrefabObjs = new List<Object>();
		}

		public void CheckOldReferences(string souresRelativePath, string oldFolderName, List<string> cullExtensions)
		{
			string[] prefabsPath = Directory.GetFiles(Path.GetFullPath(souresRelativePath), "*.prefab", SearchOption.AllDirectories);

			if (prefabsPath == null)
			{
				return;
			}

			if (m_PrefabObjs == null)
			{
				m_PrefabObjs = new List<Object>();
			}
			else
			{
				m_PrefabObjs.Clear();
			}

			int currentIndex = 0;
			bool isCancel = false;
			EditorApplication.update = delegate ()
			{
				string assetPath = prefabsPath[currentIndex];

				isCancel = EditorUtility.DisplayCancelableProgressBar("正在加载Prefab..."
																			, assetPath
																			, (float)currentIndex / (float)prefabsPath.Length);

				Object obj = LoadPrefabs(assetPath);
				if (obj != null)
				{
					m_PrefabObjs.Add(obj);
				}

				currentIndex++;
				if (isCancel || currentIndex >= prefabsPath.Length)
				{
					EditorUtility.ClearProgressBar();
					EditorApplication.update = null;
					if (currentIndex >= prefabsPath.Length)
					{
						CollectReferences(oldFolderName, cullExtensions);
					}
					currentIndex = 0;
				}
			};

			AssetDatabase.Refresh();
		}

		public void Draw()
		{
			foreach (var duplicaionGroupAsset in OldReferenceFileGroupDic)
			{
				duplicaionGroupAsset.Value.Draw();
			}
		}

		protected Object LoadPrefabs(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return null;
			}
			string relativePath = filePath.Substring(filePath.IndexOf("Assets"));
			return AssetDatabase.LoadAssetAtPath(relativePath, typeof(Object));
		}

		protected void CollectReferences(string oldFolderName, List<string> cullExtensions)
		{
			if (m_PrefabObjs == null || m_PrefabObjs.Count == 0)
			{
				return;
			}

			int currentIndex = 0;
			bool isCancel = false;
			EditorApplication.update = delegate ()
			{
				Object obj = m_PrefabObjs[currentIndex];

				isCancel = EditorUtility.DisplayCancelableProgressBar("正在收集依赖..."
																			, obj.name
																			, (float)currentIndex / (float)m_PrefabObjs.Count);

				CollectOldReferences(obj, oldFolderName, cullExtensions);
				currentIndex++;
				if (isCancel || currentIndex >= m_PrefabObjs.Count)
				{
					EditorUtility.ClearProgressBar();
					EditorApplication.update = null;
					currentIndex = 0;
				}
			};
		}

		protected void CollectOldReferences(Object obj, string oldFolderName, List<string> cullExtensions)
		{
			if (obj == null)
			{
				return;
			}

			GameObject root = (GameObject)obj;
			if (root == null)
			{
				return;
			}
			Object[] dependObjs = EditorUtility.CollectDependencies(new Object[] { obj });
			List<Object> shaderDependObjs = new List<Object>();
			for (int objIndex = 0; objIndex < dependObjs.Length; objIndex++)
			{
				string path = AssetDatabase.GetAssetPath(dependObjs[objIndex]);
				string extension = Path.GetExtension(path);
				if (path.Contains(oldFolderName) && !cullExtensions.Contains(extension))
				{
					AddReference(AssetDatabase.GetAssetPath(root), path);
				}
			}
		}

		protected List<GameObject> GetChildren(GameObject go)
		{
			List<GameObject> result = new List<GameObject>();
			foreach (Transform child in go.transform)
			{
				result.Add(child.gameObject);
				foreach (Transform item in child)
				{
					GetChildren(item.gameObject);
				}
			}
			return result;
		}

		protected void AddReference(string prefabPath, string referencePath)
		{
			if (string.IsNullOrEmpty(prefabPath) || string.IsNullOrEmpty(referencePath))
			{
				return;
			}

			OldReferenceFileData referenceData = new OldReferenceFileData();
			referenceData.Reference = referencePath;

			OldReferenceFileGroupData group = OldReferenceFileGroupDic.ContainsKey(prefabPath) ? OldReferenceFileGroupDic[prefabPath] : null;
			if (group == null)
			{
				group = new OldReferenceFileGroupData(prefabPath);
				group.GroupName = prefabPath;
				group.GroupPrefabPath = prefabPath;
				group.OnClickEvent = OnPreviousChanged;
				group.OldFilesList.Add(referenceData);
				OldReferenceFileGroupDic.Add(prefabPath, group);
			}
			else
			{
				OldReferenceFileGroupDic[prefabPath].OldFilesList.Add(referenceData);
			}
			referenceData.OnClickEvent = OnPreviousChanged;
		}

		protected void OnPreviousChanged(string prefabRootPath)
		{
			if (m_CurrentPreviousShowPath != prefabRootPath)
			{
				Selection.activeObject = AssetDatabase.LoadAssetAtPath(prefabRootPath, typeof(Object));
				m_CurrentPreviousShowPath = prefabRootPath;
			}
		}
	}
}