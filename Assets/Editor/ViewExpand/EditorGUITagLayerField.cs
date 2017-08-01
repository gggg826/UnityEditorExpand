using UnityEditor;
using UnityEngine;

public class EditorGUITagLayerField : EditorWindow
{

	string selectedTag = "";
	int selectedLayer = 0;

	[MenuItem("Examples/Tag - Layer for Selection")]
	static void Init()
{
	EditorWindow window = GetWindow(typeof(EditorGUITagLayerField));
	window.position = new Rect(0, 0, 350, 70);
	window.Show();
}

void OnGUI()
{
	selectedTag = EditorGUI.TagField(
		new Rect(3, 3, position.width / 2 - 6, 20),
		"New Tag:",
		selectedTag);
	selectedLayer = EditorGUI.LayerField(
		new Rect(position.width / 2 + 3, 3, position.width / 2 - 6, 20),
		"New Layer:",
		selectedLayer);

	if (Selection.activeGameObject)
	{
		if (GUI.Button(new Rect(3, 25, 90, 17), "Change Tags"))
			foreach (GameObject go  in Selection.gameObjects)
				go.tag = selectedTag;
		if (GUI.Button(new Rect(position.width - 96, 25, 90, 17), "Change Layers"))
				foreach (GameObject go in Selection.gameObjects)
				go.layer = selectedLayer;
	}
}

void OnInspectorUpdate()
{
	Repaint();
}
}
