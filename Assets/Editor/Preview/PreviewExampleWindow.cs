using UnityEngine;
using UnityEditor;

public class PreviewExampleWindow : EditorWindow
{
	private Editor m_Editor;

	[MenuItem("Window/PreviewExample")]
	static void ShowWindow()
	{
		GetWindow<PreviewExampleWindow>("PreviewExample");
	}

	private void OnDestroy()
	{
		if (m_Editor != null)
		{
			DestroyImmediate(m_Editor);
		}
		m_Editor = null;
	}

	void OnGUI()
	{
		if (m_Editor == null)
		{
			// 第一个参数这里暂时没关系，因为编辑器没有取目标对象
			m_Editor = Editor.CreateEditor(this, typeof(PreviewExampleInspector));
		}

		m_Editor.DrawPreview(GUILayoutUtility.GetRect(300, 200));
	}
}
