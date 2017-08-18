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

    [MenuItem("Window/UIModifier/Label &l")]
    private static void ShowWindowSelectLabel()
    {
		m_ButtonSpriteSelected = false;
		if (!m_UIModifier)
			m_UIModifier = GetWindow<UIModifier>(true, "UI图文批量修改工具");
		m_UIModifier.Show();
    }

	[MenuItem("Window/UIModifier/Sprite &s")]
	private static void ShowWindowSelectSprite()
	{
		m_ButtonSpriteSelected = true;
		if (!m_UIModifier)
			m_UIModifier = GetWindow<UIModifier>(true, "UI图文批量修改工具");
		m_UIModifier.Show();
	}

	//默认字体
	UIFont toFont = null;
    //切换到的字体
    static UIFont toChangeFont;
    static Color color = new Color(225 / 255f, 237 / 255f, 161 / 255f); //ffeda1;
    static Color effecfColor = Color.gray;
    static UILabel.Effect effect = UILabel.Effect.None;
    static UILabel.Overflow ov = UILabel.Overflow.ResizeFreely;
    static string SPName = "";

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

			if (NGUISettings.minimalisticLook)
				NGUIEditorTools.SetLabelWidth(70f);

			DrawPivot();
			DrawDepth();
			DrawDimensions();
			if (NGUISettings.minimalisticLook) NGUIEditorTools.SetLabelWidth(70f);

			//SerializedProperty ratio = so.FindProperty("aspectRatio");
			//SerializedProperty aspect = so.FindProperty("keepAspectRatio");

			//GUILayout.BeginHorizontal();
			//{
			//	if (!aspect.hasMultipleDifferentValues && aspect.intValue == 0)
			//	{
			//		EditorGUI.BeginDisabledGroup(true);
			//		NGUIEditorTools.DrawProperty("Aspect", ratio, false, GUILayout.Width(130f));
			//		EditorGUI.EndDisabledGroup();
			//	}
			//	else NGUIEditorTools.DrawProperty("Aspect", ratio, false, GUILayout.Width(130f));

			//	NGUIEditorTools.DrawProperty("", aspect, false, GUILayout.MinWidth(20f));
			//}
			//GUILayout.EndHorizontal();
			NGUIEditorTools.EndContents();

		}
	}

	private void DrawDimensions()
	{

	}

	private void DrawDepth()
	{

		GUILayout.Space(2f);
		GUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel("Depth");

			if (GUILayout.Button("Back", GUILayout.MinWidth(46f)))
			{
				//foreach (GameObject go in Selection.gameObjects)
				//{
				//	UIWidget pw = go.GetComponent<UIWidget>();
				//	if (pw != null) pw.depth = w.depth - 1;
				//}
			}

			//NGUIEditorTools.DrawProperty("", so, "mDepth", GUILayout.MinWidth(20f));

			if (GUILayout.Button("Forward", GUILayout.MinWidth(60f)))
			{
				//foreach (GameObject go in Selection.gameObjects)
				//{
				//	//UIWidget pw = go.GetComponent<UIWidget>();
				//	//if (pw != null) pw.depth = w.depth + 1;
				//}
			}
		}
		GUILayout.EndHorizontal();
	}

	private bool l = false;
	private bool hc = true;
	private bool r = false;
	private bool t = false;
	private bool vc = true;
	private bool b = false;
	private void DrawPivot()
	{
		short h = 73;// 0000000001001001
		short v = 7;// 0000000000000111
		GUILayout.BeginHorizontal();
		GUILayout.Label("Pivot", GUILayout.Width(NGUISettings.minimalisticLook ? 66f : 76f));
		Toggle(ref l, "\u25C4", "ButtonLeft", UIWidget.Pivot.Left, h, 0);
		Toggle(ref hc, "\u25AC", "ButtonMid", UIWidget.Pivot.Center, h, 1);
		Toggle(ref r, "\u25BA", "ButtonRight", UIWidget.Pivot.Right, h, 2);
		Toggle(ref t, "\u25B2", "ButtonLeft", UIWidget.Pivot.Top, v, 0);
		Toggle(ref vc, "\u258C", "ButtonMid", UIWidget.Pivot.Center, v, 3);
		Toggle(ref b, "\u25BC", "ButtonRight", UIWidget.Pivot.Bottom, v, 6);

		GUILayout.EndHorizontal();
	}

	private void Toggle(ref bool bToggle, string text, string style, UIWidget.Pivot pivot, int value, int shift)
	{
		if (GUILayout.Toggle(bToggle, text, style) != bToggle)
		{
			//w.SetPivot(w, pivot, isHorizontal);
			//w.pivot = 
			l = false;
			hc = false;
			r = false;
			
			bToggle = !bToggle;
			value = value << shift;

			Debug.Log(style + " " + value);
		}
	}

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
