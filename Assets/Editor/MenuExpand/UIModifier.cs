/********************************************************************
*	created:	18/8/2017   22:56
*	filename: 	UITools
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIModifier : EditorWindow
{
	private static EditorWindow m_UIModifier;
	private static bool m_ButtonSpriteSelected;
	private static UIWidget WidgetContents = new UIWidget();
	private int m_H_Select_Pivot = 1;
	private int m_V_Select_Pivot = 1;

	//默认字体
	UIFont toFont = null;
	//切换到的字体
	static UIFont toChangeFont;
	static Color color = new Color(225 / 255f, 237 / 255f, 161 / 255f); //ffeda1;
	static Color effecfColor = Color.gray;
	static UILabel.Effect effect = UILabel.Effect.None;
	static UILabel.Overflow ov = UILabel.Overflow.ResizeFreely;
	static string SPName = "";


	[MenuItem("Window/UIModifier/Label &l")]
	private static void ShowWindowSelectLabel()
	{
		m_ButtonSpriteSelected = false;
		if (!m_UIModifier)
			m_UIModifier = GetWindow(typeof(UIModifier));
		//m_UIModifier.titleContent = EditorGUIUtility.IconContent("lightMeter/greenLight");
		m_UIModifier.Show();
	}

	[MenuItem("Window/UIModifier/Sprite &s")]
	private static void ShowWindowSelectSprite()
	{
		m_ButtonSpriteSelected = true;
		if (!m_UIModifier)
			m_UIModifier = GetWindow(typeof(UIModifier));
		m_UIModifier.Show();
	}

	private void OnGUI()
	{
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Overflow", GUILayout.Width(70));
		ov = (UILabel.Overflow)EditorGUILayout.EnumPopup(ov);
		if (GUILayout.Button("修改"))
		{
			ChangeOverflow();
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Effect", GUILayout.Width(70));
		effect = (UILabel.Effect)EditorGUILayout.EnumPopup(effect);
		if (GUILayout.Button("修改"))
		{
			ChangeEffect();
			//ChangeSize();
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.Label("字体阴影颜色RGB值(在编辑器选中预设后批量修改)");
		color = EditorGUILayout.ColorField(color);
		if (GUILayout.Button("修改字体颜色"))
		{
			ChangeColor();
		}
		GUILayout.Space(10);
		GUILayout.Label("目标字体:");
		toFont = (UIFont)EditorGUILayout.ObjectField(toFont, typeof(UIFont), true, GUILayout.MinWidth(100f));
		toChangeFont = toFont;
		if (GUILayout.Button("修改字体大小(Unity4.6字体大小由动态字体决定)"))
		{
			ChangeSize();
		}
		GUILayout.Space(10);
		effecfColor = EditorGUILayout.ColorField(effecfColor);
		if (GUILayout.Button("修改带特效字体边框颜色"))
		{
			ChangeEffectColor();
		}

		GUILayout.Space(10);
		GUILayout.Label("Spriname");
		SPName = EditorGUILayout.TextField(SPName);
		if (GUILayout.Button("修改图片"))
		{
			ChangeSP();
		}

		if (NGUIEditorTools.DrawHeader("Widget"))
		{
			NGUIEditorTools.BeginContents();
			#region Pivot
			//DrawPivot(); 采用新算法 [8/20/2017 BingLau]
			GUILayout.Space(10);
			int newPivotH = m_H_Select_Pivot, newPivotV = m_V_Select_Pivot;
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
				WidgetContents.pivot = pivot;
			}
			#endregion
			#region DrawDepth
			//DrawDepth();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Depth", GUILayout.Width(66f));
			if (GUILayout.Button("Back", GUILayout.MinWidth(46f)))
			{
				depth -= 1;
			}

			GUILayout.Space(5);
			int newDepth = EditorGUILayout.IntField("", depth, GUILayout.MinWidth(50));
			if (newDepth != depth)
			{
				depth = newDepth;
			}
			GUILayout.Space(5);

			if (GUILayout.Button("Forward", GUILayout.MinWidth(60f)))
			{
				depth += 1;
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Change Depth"))
			{
				Debug.Log("Current Depth: " + depth);
			}
			#endregion
			#region DrawDimensions
			//DrawDimensions();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth = 70;
			int newBoundX = EditorGUILayout.IntField("Size", with, GUILayout.MinWidth(30f));
			EditorGUIUtility.labelWidth = 12;
			int newBoundY = EditorGUILayout.IntField("x", height, GUILayout.MinWidth(30f));
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
			NGUIEditorTools.EndContents();

		}
	}

	private int with = 100;
	private int height = 100;
	private void DrawDimensions()
	{
		GUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = 70;
		int w = EditorGUILayout.IntField("Size", with, GUILayout.MinWidth(30f));
		EditorGUIUtility.labelWidth = 12;
		int h = EditorGUILayout.IntField("x", height, GUILayout.MinWidth(30f));
		EditorGUIUtility.labelWidth = 70;
		if (GUILayout.Button("Snap", GUILayout.Width(60f)))
		{

		}
		GUILayout.EndHorizontal();
	}

	private static int depth = 0;
	private void DrawDepth()
	{

		
	}


	#region 旧的算法，采用移位，比较麻烦   [8/20/2017 BingLau]
	private short h = 146;// 0000000010010010
	private short v = 56;// 0000000000001110
	private bool l = false;
	private bool hc = true;
	private bool r = false;
	private bool t = false;
	private bool vc = true;
	private bool b = false;
	private void DrawPivot()
	{
		int h = m_H_Select_Pivot, v = m_V_Select_Pivot;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pivot", GUILayout.Width(NGUISettings.minimalisticLook ? 66f : 76f));
		Toggle(ref l, true, "\u25C4", "ButtonLeft", 0);       //UIWidget.Pivot.Left,
		Toggle(ref hc, true, "\u25AC", "ButtonMid", 1);   //UIWidget.Pivot.Center,
		Toggle(ref r, true, "\u25BA", "ButtonRight", 2);      //UIWidget.Pivot.Right,
		Toggle(ref t, false, "\u25B2", "ButtonLeft", 0);          //UIWidget.Pivot.Top,
		Toggle(ref vc, false, "\u258C", "ButtonMid", 3);     //UIWidget.Pivot.Center,
		Toggle(ref b, false, "\u25BC", "ButtonRight", 6);     //UIWidget.Pivot.Bottom,
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Change Pivot"))
		{
			Debug.Log(string.Format("h={0},v={1}", h, v));
			int count = 0;
			int value = h & v;
			Debug.Log(value);
			while (value != 1)
			{
				value = value >> 1;
				count++;
			}
			Debug.Log(value + "  " + count);
			Debug.Log(((UIWidget.Pivot)(count)).ToString());
		}
	}


	private void Toggle(ref bool bToggle, bool isH, string text, string style, int shift)
	{
		if (GUILayout.Toggle(bToggle, text, style) != bToggle)
		{
			if (isH)
			{
				l = false;
				hc = false;
				r = false;
				bToggle = !bToggle;
				h = (short)(73 << shift); //73 = 0000000001001001
			}
			else
			{
				t = false;
				vc = false;
				b = false;
				bToggle = !bToggle;
				v = (short)(7 << shift); //7 = 0000000000000111
			}
		}
	}
	#endregion


	private void ChangeEffect()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			item.effectStyle = effect;
			EditorUtility.SetDirty(item);
		}
	}

	private void ChangeOverflow()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			item.overflowMethod = ov;
			EditorUtility.SetDirty(item);
		}
	}


	public static void ChangeSize()
	{
		if (Selection.objects == null || Selection.objects.Length == 0)
			return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		if (!toChangeFont)
		{
			EditorUtility.DisplayDialog("", "先选择字体", "好");
			return;
		}
		foreach (UILabel item in labels)
		{
			UILabel label = (UILabel)item;
			label.font = toChangeFont;
			EditorUtility.SetDirty(label);
		}
	}
	public static void ChangeColor()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			//item.color = new Color(R / 255f, G / 255f, B / 255f);
			item.color = color;
			EditorUtility.SetDirty(item);
		}
	}
	public static void ChangeEffectColor()
	{
		//获取所有UILabel组件  
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			//item.effectColor = new Color(R / 255f, G / 255f, B / 255f);
			item.effectColor = effecfColor;
			EditorUtility.SetDirty(item);
		}
		//如果是UGUI讲UILabel换成Text就可以  
		//         Object[] labels = Selection.GetFiltered(typeof(Text), SelectionMode.Deep);
		//         foreach (Object item in labels)
		//         {
		//             //如果是UGUI讲UILabel换成Text就可以  
		//              Text label = (Text)item;
		//             label.font = toChangeFont;
		//             label.fontStyle = toChangeFontStyle;
		//label.font = toChangeFont;（UGUI）  
		//             Debug.Log(item.name + ":" + label.text);
		//  
		//            EditorUtility.SetDirty(item);//重要  
	}
	[MenuItem("Window/Change Font/EffectNone")]
	public static void ChangeFontEffect()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			item.effectStyle = UILabel.Effect.None;
			EditorUtility.SetDirty(item);
		}
	}
	[MenuItem("Window/Change Font/yellow")]
	public static void ChangeYellow()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			item.effectColor = new Color(128 / 255f, 75 / 255f, 0 / 255f);
			EditorUtility.SetDirty(item);
		}
	}
	[MenuItem("Window/Change Font/blue")]
	public static void ChangeBlue()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UILabel), SelectionMode.Deep);
		foreach (UILabel item in labels)
		{
			item.effectColor = new Color(16 / 255f, 136 / 255f, 212 / 255f);
			EditorUtility.SetDirty(item);
		}
	}
	[MenuItem("Window/Change Sprite/blue")]
	public static void ChangeSPBlue()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
		foreach (UISprite item in labels)
		{
			item.spriteName = "Btn3";
			item.type = UISprite.Type.Simple;
			item.MakePixelPerfect();
		}
	}
	[MenuItem("Window/Change Sprite/yellow")]
	public static void ChangeSPYellow()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
		foreach (UISprite item in labels)
		{
			item.spriteName = "BtnDown3";
			item.type = UISprite.Type.Simple;
			item.MakePixelPerfect();
		}
	}
	public static void ChangeSP()
	{
		if (Selection.objects == null || Selection.objects.Length == 0) return;
		Object[] labels = Selection.GetFiltered(typeof(UISprite), SelectionMode.Deep);
		foreach (UISprite item in labels)
		{
			item.spriteName = SPName;
			item.type = UISprite.Type.Simple;
			item.MakePixelPerfect();
		}
	}
}
