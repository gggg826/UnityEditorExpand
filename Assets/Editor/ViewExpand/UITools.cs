using UnityEngine;
using System.Collections;
using UnityEditor;
public class UITools : EditorWindow
{
    //[InitializeOnLoad]  
    [MenuItem("Window/Change Font/Normal %l")]
    [MenuItem("Window/Change Sprite/Normal")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow<UITools>(true, "字体批量修改工具");

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
