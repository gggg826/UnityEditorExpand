/********************************************************************
*	created:	21/8/2017   21:06
*	filename: 	UIQuickModifier
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

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

	//private List<string> m_ViewButtons;
	private Dictionary<string, System.Action> m_ViewItems;

	private static WidgetProperty m_SpriteProperty_One;
	private static WidgetProperty m_SpriteProperty_Two;
	private static WidgetProperty m_SpriteProperty_Three;

	private static WidgetProperty m_LabelProperty_One;
	private static WidgetProperty m_LabelProperty_Two;
	private static WidgetProperty m_LabelProperty_Three;

	//private List<string> m_ViewHeaders;
	private static UIQuickModifier MainWindow;
	private static string CurrentViewHeader;

	[MenuItem("Window/UIModifier/Label &s")]
	protected static void OpenLableModifier()
	{
		CurrentViewHeader = "Button";
		Initialize();
	}

	[MenuItem("Window/UIModifier/Button &l")]
	protected static void OpenButtonModifier()
	{
		CurrentViewHeader = "Label";
		Initialize();
	}

	protected static void Initialize()
	{
		m_SpriteProperty_One = new WidgetProperty("background", 1, 1, 1, -1, 140, 45);
		m_LabelProperty_One = new WidgetProperty("label", 1, 1, 1, -1, 140, 45);
		if (MainWindow == null)
			MainWindow = GetWindow<UIQuickModifier>("UIQuick Replace", true);
		MainWindow.Show();
		MainWindow.Repaint();
	}

	protected void OnEnable()
	{
		if (m_ViewItems == null)
			m_ViewItems = new Dictionary<string, System.Action>();
		m_ViewItems.Add("Label", OnLabelModifierShow);
		m_ViewItems.Add("Button", OnButtonModifierShow);
		//m_SpriteProperty_One = new WidgetProperty("background", 1, 1, 1, -1, 140, 45);
		//m_LabelProperty_One = new WidgetProperty("label", 1, 1, 1, -1, 140, 45);
	}

	protected void OnDisable()
	{
		if (m_ViewItems != null)
			m_ViewItems.Clear();
		m_ViewItems = null;
		m_SpriteProperty_One = null;
		m_LabelProperty_One = null; 
	}

	protected void OnGUI()
	{

		GUILayout.Space(20);
		//GUILayout.BeginVertical();
		string[] m_ViewHeaders = new string[m_ViewItems.Count];
		m_ViewItems.Keys.CopyTo(m_ViewHeaders, 0);
		CurrentViewHeader = DrawHeaderButtons(m_ViewHeaders, 60, CurrentViewHeader, 60);
		m_ViewItems[CurrentViewHeader]();
		
		//GUILayout.EndVertical();
	}
	
	private void OnButtonModifierShow()
	{
		GUILayout.Space(20);
		DrawWidget("StyleOne", m_SpriteProperty_One, UISprite.Type.Sliced);

		GUI.backgroundColor = Color.cyan;
		if (GUILayout.Button("Modify Button Sprite", GUILayout.MinHeight(60f)))
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

	private void OnLabelModifierShow()
	{
		GUILayout.Space(20);
		DrawWidget("Fuck", m_LabelProperty_One, UILabel.Effect.None);
	}

	private void DrawWidget(string header, WidgetProperty propertys, object type)
	{
		if (DrawContentHeader(header))
		{
			//SpriteProperty newSprite = new SpriteProperty();
			GUILayout.Space(10f);
			BeginContents(DefaultContentColor);
			#region Sprie Name
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name", GUILayout.Width(66f));
			propertys.Sprite_Name = GUILayout.TextField(propertys.Sprite_Name, GUILayout.MinWidth(40f));
			GUILayout.EndHorizontal();
			#endregion
			#region Sprie Type

			GUILayout.Space(3f);
			GUILayout.BeginHorizontal();
			EditorGUILayout.EnumPopup((Enum)(type));
			GUILayout.EndHorizontal();
			#endregion
			#region Pivot
			GUILayout.Space(3f);
			int newPivotH = propertys.Sprite_PivotH, newPivotV = propertys.Sprite_PivotV;
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
			int newDepth = EditorGUILayout.IntField("", /*m_Depth*/propertys.Sprite_Depth, GUILayout.MinWidth(50));
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
			int newBoundX = EditorGUILayout.IntField("Size", /*m_With*/propertys.Sprite_Width, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 12;
			int newBoundY = EditorGUILayout.IntField("x", /*m_Height*/propertys.Sprite_Height, GUILayout.MinWidth(30f));
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

	private bool DrawContentHeader(string title)
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

	private string DrawHeaderButtons(string[] texts, int padding, string selectionIndex, int minHeight = 20)
	{
		if (texts == null || texts.Length == 0)
			return string.Empty;
		string newSelect = selectionIndex;
		string style = null;
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		for (int index = 0; index < texts.Length; index++)
		{
			if (index == 0 && index != texts.Length - 1)
				style = "ButtonLeft";
			else if (index == texts.Length - 1 && index != 0)
				style = "ButtonRight";
			else
				style = "ButtonMid";
				
			if (GUILayout.Toggle(newSelect == texts[index], texts[index], style, GUILayout.MinHeight(minHeight)))
				newSelect = texts[index];
			if (newSelect != selectionIndex)
				selectionIndex = newSelect;
		}
		GUILayout.Space(padding);
		GUILayout.EndHorizontal();
		return newSelect;
	}

}

public class WidgetProperty
{
	public string Sprite_Name;
	public int Sprite_Type;
	public int Sprite_PivotH;
	public int Sprite_PivotV;
	public int Sprite_Depth;
	public int Sprite_Width;
	public int Sprite_Height;

	public WidgetProperty(string name = ""
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

	public void SetProperty(WidgetProperty content)
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
