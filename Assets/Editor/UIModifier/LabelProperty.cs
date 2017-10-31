/********************************************************************
*	created:	26/10/2017  13:09
*	filename: 	LabelProperty
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using UnityEngine;
using UnityEditor;
using System;

public class LabelProperty : WidgetProperty
{
	public UILabelInspector.FontType Font_Type;
	public UIFont UI_Font;
	//public Font Dyn_Font;
	public int Font_Size;
	public int Overflow_Width;
	public int Max_Lines;
	public FontStyle Font_Style;
	public UILabel.Overflow Overflow_Method;
	public UILabel.Modifier Modifier_Type;
	public UILabel.Crispness Crisp;
	public UILabel.Effect Effect_Type;
	public NGUIText.Alignment Alig_Type;
	public NGUIText.SymbolStyle Symbol_Style;
	public bool UseEllipsis;
	public bool Gradient;
	public bool BBCode;
	public Color Gradient_Top_Color;
	public Color Gradient_Bottom_Color;
	public Color Effect_Color;
	public Vector2 Effect_Distance;

	public override void SubReset()
	{
		Name = "Label";

		Font_Type             = UILabelInspector.FontType.NGUI;
		UI_Font               = null;
		//Dyn_Font              = null;
		Font_Size             = -1;
		Overflow_Width        = -1;
		Max_Lines             = -1;
		Font_Style            = FontStyle.Normal;
		Overflow_Method       = UILabel.Overflow.ResizeFreely;
		Crisp                 = UILabel.Crispness.Never;
		Modifier_Type         = UILabel.Modifier.None;
		Alig_Type             = NGUIText.Alignment.Automatic;
		Symbol_Style          = NGUIText.SymbolStyle.Normal;
		Effect_Type           = UILabel.Effect.None;
		UseEllipsis           = false;
		Gradient              = false;
		BBCode                = true;
		Gradient_Top_Color    = Color.white;
		Gradient_Bottom_Color = new Color(0.7f, 0.7f, 0.7f);
		Effect_Color          = Color.black;
		Effect_Distance       = new Vector2(-1, -1);
	}

	public override void DrawSubProperty()
	{
		state = UIModifierUtils.DrawContentHeader("UILabel", state);
		if(state)
		{
			#region Font
			EditorGUILayout.HelpBox("暂时只支持Dynamic Fonts替换", MessageType.Warning);
			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(true);
			Font_Type = (UILabelInspector.FontType)EditorGUILayout.EnumPopup(Font_Type, "DropDown", GUILayout.Width(74f));
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button("Font", "DropDown", GUILayout.Width(64f)))
			{
				ComponentSelector.Show<UIFont>(OnNGUIFont);
			}
			EditorGUILayout.ObjectField(UI_Font, typeof(UIFont), false);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Font Size", GUILayout.Width(66));
				Font_Size = EditorGUILayout.IntField("", Font_Size, GUILayout.Width(142f));
				Font_Style = (FontStyle)EditorGUILayout.EnumPopup(Font_Style);

				GUILayout.Space(18f);
			}
			GUILayout.EndHorizontal();
			#endregion


			GUILayout.BeginHorizontal();
			GUILayout.Label("Modifier", GUILayout.Width(66));
			Modifier_Type = (UILabel.Modifier)EditorGUILayout.EnumPopup(Modifier_Type);
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Alignment", GUILayout.Width(66));
			Alig_Type = (NGUIText.Alignment)EditorGUILayout.EnumPopup(Alig_Type);
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Effect", GUILayout.Width(66));
			Effect_Type = (UILabel.Effect)EditorGUILayout.EnumPopup(Effect_Type);
			if(Effect_Type != UILabel.Effect.None)
			{
				Effect_Color = EditorGUILayout.ColorField(Effect_Color, GUILayout.MinWidth(10f));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				{
					GUILayout.Label(" ", GUILayout.Width(66));
					EditorGUIUtility.labelWidth = 20f;
					Effect_Distance.x = EditorGUILayout.FloatField("X", Effect_Distance.x, GUILayout.MinWidth(40f));
					Effect_Distance.y = EditorGUILayout.FloatField("Y", Effect_Distance.y, GUILayout.MinWidth(40f));
					EditorGUIUtility.labelWidth = 70;
				}
			}
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Overflow", GUILayout.Width(66));
			Overflow_Method = (UILabel.Overflow)EditorGUILayout.EnumPopup(Overflow_Method);
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();
			//EditorGUI.indentLevel++;
			if (Overflow_Method == UILabel.Overflow.ClampContent)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(" ", GUILayout.Width(66));
				UseEllipsis = EditorGUILayout.ToggleLeft("Use Ellipsis", UseEllipsis);
				GUILayout.EndHorizontal();
			}
			else if (Overflow_Method == UILabel.Overflow.ResizeFreely)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(" ", GUILayout.Width(66));
				Overflow_Width = EditorGUILayout.IntField("Max Width", Overflow_Width, GUILayout.MinWidth(100));
				if (Overflow_Width < 1)
					GUILayout.Label("unlimited");
				GUILayout.Space(18f);
				GUILayout.EndHorizontal();
			}
			//EditorGUI.indentLevel--;

			

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
			
			GUILayout.BeginHorizontal();
			BBCode = EditorGUILayout.Toggle("BBCode", BBCode, GUILayout.Width(80));
			EditorGUI.BeginDisabledGroup(!BBCode);
			Symbol_Style = (NGUIText.SymbolStyle)EditorGUILayout.EnumPopup("Symbols", Symbol_Style);
			EditorGUI.EndDisabledGroup();
			GUILayout.Space(18);
			GUILayout.EndHorizontal();

			Max_Lines = EditorGUILayout.IntField("Max Lines", Max_Lines, GUILayout.Width(120));

			EditorGUILayout.HelpBox("依据官方文档，强制修改Keep Crisp为Never", MessageType.Warning);
			EditorGUI.BeginDisabledGroup(true);
			Crisp = UILabel.Crispness.Never;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Keep Crisp", GUILayout.Width(66));
			Crisp = (UILabel.Crispness)EditorGUILayout.EnumPopup(Crisp);
			GUILayout.Space(18f);
			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();

		}
	}

	private void OnNGUIFont(UnityEngine.Object obj)
	{
		if (obj != null)
			UI_Font = obj as UIFont;
	}
}

