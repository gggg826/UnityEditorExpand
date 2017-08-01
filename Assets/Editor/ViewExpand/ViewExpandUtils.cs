/********************************************************************
*	created:	28/7/2017   0:08
*	filename: 	EditorUtils
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ViewExpandUtils
{
    /// <summary>
    /// 添加指定值的空白区域
    /// </summary>
    /// <param name="list"></param>
    /// <param name="value"></param>
    public static void AddSpace(ref List<EditorViewItem> list, float value)
    {
        EditorViewItem item = new EditorViewItem();
        list.Add(item);
        item.ItemType = EditorViewItem.Type.Space;
        item.BlockValue = value;
    }

    /// <summary>
    /// 添加值为-1的空白区域
    /// </summary>
    /// <param name="list"></param>
    public static void AddSpace(ref List<EditorViewItem> list)
    {
        AddSpace(ref list, -1);
    }

    /// <summary>
    /// 添加可调整的空白区域
    /// </summary>
    /// <param name="list"></param>
    public static void AddFlexibleSpace(ref List<EditorViewItem> list)
    {
        EditorViewItem item = new EditorViewItem();
        list.Add(item);
        item.ItemType = EditorViewItem.Type.FlexibleSpace;
    }

    /// <summary>
    /// 添加按钮
    /// </summary>
    /// <param name="list"></param>
    /// <param name="text"></param>
    /// <param name="tooltip"></param>
    /// <param name="onButtonClick"></param>
    /// <param name="options"></param>
    public static void AddPushButton(ref List<EditorViewItem> list, string text, string tooltip, System.Action onButtonClick, params GUILayoutOption[] options)
    {
        EditorViewItem item = new EditorViewItem();
        list.Add(item);
        item.ItemType = EditorViewItem.Type.PushButton;
        item.Content = new GUIContent(text, tooltip);
        item.OnButtonClick = onButtonClick;
        item.LayoutOptions = options;
    }

	public static void AddToggleButton(ref List<EditorViewItem> list, string text, string tooltip, System.Action<bool> onToggleChanged, params GUILayoutOption[] options)
	{
		EditorViewItem item = new EditorViewItem();
		list.Add(item);
		item.ItemType = EditorViewItem.Type.ToggleButton;
		item.Content = new GUIContent(text, tooltip);
		item.OnToggleChanged = onToggleChanged;
		item.LayoutOptions = options;
	}

	/// <summary>
	/// 添加自定义区域
	/// </summary>
	/// <param name="list"></param>
	/// <param name="onClickEvent"></param>
	public static void AddCustom(ref List<EditorViewItem> list, System.Action onDrow)
    {
        EditorViewItem item = new EditorViewItem();
        list.Add(item);
        item.ItemType = EditorViewItem.Type.Custom;
        item.OnCustomDraw = onDrow;
    }
}

public class EditorViewItem
{
    public enum Type
    {
        PushButton,
		ToggleButton,
        Space,
        FlexibleSpace,
        Custom,
    }

    public float BlockValue;
	public bool Toggled;
    public Type ItemType;
    public GUILayoutOption[] LayoutOptions;
    public GUIContent Content;
    public System.Action OnButtonClick;
    public System.Action OnCustomDraw;
	public System.Action<bool> OnToggleChanged;

    public void Draw()
    {
        switch (ItemType)
        {
            case Type.PushButton:
                if (GUILayout.Button(Content, EditorStyles.toolbarButton, LayoutOptions))
                    OnButtonClick();
                break;
			case Type.ToggleButton:
				GUIStyle normalStyle = EditorStyles.toolbarButton;
				GUIStyle toggledStyle = new GUIStyle(normalStyle);
				toggledStyle.normal.background = toggledStyle.onActive.background;
				if (GUILayout.Button(Content, Toggled ? toggledStyle : normalStyle, LayoutOptions))
				{
					Toggled = !Toggled;
					OnToggleChanged(Toggled);
				}
				break; 
            case Type.Space:
                if (BlockValue > 0)
                    GUILayout.Space(BlockValue);
                else
                    EditorGUILayout.Space();
                break;
            case Type.FlexibleSpace:
                GUILayout.FlexibleSpace();
                break;
            case Type.Custom:
                if (null != OnCustomDraw)
                    OnCustomDraw();
                break;
        }
    }
}


