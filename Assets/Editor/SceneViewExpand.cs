/********************************************************************
*	created:	28/7/2017   1:03
*	filename: 	SceneViewExpand
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using System.Collections;
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
    public int MaxRecordScenesLength;

    private List<EditorViewItem> m_SceneExpandItems;
    private GUIContent[] SceneDisplayOptions;
    private string[] m_RecordScenes;
    private string m_RegKey_RecordCount;
    private string m_RegKey_RecordPrefix;
    private string m_LastScene;
    public SceneViewExpand()
    {
        string projectName = Path.GetFileNameWithoutExtension(System.Environment.CurrentDirectory);
        Debug.Log(projectName);
        m_RegKey_RecordCount = projectName + "_Scenes_Count";
        m_RegKey_RecordPrefix = projectName + "_ScenesRecord_";
        m_LastScene = SceneManager.GetActiveScene().path;

        int length = EditorPrefs.GetInt(m_RegKey_RecordCount, 0);

        if (length < 1)
        {
            m_RecordScenes = new string[] { m_LastScene };
        }
        else
        {
            for (int i = 0; i < length; ++i)
            {
                m_RecordScenes[i] = WWW.UnEscapeURL(EditorPrefs.GetString(m_RegKey_RecordPrefix + i, string.Empty));
            }
        }
       
        UpdateRecordScenes(m_LastScene);

        m_SceneExpandItems = new List<EditorViewItem>();
        ViewExpandUtils.AddCustom(ref m_SceneExpandItems, AddRecentButton);
    }

    public void OnHierarchyWindowChanged()
    {
        if (m_LastScene != EditorSceneManager.GetActiveScene().path)
        {
            m_LastScene = EditorApplication.currentScene;
            UpdateRecordScenes(EditorApplication.currentScene);
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
    
    void UpdateRecordScenes(string scenePath)
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

        SceneDisplayOptions = new GUIContent[m_RecordScenes.Length];
        for (int i = 0; i < m_RecordScenes.Length; ++i)
        {
            SceneDisplayOptions[i] = new GUIContent(i + " " + Path.GetFileNameWithoutExtension(m_RecordScenes[i]));
        }

        EditorPrefs.SetInt(m_RegKey_RecordCount, m_RecordScenes.Length);
        for (int i = 0; i < m_RecordScenes.Length; ++i)
        {
            EditorPrefs.SetString(m_RegKey_RecordPrefix + i, WWW.EscapeURL(m_RecordScenes[i]));
        }
    }
    
    private void AddRecentButton()
    {
        GUIContent contentRecent = new GUIContent("Recent");
        GUIStyle styleRecent = EditorStyles.toolbarDropDown;
        Rect rect = GUILayoutUtility.GetRect(contentRecent, styleRecent, GUILayout.Width(60));
        if (GUI.Button(rect, contentRecent, styleRecent))
        {
            EditorUtility.DisplayCustomMenu(rect, SceneDisplayOptions, -1, OnRecentMenuSelected, null);
        }
    }
}
