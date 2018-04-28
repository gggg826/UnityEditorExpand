/********************************************************************
*	created:	29/4/2018   2:35
*	filename: 	OldReferenceCheckWindow
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OldReferencesChecker
{
	public class OldReferenceCheckWindow : EditorWindow
	{
		protected string m_SoureceRelativePath = "Assets/BundleData/UI";
		protected string m_OldFolder = "FengYun";
		protected string m_CullExtensions = ".cs";

		protected OldReferenceData m_OldReferenceData;
		protected List<string> m_Extensions;
		protected Vector2 m_ScrollPosition;

		[MenuItem("Window/Old Reference Checker")]
		public static void OpenOldReferenceCheckWindow()
		{
			GetWindow<OldReferenceCheckWindow>("Old Reference", true).Show();
		}

		protected void OnGUI()
		{
			if (m_OldReferenceData == null)
			{
				m_OldReferenceData = new OldReferenceData();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			GUILayout.BeginVertical();
			DrawView();
			GUILayout.EndVertical();
			GUILayout.Space(5);
			GUILayout.EndHorizontal();
		}

		protected void DrawView()
		{
			GUILayout.Space(10);
			float tempLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 100;
			m_SoureceRelativePath = EditorGUILayout.TextField("Sourece Path", m_SoureceRelativePath, GUILayout.MinWidth(300));
			m_OldFolder = EditorGUILayout.TextField("Old Folder", m_OldFolder, GUILayout.MinWidth(300));
			m_CullExtensions = EditorGUILayout.TextField("Cull Extensions", m_CullExtensions, GUILayout.MinWidth(300));
			CheckCullExtensions();
			EditorGUIUtility.labelWidth = tempLabelWidth;
			GUILayout.Space(10);
			if (GUILayout.Button("Check Old References", GUILayout.MinHeight(30)))
			{
				m_OldReferenceData.CheckOldReferences(m_SoureceRelativePath, m_OldFolder, m_Extensions);
			}
			GUILayout.Label("Total Count : " + m_OldReferenceData.OldReferenceFileGroupDic.Count);
			GUILayout.Space(10);
			m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
			m_OldReferenceData.Draw();
			GUILayout.EndScrollView();
			GUILayout.Space(10);
		}

		protected void CheckCullExtensions()
		{
			if (m_Extensions == null)
			{
				m_Extensions = new List<string>();
			}
			m_Extensions.Clear();
			if (m_CullExtensions.IndexOf(',') == -1)
			{
				m_Extensions.Add(m_CullExtensions);
			}
			else
			{
				string[] extensionArray = m_CullExtensions.Split(',');
				for (int i = 0; i < extensionArray.Length; i++)
				{
					m_Extensions.Add(extensionArray[i]);
				}
			}
		}
	}
}