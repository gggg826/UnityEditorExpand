/********************************************************************
*	created:	21/8/2017   21:06
*	filename: 	UIQuickModifier
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEngine;
using UnityEditor;

public class UIQuickModifier : EditorWindow
{
	private static Color DefaultBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
	private static Color DefaultContentColor = new Color(1f, 1f, 1f, 0.7f);

	private int m_Depth;
	private int m_Height;
	private int m_With;
	private int m_H_Select_Pivot;
	private int m_V_Select_Pivot;

	private bool m_StyleOneState = false;
	private bool m_StyleTwoState = false;
	private bool m_StyleThreeState = false;

	private SpriteProperty m_SpriteProperty_One;
	private SpriteProperty m_SpriteProperty_Two;
	private SpriteProperty m_SpriteProperty_Three;

	[MenuItem("Window/UIModifier/Button")]
	protected static void Initialize()
	{
		GetWindow<UIQuickModifier>("Quick Replace Button", true);
	}

	protected void OnEnable()
	{
		m_SpriteProperty_One = new SpriteProperty("background", 1, 1, 1, -1, 140, 45);
	}

	protected void OnDisable()
	{
		m_SpriteProperty_One = null;
	}

	protected void OnGUI()
	{
		GUILayout.Space(10);
		DrawSpriteWidget("StyleOne", m_SpriteProperty_One);
		
		GUI.backgroundColor = Color.cyan;
		if(GUILayout.Button("Modify Button Sprite", GUILayout.MinHeight(60f)))
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("Property:\n");
			sb.Append("Name:" + m_SpriteProperty_One.Sprite_Name + "\n");
			sb.Append("Type:" + m_SpriteProperty_One.Sprite_Type + "\n");
			sb.Append("Width:" + m_SpriteProperty_One.Sprite_Width + "\n");
			sb.Append("Height:" + m_SpriteProperty_One.Sprite_Height);
			Debug.Log(sb);
			sb = null;
		}
		GUI.backgroundColor = DefaultBackgroundColor;
	}

	private void DrawSpriteWidget(string title, SpriteProperty sprite)
	{
		if (DrawHeader(title))
		{
			//SpriteProperty newSprite = new SpriteProperty();
			GUILayout.Space(10f);
			BeginContents(DefaultContentColor);
			#region Sprie Name
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name", GUILayout.Width(66f));
			GUILayout.TextField(sprite.Sprite_Name, GUILayout.MinWidth(40f));
			GUILayout.EndHorizontal();
			#endregion
			#region Sprie Name

			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			EditorGUILayout.EnumPopup((UISprite.Type)sprite.Sprite_Type);
			GUILayout.EndHorizontal();
			#endregion
			#region Pivot
			GUILayout.Space(3f);
			int newPivotH = sprite.Sprite_PivotH, newPivotV = sprite.Sprite_PivotV;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pivot", GUILayout.Width(66f));
			if (GUILayout.Toggle(newPivotH == 0, "\u25C4", "ButtonLeft"))
				newPivotH = 0;
			if (GUILayout.Toggle(newPivotH == 1, "\u25AC", "ButtonMid"))
				newPivotH = 1;
			if (GUILayout.Toggle(newPivotH == 2, "\u25BA", "ButtonRight"))
				newPivotH = 2;
			if (GUILayout.Toggle(newPivotV == 0, "\u25B2", "ButtonLeft"))
				newPivotV = 0;
			if (GUILayout.Toggle(newPivotV == 1, "\u258C", "ButtonMid"))
				newPivotV = 1;
			if (GUILayout.Toggle(newPivotV == 2, "\u25BC", "ButtonRight"))
				newPivotV = 2;
			GUILayout.EndHorizontal();

			if (m_H_Select_Pivot != newPivotH)
			{
				m_H_Select_Pivot = newPivotH;
			}
			if (m_V_Select_Pivot != newPivotV)
			{
				m_V_Select_Pivot = newPivotV;
			}

			if (GUILayout.Button("Change Pivot"))
			{
				UIWidget.Pivot pivot = (UIWidget.Pivot)(3 * m_V_Select_Pivot + m_H_Select_Pivot);
				Debug.Log(pivot.ToString());
			}
			#endregion
			#region DrawDepth
			GUILayout.Space(3);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Depth", GUILayout.Width(66f));
			if (GUILayout.Button("Back", GUILayout.MinWidth(46f)))
			{
				m_Depth -= 1;
			}

			GUILayout.Space(5);
			int newDepth = EditorGUILayout.IntField("", /*m_Depth*/sprite.Sprite_Depth, GUILayout.MinWidth(50));
			if (newDepth != m_Depth)
			{
				m_Depth = newDepth;
			}
			GUILayout.Space(5);

			if (GUILayout.Button("Forward", GUILayout.MinWidth(60f)))
			{
				m_Depth += 1;
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Change Depth"))
			{
				Debug.Log("Current Depth: " + m_Depth);
			}
			#endregion
			#region DrawDimensions
			GUILayout.Space(3);
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			int newBoundX = EditorGUILayout.IntField("Size", /*m_With*/sprite.Sprite_Width, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 12;
			int newBoundY = EditorGUILayout.IntField("x", /*m_Height*/sprite.Sprite_Height, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 70;
			if (GUILayout.Button("Snap", GUILayout.Width(60f)))
			{
				Debug.LogError("UNDONE: Change this button to toggle");
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Change Dimensions"))
			{
				Debug.LogError("UNDONE: Change this button to toggle");
			}
			#endregion
			EndContents(DefaultContentColor);
		}
	}

	private void BeginContents(Color contentsColor)
	{
		GUILayout.Space(3f);
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUI.contentColor = contentsColor;
	}

	private void EndContents(Color contentsColor)
	{
		GUI.contentColor = contentsColor;
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(3f);
	}

	private bool DrawHeader(string title)
	{
		bool isOn = m_StyleOneState;
		if (isOn)
			GUI.backgroundColor = Color.cyan;
		GUILayout.BeginHorizontal();
		GUI.changed = false;
		title = "<b><size=11>" + title + "</size></b>";
		if (isOn)
			title = "\u25BC " + title;
		else
			title = "\u25BA " + title;
		if (!GUILayout.Toggle(true, title, "dragtab", GUILayout.MinWidth(20f)))
			isOn = !isOn;
		GUILayout.EndHorizontal();
		if (GUI.changed)
			m_StyleOneState = !m_StyleOneState;
		GUI.backgroundColor = DefaultBackgroundColor;
		return isOn;
	}
}

public class SpriteProperty
{
	public string Sprite_Name;
	public int Sprite_Type;
	public int Sprite_PivotH;
	public int Sprite_PivotV;
	public int Sprite_Depth;
	public int Sprite_Width;
	public int Sprite_Height;

	public SpriteProperty(string name = ""
						, int type = -1
						, int pivotH = -1
						, int pivotV = -1
						, int depth = -1
						, int width = -1
						, int height = -1)
	{
		SetProperty(name, type, pivotH, pivotV, depth, width, height);
	}

	public void SetProperty(string name = ""
					, int type = -1
					, int pivotH = -1
					, int pivotV = -1
					, int depth = -1
					, int width = -1
					, int height = -1)
	{
		Sprite_Name = name;
		Sprite_Type = type;
		Sprite_PivotH = pivotH;
		Sprite_PivotV = pivotV;
		Sprite_Depth = depth;
		Sprite_Width = width;
		Sprite_Height = height;
	}

	public void SetProperty(SpriteProperty content)
	{
		Sprite_Name = content.Sprite_Name;
		Sprite_Type = content.Sprite_Type;
		Sprite_PivotH = content.Sprite_PivotH;
		Sprite_PivotV = content.Sprite_PivotV;
		Sprite_Depth = content.Sprite_Depth;
		Sprite_Width = content.Sprite_Width;
		Sprite_Height = content.Sprite_Height;
	}
}
