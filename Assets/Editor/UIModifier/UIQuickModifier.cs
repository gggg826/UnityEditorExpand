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
	//private List<string> m_ViewButtons;
	private Dictionary<string, System.Action> m_ViewItems;

	private static WidgetProperty m_LabelProperty;
	private static WidgetProperty m_SpriteProperty;
	private static ButtonProperty m_ButtonProperty;

	//private List<string> m_ViewHeaders;
	private static UIQuickModifier MainWindow;
	private static string CurrentViewHeader;

	[MenuItem("Window/UIModifier &m")]
	public static void OpenLableModifier()
	{
		Initialize();
	}

	//[MenuItem("Window/UIModifier/Button &l")]
	//protected static void OpenButtonModifier()
	//{
	//	CurrentViewHeader = "Label";
	//	Initialize();
	//}

	public static void Initialize()
	{
		CurrentViewHeader = "Button";

		m_LabelProperty = new LabelProperty();
		m_LabelProperty.Reset();
		m_LabelProperty.OnSnap = OnSnapEvent;
		m_LabelProperty.OnAllPropertiesChanged = OnChangeAllEvent;
		m_SpriteProperty = new SpriteProperty();
		m_SpriteProperty.Reset();
		m_SpriteProperty.OnSnap = OnSnapEvent;
		m_SpriteProperty.OnAllPropertiesChanged = OnChangeAllEvent;

		m_ButtonProperty = new ButtonProperty();
		m_ButtonProperty.Reset();
		m_ButtonProperty.OnAllPropertiesChanged = OnChangeAllEvent;
		m_ButtonProperty.OnUpdateClick = OnButtonUpdateEvent;


		if (MainWindow == null)
			MainWindow = GetWindow<UIQuickModifier>("UIQuick Replace", true);
		MainWindow.Show();
		MainWindow.Repaint();
	}

	public void OnEnable()
	{
		if (m_ViewItems == null)
			m_ViewItems = new Dictionary<string, System.Action>();
		m_ViewItems.Add("Label", OnLabelModifierShow);
		m_ViewItems.Add("Sprite", OnSpriteModifierShow);
		m_ViewItems.Add("Button", OnButtonModifierShow);
	}

	public void OnDisable()
	{
		//if (m_ViewItems != null)
		//	m_ViewItems.Clear();
		//m_ViewItems = null;
		//m_LabelProperty = null;
		//m_ButtonProperty = null;
	}

	protected void OnGUI()
	{
		GUILayout.Space(20);
		string[] m_ViewHeaders = new string[m_ViewItems.Count];
		m_ViewItems.Keys.CopyTo(m_ViewHeaders, 0);
		if (CurrentViewHeader == null)
			Initialize();
		CurrentViewHeader = UIModifierUtils.DrawHeaderButtons(m_ViewHeaders, 60, CurrentViewHeader, 60);
		m_ViewItems[CurrentViewHeader]();
	}

	private void OnButtonModifierShow()
	{
		GUILayout.Space(20);
		m_ButtonProperty.DrawProperty();
	}

	private void OnSpriteModifierShow()
	{
		GUILayout.Space(20);
		m_SpriteProperty.DrawProperty();
	}

	private void OnLabelModifierShow()
	{
		GUILayout.Space(20);
		m_LabelProperty.DrawProperty();
	}

	static private void OnChangeAllEvent()
	{
		if (Selection.objects == null || Selection.objects.Length == 0)
			return;
		
		int count = 0;
		if (CurrentViewHeader == "Label")
		{
			UnityEngine.Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
			foreach (UILabel uiLabel in labels)
			{
				if (UIModifierHelper.ChangeAllProperties(uiLabel, m_LabelProperty))
					count++;
			}
		}
		else if (CurrentViewHeader == "Sprite")
		{
			UnityEngine.Object[] sprites = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
			foreach (UISprite uiSprite in sprites)
			{
				if (UIModifierHelper.ChangeAllProperties(uiSprite, m_SpriteProperty))
					count++;
			}
		}
		else if(CurrentViewHeader == "Button")
		{
			UnityEngine.Object[] buttons = Selection.GetFiltered(typeof(UIButton), SelectionMode.Deep);
			foreach (UIButton uiButton in buttons)
			{
				if (UIModifierHelper.ChangeButtonProperties(uiButton, m_ButtonProperty))
					count++;
			}
		}

		//EditorUtility.DisplayDialog("", "Completed!!\r\nTotal: " + count, "OK");
		Debug.Log("Total modify count:  " + count);
	}

	private static void OnSnapEvent()
	{
		if (CurrentViewHeader != "Sprite")
		{
			EditorUtility.DisplayDialog("Error", "Operation is allowed only in UISprite!!", "OK");
			return;
		}

		if (Selection.objects == null || Selection.objects.Length == 0)
			return;

		int count = 0;
		UnityEngine.Object[] sprites = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
		foreach (UISprite uiSprite in sprites)
		{
			if (UIModifierHelper.OnSnap(uiSprite, m_SpriteProperty))
				count++;
		}

		//EditorUtility.DisplayDialog("", "Completed!!\r\nTotal: " + count, "OK");
		Debug.Log("Total modify count:  " + count);
	}
	
	private static void OnButtonUpdateEvent()
	{
		if (Selection.objects == null || Selection.objects.Length == 0)
			return;

		int count = 0;
		UnityEngine.Object[] buttons = Selection.GetFiltered(typeof(UIImageButton), SelectionMode.Deep);
		foreach (UIImageButton uiImageButton in buttons)
		{
			if (UIModifierHelper.UpgradeUIImageButton(uiImageButton))
				count++;
		}

		//EditorUtility.DisplayDialog("", "Completed!!\r\nTotal: " + count, "OK");
		Debug.Log("Total modify count:  " + count);
	}
}
