/********************************************************************
*	created:	28/7/2017   1:03
*	filename: 	SceneViewExpand
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneViewExpand
{
	private static SceneViewExpand m_Instance;
	public static SceneViewExpand Instance
	{
		get
		{
			if (null == m_Instance)
				m_Instance = new SceneViewExpand();
			return m_Instance;
		}
	}
	public int MaxRecordScenesLength = 5;

	private List<EditorViewItem> m_SceneExpandItems;

	private GUIContent[] SceneDisplayOptions;
	private string[] m_RecordScenes;
	private string m_RegKey_RecordCount;
	private string m_RegKey_RecordPrefix;
	private string m_LastScene;
	private bool m_Started;
	private int m_PlayerLayer;

	public SceneViewExpand()
	{
		string projectName = Path.GetFileNameWithoutExtension(System.Environment.CurrentDirectory);
		m_RegKey_RecordCount = projectName + "_Scenes_Count";
		m_RegKey_RecordPrefix = projectName + "_ScenesRecord_";
		m_LastScene = SceneManager.GetActiveScene().path;
		m_Started = false;

		int length = EditorPrefs.GetInt(m_RegKey_RecordCount, 0);
		m_RecordScenes = new string[length];
		for (int i = 0; i < length; ++i)
		{
			m_RecordScenes[i] = WWW.UnEscapeURL(EditorPrefs.GetString(m_RegKey_RecordPrefix + i, string.Empty));
		}
		UpdateSceneDisplayOptions();
		m_SceneExpandItems = new List<EditorViewItem>();
		
		AddRecentButton();
		AddSpace();
		AddUIEditorButton();
		AddSpace();
		AddStartGameButton();
	}

	public void OnHierarchyWindowChanged()
	{
		if (m_LastScene != SceneManager.GetActiveScene().path || !m_Started)
		{
			m_Started = true;
			m_LastScene = SceneManager.GetActiveScene().path;
			UpdateRecordScenes(SceneManager.GetActiveScene().path);
			SaveConfig();
		}
	}

	public void OnSceneFunc(SceneView sceneView)
	{
		Handles.BeginGUI();
		GUILayout.Space(-18);
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
		foreach (var item in m_SceneExpandItems)
			item.Draw();
		GUILayout.EndHorizontal();
		Handles.EndGUI();
	}

	/// <summary>
	/// 关闭旧场景，打开新场景
	/// </summary>
	/// <param name="userData"></param>
	/// <param name="options"></param>
	/// <param name="selected"></param>
	private void OnRecentMenuSelected(object userData, string[] options, int selected)
	{
		if (selected >= 0 && selected < m_RecordScenes.Length)
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(m_RecordScenes[selected]);
			}
		}
	}

	private void UpdateRecordScenes(string scenePath)
	{
		List<string> recordScenes = new List<string>(m_RecordScenes);
		if (File.Exists(scenePath) && !string.IsNullOrEmpty(scenePath))
		{
			if (recordScenes.Contains(scenePath))
				recordScenes.Remove(scenePath);
			recordScenes.Insert(0, scenePath);
			if (recordScenes.Count > MaxRecordScenesLength)
				recordScenes.RemoveRange(MaxRecordScenesLength, recordScenes.Count - MaxRecordScenesLength);
		}

		m_RecordScenes = recordScenes.ToArray();
		UpdateSceneDisplayOptions();

	}

	private void UpdateSceneDisplayOptions()
	{
		SceneDisplayOptions = new GUIContent[m_RecordScenes.Length];
		for (int i = 0; i < m_RecordScenes.Length; ++i)
		{
			SceneDisplayOptions[i] = new GUIContent(i + " " + Path.GetFileNameWithoutExtension(m_RecordScenes[i]));
		}
	}

	private void SaveConfig()
	{
		EditorPrefs.SetInt(m_RegKey_RecordCount, m_RecordScenes.Length);
		for (int i = 0; i < m_RecordScenes.Length; ++i)
		{
			EditorPrefs.SetString(m_RegKey_RecordPrefix + i, WWW.EscapeURL(m_RecordScenes[i]));
		}
	}

	/// <summary>
	/// 添加Recent Scene下拉菜单
	/// </summary>
	private void AddRecentButton()
	{
		string title = "Recent";
		string tooltip = "快速切换历史Scene\r\n至多可储存5个历史Scene";
		ViewExpandUtils.AddCustom(ref m_SceneExpandItems
								 , () =>
								 {
									 GUIContent contentRecent = new GUIContent(title, tooltip);
									 GUIStyle styleRecent = EditorStyles.toolbarDropDown;
									 Rect rect = GUILayoutUtility.GetRect(contentRecent, styleRecent, GUILayout.Width(60));
									 if (GUI.Button(rect, contentRecent, styleRecent))
									 {
										 EditorUtility.DisplayCustomMenu(rect, SceneDisplayOptions, -1, OnRecentMenuSelected, null);
									 }
								 });
	}

	/// <summary>
	/// 添加启动游戏菜单（无论当前是哪个场景只要BuildSettings里添加过场景就会自动切换到第0个场景并启动游戏）
	/// </summary>
	private void AddStartGameButton()
	{
		string title = "StartGame";
		string tooltip = "快速启动游戏\r\n无论你现在在编辑哪个Scene";
		ViewExpandUtils.AddPushButton(ref m_SceneExpandItems
									, title
									, tooltip
									, () =>
									{
										if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
										{
											EditorBuildSettingsScene[] sceneInBuilderSettings = EditorBuildSettings.scenes;
											if(sceneInBuilderSettings.Length > 0 && !string.IsNullOrEmpty(sceneInBuilderSettings[0].path))
												EditorSceneManager.OpenScene(sceneInBuilderSettings[0].path);
											EditorApplication.isPlaying = true;
										}
									}, GUILayout.Width(70));
	}

	private void AddUIEditorButton()
	{
		string title = "UI Layer";
		string tooltip = "只显示UI Layer\r\n便于编辑UI";
		ViewExpandUtils.AddToggleButton(ref m_SceneExpandItems
									  , title
									  , tooltip
									  , (toggled) =>
									  {
										  //Debug.Log(Tools.visibleLayers);
										  //Tools.visibleLayers值的计算方式
										  //Debug.Log((1 << LayerMask.NameToLayer("UI")) | (1 << LayerMask.NameToLayer("Default")));
										  //下面这个层不存在的话会返回Int的最小值-2147483648
										  //Debug.Log((1 << LayerMask.NameToLayer("NGUI")));
										  SceneView view = SceneView.currentDrawingSceneView;
										  if (toggled)
										  {
											  m_PlayerLayer = EditorPrefs.GetInt("VisibleLayers");
											  //需要根据工程Layer设定更改要打开的层级名
											  Tools.visibleLayers = 1 << LayerMask.NameToLayer("UI");
											  view.in2DMode = true;
											  Transform target = GameObject.Find("Canvas").transform;
											  view.LookAt(target.position);
											  view.size = 500;
										  }
										  else
										  {
											  Tools.visibleLayers = m_PlayerLayer;
											  view.in2DMode = false;
											  view.size = 1;
										  }
									  }
									  , GUILayout.Width(60));
	}

	private void AddSpace()
	{
		ViewExpandUtils.AddSpace(ref m_SceneExpandItems, 6);
	}
}
