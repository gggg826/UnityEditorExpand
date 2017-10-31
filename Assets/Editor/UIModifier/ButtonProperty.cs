/********************************************************************
*	created:	31/10/2017  11:01
*	filename: 	ButtonProperty
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class ButtonProperty
{
	public Action OnUpdateClick;
	public Action OnAllPropertiesChanged;

	public bool EnableColor;
	public Color Normal_Color;
	public Color Hover_Color = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
	public Color Pressed_Color = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
	public Color Disabled_Color = Color.grey;

	public bool EnableSprite;
	public UIAtlas Atlas;
	public string Normal_Sprite;
	public string Hover_Sprite;
	public string Pressed_Sprite;
	public string Disabled_Sprite;

	public UnityEngine.Sprite Normal_Sprite2D;
	public UnityEngine.Sprite Hover_Sprite2D;
	public UnityEngine.Sprite Pressed_Sprite2D;
	public UnityEngine.Sprite Disabled_Sprite2D;
	public bool pixelSnap = false;

	private bool state;
	private bool m_Color_Folder_State;
	private bool m_Sprite_Folder_State;
	private bool m_Sprite2D_Folder_State;
	private int m_CurrentSelectSpriteIndex;

	public void DrawProperty()
	{
		UIModifierUtils.BeginContents(UIModifierUtils.DefaultContentColor);
		EditorGUILayout.HelpBox("进行UI替换前先执行下面按钮升级UIImageButton组件", MessageType.Error);
		GUILayout.Space(3);
		Color temp = GUI.color;
		GUI.color = Color.cyan;
		if (GUILayout.Button("AUTO-UPGRADE UIIMAGEBUTTON"))
		{
			if (OnUpdateClick != null)
				OnUpdateClick();
		}

		GUILayout.Space(20f);
		if (GUILayout.Button("RESET CONDITIONS"))
		{
			Reset();
		}
		GUILayout.Space(20f);
		GUI.color = temp;



		state = UIModifierUtils.DrawContentHeader("UIButton", state);
		if (state)
		{
			GUILayout.Space(3);
			EditorGUIUtility.labelWidth = 100;
			EnableColor = EditorGUILayout.Toggle("EnableColor", EnableColor);
			EditorGUIUtility.labelWidth = 70;
			EditorGUI.BeginDisabledGroup(!EnableColor);
			m_Color_Folder_State = EditorGUILayout.Foldout(m_Color_Folder_State, "Colors", true);
			if (m_Color_Folder_State)
			{
				EditorGUI.indentLevel++;
				Normal_Color = EditorGUILayout.ColorField("Normal", Normal_Color);
				Hover_Color = EditorGUILayout.ColorField("Hover", Hover_Color);
				Pressed_Color = EditorGUILayout.ColorField("Pressed", Pressed_Color);
				Disabled_Color = EditorGUILayout.ColorField("Disabled", Disabled_Color);
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();

			GUILayout.Space(10);
			EditorGUIUtility.labelWidth = 100;
			EnableSprite = EditorGUILayout.Toggle("EnabelSprite", EnableSprite);
			EditorGUIUtility.labelWidth = 70;
			EditorGUI.BeginDisabledGroup(!EnableSprite);
			m_Sprite_Folder_State = EditorGUILayout.Foldout(m_Sprite_Folder_State, "Sprite", true);
			if (m_Sprite_Folder_State)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				if (GUILayout.Button("Atlas", "DropDown", GUILayout.Width(70)))
				{
					ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
				}
				EditorGUILayout.ObjectField(Atlas, typeof(UIAtlas), false);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Normal", GUILayout.Width(70));
				if (GUILayout.Button(Normal_Sprite, "DropDown", GUILayout.MinWidth(70)))
				{
					m_CurrentSelectSpriteIndex = 1;
					NGUISettings.atlas = Atlas;
					NGUISettings.selectedSprite = Normal_Sprite;
					SpriteSelector.Show(SelectSprite);
				}
				GUILayout.Space(18);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Hover", GUILayout.Width(70));
				if (GUILayout.Button(Hover_Sprite, "DropDown", GUILayout.MinWidth(70)))
				{
					m_CurrentSelectSpriteIndex = 2;
					NGUISettings.atlas = Atlas;
					NGUISettings.selectedSprite = Hover_Sprite;
					SpriteSelector.Show(SelectSprite);
				}
				GUILayout.Space(18);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Press", GUILayout.Width(70));
				if (GUILayout.Button(Pressed_Sprite, "DropDown", GUILayout.MinWidth(70)))
				{
					m_CurrentSelectSpriteIndex = 3;
					NGUISettings.atlas = Atlas;
					NGUISettings.selectedSprite = Pressed_Sprite;
					SpriteSelector.Show(SelectSprite);
				}
				GUILayout.Space(18);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Disabled", GUILayout.Width(70));
				if (GUILayout.Button(Disabled_Sprite, "DropDown", GUILayout.MinWidth(70)))
				{
					m_CurrentSelectSpriteIndex = 4;
					NGUISettings.atlas = Atlas;
					NGUISettings.selectedSprite = Disabled_Sprite;
					SpriteSelector.Show(SelectSprite);
				}
				GUILayout.Space(18);
				GUILayout.EndHorizontal();
			}
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(EnableSprite);
			m_Sprite2D_Folder_State = EditorGUILayout.Foldout(m_Sprite2D_Folder_State, "Sprite2D", true);
			if (m_Sprite2D_Folder_State)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Normal", GUILayout.Width(70));
				Normal_Sprite2D = (UnityEngine.Sprite)EditorGUILayout.ObjectField(Normal_Sprite2D, typeof(UnityEngine.Sprite), true);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Hover", GUILayout.Width(70));
				Hover_Sprite2D = (UnityEngine.Sprite)EditorGUILayout.ObjectField(Hover_Sprite2D, typeof(UnityEngine.Sprite), true);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Press", GUILayout.Width(70));
				Pressed_Sprite2D = (UnityEngine.Sprite)EditorGUILayout.ObjectField(Pressed_Sprite2D, typeof(UnityEngine.Sprite), true);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Disable", GUILayout.Width(70));
				Disabled_Sprite2D = (UnityEngine.Sprite)EditorGUILayout.ObjectField(Disabled_Sprite2D, typeof(UnityEngine.Sprite), true);
				GUILayout.EndHorizontal();
			}
			EditorGUI.EndDisabledGroup();
		}

		GUILayout.Space(3);
		GUI.color = Color.cyan;
		if (GUILayout.Button("Modify All Properties", GUILayout.MinHeight(300)))
		{
			if (OnAllPropertiesChanged != null)
				OnAllPropertiesChanged();
		}
		GUI.color = temp;
		UIModifierUtils.EndContents(UIModifierUtils.DefaultContentColor);
	}

	private void SelectSprite(string sprite)
	{
		if (m_CurrentSelectSpriteIndex < 1)
			return;

		switch(m_CurrentSelectSpriteIndex)
		{
			case 1:
				Normal_Sprite = sprite;
				break;
			case 2:
				Hover_Sprite = sprite;
				break;
			case 3:
				Pressed_Sprite = sprite;
				break;
			case 4:
				Disabled_Sprite = sprite;
				break;
			default:
				break;
		}
	}

	public void Reset()
	{
		EnableColor = false;
		Normal_Color = Color.white;
		Hover_Color = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);
		Pressed_Color = new Color(183f / 255f, 163f / 255f, 123f / 255f, 1f);
		Disabled_Color = Color.grey;

		EnableSprite = true;
		m_Sprite_Folder_State = true;
		Atlas = null;
		Normal_Sprite = string.Empty;
		Hover_Sprite = string.Empty;
		Pressed_Sprite = string.Empty;
		Disabled_Sprite = string.Empty;

		Normal_Sprite2D = null;
		Hover_Sprite2D = null;
		Pressed_Sprite2D = null;
		Disabled_Sprite2D = null;
	}

	private void OnSelectAtlas(UnityEngine.Object obj)
	{
		if (obj != null)
			Atlas = obj as UIAtlas;
	}
}
