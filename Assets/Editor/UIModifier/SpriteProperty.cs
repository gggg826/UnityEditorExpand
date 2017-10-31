/********************************************************************
*	created:	26/10/2017  13:12
*	filename: 	SpriteProperty
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SpriteProperty : WidgetProperty
{
	public UIAtlas Atlas;
	public string Sprite_Name;
	public UIBasicSprite.Type Sprite_Type;
	public UIBasicSprite.Flip Flip_Type;
	public UIBasicSprite.FillDirection FillDirection_Type;
	public UIBasicSprite.AdvancedType Center_Type;
	public UIBasicSprite.AdvancedType Left_Type;
	public UIBasicSprite.AdvancedType Right_Type;
	public UIBasicSprite.AdvancedType Bottom_Type;
	public UIBasicSprite.AdvancedType Top_Type;
	public float Fill_Amount;
	public bool Invert;
	public bool Fill_Center;
	public bool Gradient;
	public Color Gradient_Top_Color;
	public Color Gradient_Bottom_Color;

	public override void SubReset()
	{
		Name = "Background";
		Width = 140;
		Height = 45;

		Sprite_Type = UIBasicSprite.Type.Sliced;
		Flip_Type = UIBasicSprite.Flip.Nothing;
		FillDirection_Type = UIBasicSprite.FillDirection.Horizontal;
		Center_Type = UIBasicSprite.AdvancedType.Sliced;
		Left_Type = UIBasicSprite.AdvancedType.Sliced;
		Right_Type = UIBasicSprite.AdvancedType.Sliced;
		Bottom_Type = UIBasicSprite.AdvancedType.Sliced;
		Top_Type = UIBasicSprite.AdvancedType.Sliced;
		Gradient = false;
		Gradient_Top_Color = Color.white;
		Gradient_Bottom_Color = new Color(0.7f, 0.7f, 0.7f);
	}

	public override void DrawSubProperty()
	{
		state = UIModifierUtils.DrawContentHeader("UISprite", state);
		if (state)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Atlas", "DropDown", GUILayout.Width(70)))
			{
				ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
			}
			EditorGUILayout.ObjectField(Atlas, typeof(UIFont), false);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Sprite", "DropDown", GUILayout.Width(70)))
			{
				NGUISettings.atlas = Atlas;
				NGUISettings.selectedSprite = Sprite_Name;
				SpriteSelector.Show(SelectSprite);
			}
			GUILayout.Label(Sprite_Name, "HelpBox", GUILayout.Height(18f));
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			{
				Flip_Type = (UIBasicSprite.Flip)EditorGUILayout.EnumPopup("Flip", Flip_Type);

				Sprite_Type = (UIBasicSprite.Type)EditorGUILayout.EnumPopup("Sprite Type", Sprite_Type);

				EditorGUI.indentLevel++;
				{
					if (Sprite_Type == UISprite.Type.Simple)
					{
						GUILayout.BeginHorizontal();
						Gradient = EditorGUILayout.Toggle("Gradient", Gradient, GUILayout.Width(80));
						EditorGUI.BeginDisabledGroup(!Gradient);
						{
							EditorGUIUtility.labelWidth = 40;
							Gradient_Top_Color = EditorGUILayout.ColorField("Top", Gradient_Top_Color, GUILayout.MinWidth(60));
							GUILayout.EndHorizontal();

							GUILayout.BeginHorizontal();
							GUILayout.Label(" ", GUILayout.Width(80));
							Gradient_Bottom_Color = EditorGUILayout.ColorField("Bottom", Gradient_Bottom_Color, GUILayout.MinWidth(60));
							EditorGUIUtility.labelWidth = 70;
						}
						EditorGUI.EndDisabledGroup();
						GUILayout.EndHorizontal();
					}
					else if(Sprite_Type == UISprite.Type.Tiled)
					{

					}
					else if (Sprite_Type == UISprite.Type.Sliced)
					{
						Fill_Center = EditorGUILayout.Toggle("Fill Center", Fill_Center);

						GUILayout.BeginHorizontal();
						Gradient = EditorGUILayout.Toggle("Gradient", Gradient, GUILayout.Width(80));
						EditorGUI.BeginDisabledGroup(!Gradient);
						{
							Gradient_Top_Color = EditorGUILayout.ColorField("Top", Gradient_Top_Color, GUILayout.MinWidth(60));
							GUILayout.EndHorizontal();

							GUILayout.BeginHorizontal();
							GUILayout.Label(" ", GUILayout.Width(80));
							Gradient_Bottom_Color = EditorGUILayout.ColorField("Bottom", Gradient_Bottom_Color, GUILayout.MinWidth(60));
						}
						EditorGUI.EndDisabledGroup();
						GUILayout.EndHorizontal();
					}
					else if (Sprite_Type == UISprite.Type.Filled)
					{
						FillDirection_Type = (UIBasicSprite.FillDirection)EditorGUILayout.EnumPopup("Flip", FillDirection_Type, GUILayout.MinWidth(20f));
						Fill_Amount = EditorGUILayout.Slider("Amount", Fill_Amount, 0, 1f, GUILayout.MinWidth(20f));
						Invert = EditorGUILayout.Toggle("Invert", Invert);
					}
					else if (Sprite_Type == UISprite.Type.Advanced)
					{
						Left_Type = (UIBasicSprite.AdvancedType)EditorGUILayout.EnumPopup("Left", Left_Type);
						Right_Type = (UIBasicSprite.AdvancedType)EditorGUILayout.EnumPopup("Right", Right_Type);
						Bottom_Type = (UIBasicSprite.AdvancedType)EditorGUILayout.EnumPopup("Bottom", Bottom_Type);
						Top_Type = (UIBasicSprite.AdvancedType)EditorGUILayout.EnumPopup("Top", Top_Type);
						Center_Type = (UIBasicSprite.AdvancedType)EditorGUILayout.EnumPopup("Center", Center_Type); ;
					}
				}
				EditorGUI.indentLevel--;
			}
			GUILayout.EndVertical();
			GUILayout.Space(18);
			GUILayout.EndHorizontal();
		}
	}

	private void SelectSprite(string sprite)
	{
		if (!string.IsNullOrEmpty(sprite))
			Sprite_Name = sprite;
	}

	private void OnSelectAtlas(UnityEngine.Object obj)
	{
		if (obj != null)
			Atlas = obj as UIAtlas;
	}
}
