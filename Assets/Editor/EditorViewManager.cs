/********************************************************************
*	created:	28/7/2017   1:03
*	filename: 	SceneViewExpand
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEditor;

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
