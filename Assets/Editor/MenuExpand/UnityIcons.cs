using UnityEngine;
using UnityEditor;
using System;

class UnityIcons : EditorWindow
{
	static string[] text;

	[MenuItem("Window/My Window")]
	public static void ShowWindow()
	{
		TextAsset iconText = Resources.Load<TextAsset>("UnityIcons");
		text = iconText.text.Replace("\r", "").Split('\n');
		GetWindow(typeof(UnityIcons));
	}
	public Vector2 scrollPosition;
	void OnGUI()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		//鼠标放在按钮上的样式
		foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
		{
			GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
			GUILayout.Space(10);
		}


		//内置图标
		for (int i = 0; i < text.Length; i += 8)
		{
			GUILayout.BeginHorizontal();
			for (int j = 0; j < 8; j++)
			{
				int index = i + j;
				if (index < text.Length)
				{
					if (GUILayout.Button(EditorGUIUtility.IconContent(text[index]), GUILayout.Width(50), GUILayout.Height(30)))
					{
						Debug.Log("[Icon_Name] " + text[index]);
					}
				}

			}
			GUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();
	}
}