using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class EditorViewManager
{
    static EditorViewManager()
    {
        SceneViewExpand sceneViewExpand = SceneViewExpand.Instance;
        sceneViewExpand.MaxRecordScenesLength = 5;
        SceneView.onSceneGUIDelegate -= sceneViewExpand.OnSceneFunc;
        SceneView.onSceneGUIDelegate += sceneViewExpand.OnSceneFunc;
        EditorApplication.hierarchyWindowChanged -= sceneViewExpand.OnHierarchyWindowChanged;
        EditorApplication.hierarchyWindowChanged += sceneViewExpand.OnHierarchyWindowChanged;

    }

}
