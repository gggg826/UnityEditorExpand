/********************************************************************
*	created:	28/2/2018   22:46
*	filename: 	TextureFormatTool
*	author:		Bing Lau
*				批量修改图片导入格式
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/


//  ICONS
//list.Add(new BuildPlayerWindow.BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", BuildTargetGroup.Standalone, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("iOS", "BuildSettings.iPhone", BuildTargetGroup.iPhone, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("tvOS", "BuildSettings.tvOS", BuildTargetGroup.tvOS, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Android", "BuildSettings.Android", BuildTargetGroup.Android, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Tizen", "BuildSettings.Tizen", BuildTargetGroup.Tizen, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Xbox One", "BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("PS Vita", "BuildSettings.PSP2", BuildTargetGroup.PSP2, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("PS4", "BuildSettings.PS4", BuildTargetGroup.PS4, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Wii U", "BuildSettings.WiiU", BuildTargetGroup.WiiU, false));
//list.Add(new BuildPlayerWindow.BuildPlatform("Windows Store", "BuildSettings.Metro", BuildTargetGroup.WSA, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("WebGL", "BuildSettings.WebGL", BuildTargetGroup.WebGL, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, true));
//list.Add(new BuildPlayerWindow.BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", BuildTargetGroup.N3DS, false));

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TextureFormatTool : EditorWindow
{
	public static readonly string[] MAXTEXTURESIZESTRINGS = new string[]
		{
			"Don't Change",
			"Onece Low Level",
			"Twice Low Level",
			"thrice Low Level",
			"32",
			"64",
			"128",
			"256",
			"512",
			"1024",
			"2048",
			"4096",
			"8192"
		};

	protected TextureFormatData m_FormatData;

	public string CurrentSelectPlatform;


	[MenuItem("Window/Texture Format", false)]
	public static void OpenTextureFormatTool()
	{
		GetWindow<TextureFormatTool>("Texture Format Tool", true).Show();
	}

	#region GUI

	protected void OnGUI()
	{
		if(m_FormatData == null)
		{
			m_FormatData = new TextureFormatData();
			m_FormatData.Initialize();
		}

		if (string.IsNullOrEmpty(CurrentSelectPlatform))
		{
			CurrentSelectPlatform = BuildTarget.Android.ToString();
		}
		GUILayout.Space(20);
		DrawQuickButton();

		GUILayout.Space(30);
		DrawPlatformSettings();

		GUILayout.Space(20);
		Color temp = GUI.color;
		GUI.color = Color.cyan;
		if (GUILayout.Button("GO", GUILayout.MinHeight(20)))
		{
			m_FormatData.ChangeSelectedTextureFormatSettings(GetSelectedTextures(), m_FormatData.TargetImporterData);
		}
		GUI.color = temp;
	}

	protected void DrawQuickButton()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();

		string newQuickButton = CustomEditorUtils.DrawHeaderButtons(m_FormatData.QuickSetResolutions.ToArray(), 10, m_FormatData.CurrentSelectedQuickSetResolution, 20);
		if (newQuickButton != m_FormatData.CurrentSelectedQuickSetResolution)
		{
			m_FormatData.CurrentSelectedQuickSetResolution = newQuickButton;
			m_FormatData.OnQuickButtonChanged();
		}
		GUILayout.Space(10);

		EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
		m_FormatData.TargetImporterData.Readable = EditorGUILayout.ToggleLeft("Read/Write", m_FormatData.TargetImporterData.Readable, new GUILayoutOption[0]);
		m_FormatData.TargetImporterData.sRGBTexture = EditorGUILayout.ToggleLeft("sRGBTexture", m_FormatData.TargetImporterData.sRGBTexture, new GUILayoutOption[0]);
		m_FormatData.TargetImporterData.MipmapEnabled = EditorGUILayout.ToggleLeft("Mipmap", m_FormatData.TargetImporterData.MipmapEnabled, new GUILayoutOption[0]);
		m_FormatData.TargetImporterData.aTranparency = EditorGUILayout.ToggleLeft("a is Transparency", m_FormatData.TargetImporterData.aTranparency, new GUILayoutOption[0]);
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();

		GUILayout.Space(10);
		EditorGUILayout.EndHorizontal();
	}

	protected void DrawPlatformSettings()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);

		//GUI.changed = false;
		Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
		rect.width -= 1f;
		int height = 18;
		GUIStyle toolbarButton = EditorStyles.toolbarButton;

		int platformIndex = 0;
		int platformCount = m_FormatData.TargetImporterData.PlatformOverrideContences.Count;
		foreach (var item in m_FormatData.TargetImporterData.PlatformOverrideContences)
		{
			Rect position;
			int startX = Mathf.RoundToInt((float)platformIndex * rect.width / (float)platformCount);
			int endX = Mathf.RoundToInt((float)(platformIndex + 1) * rect.width / (float)platformCount);
			position = new Rect(rect.x + (float)startX, rect.y, (float)(endX - startX), (float)height);
			if (GUI.Toggle(position, CurrentSelectPlatform == item.Key, item.Value.OverrideContent, toolbarButton))
			{
				CurrentSelectPlatform = item.Key;
			}
			platformIndex++;
		}

		GUILayoutUtility.GetRect(10f, (float)height);
		PlatformOverrideContence current = m_FormatData.TargetImporterData.PlatformOverrideContences[CurrentSelectPlatform];
		string label = "Override for " + current.TargetPlatform;
		bool isOverride = EditorGUILayout.ToggleLeft(label, current.IsOverride, new GUILayoutOption[0]);
		if (isOverride != current.IsOverride)
		{
			current.IsOverride = isOverride;
		}

		EditorGUILayout.HelpBox("如果当前MaxSize为倍率压缩时，NormalMap不受影响(不改变)", MessageType.Info);
		current.MaxSize = EditorGUILayout.IntPopup("Max Size", current.MaxSize, MAXTEXTURESIZESTRINGS, TextureFormatData.MAXTEXTURESIZEVALUES, new GUILayoutOption[0]);
		current.DefaultFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Default Format", current.DefaultFormat, new GUILayoutOption[0]);
		current.NormalFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Normal Format", current.NormalFormat, new GUILayoutOption[0]);
		EditorGUILayout.EndVertical();

		GUILayout.Space(10);
		EditorGUILayout.EndHorizontal();
	}
	#endregion GUI
	
	protected static Object[] GetSelectedTextures()
	{
		return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
	}
}