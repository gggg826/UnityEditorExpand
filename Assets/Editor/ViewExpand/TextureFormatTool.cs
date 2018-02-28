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

public class OverrideContence
{
	public BuildTarget TargetPlatform;
	public GUIContent OverrideContent;
	public bool IsOverride;

	public int MaxSize;

	public TextureImporterFormat DefaultFormat;
	public TextureImporterFormat NormalFormat;

	public OverrideContence(BuildTarget target, string tip, TextureImporterFormat defalutFormat, TextureImporterFormat normalFormat, bool isOverride = true)
	{
		TargetPlatform = target;
		OverrideContent = EditorGUIUtility.IconContent(GetPlatformSmallIcon(TargetPlatform), tip);
		DefaultFormat = defalutFormat;
		NormalFormat = normalFormat;

		MaxSize = 512;
		IsOverride = isOverride;
	}

	public static string GetPlatformSmallIcon(BuildTarget targetPlatform)
	{
		switch (targetPlatform)
		{
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "BuildSettings.Standalone.Small";

			case BuildTarget.iOS:
				return "BuildSettings.iPhone.Small";

			default:
				return string.Format("BuildSettings.{0}.Small", targetPlatform.ToString());
		}
	}
}

public class TextureFormatTool : EditorWindow
{
	protected static readonly string[] MAXTEXTURESIZESTRINGS = new string[]
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

	protected static readonly int[] MAXTEXTURESIZEVALUES = new int[]
	{
			-1,
			1,
			2,
			3,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192
	};

	protected TextureImporterFormat m_Textureforamt;
	protected TextureImporterNPOTScale m_NPOTS;
	protected bool m_Readable;
	protected bool m_sRGBTexture;
	protected bool m_MipmapEnabled;
	private List<string> m_QuickSetButtions;
	private string m_CurrentSelectedQuickSet;

	protected Dictionary<string, OverrideContence> m_PlatformOverrideContence;
	protected string m_CurrentSelectPlatform;

	[MenuItem("Window/Texture Format", false)]
	public static void OpenTextureFormatTool()
	{
		GetWindow<TextureFormatTool>("Texture Format Tool", true).Show();
	}

	#region GUI

	protected void OnGUI()
	{
		if (m_CurrentSelectedQuickSet == null)
		{
			Initialize();
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
			ChangeSelectedTextureFormatSettings();
		}
		GUI.color = temp;
	}

	protected void DrawQuickButton()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();

		string newQuickButton = CustomEditorUtils.DrawHeaderButtons(m_QuickSetButtions.ToArray(), 10, m_CurrentSelectedQuickSet, 20);
		if (newQuickButton != m_CurrentSelectedQuickSet)
		{
			m_CurrentSelectedQuickSet = newQuickButton;
			OnQuickButtonChanged();
		}
		GUILayout.Space(10);

		EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
		m_Readable = EditorGUILayout.ToggleLeft("Read/Write", m_Readable, new GUILayoutOption[0]);
		m_sRGBTexture = EditorGUILayout.ToggleLeft("sRGBTexture", m_sRGBTexture, new GUILayoutOption[0]);
		m_MipmapEnabled = EditorGUILayout.ToggleLeft("Mipmap", m_MipmapEnabled, new GUILayoutOption[0]);
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
		int platformCount = m_PlatformOverrideContence.Count;
		foreach (var item in m_PlatformOverrideContence)
		{
			Rect position;
			int startX = Mathf.RoundToInt((float)platformIndex * rect.width / (float)platformCount);
			int endX = Mathf.RoundToInt((float)(platformIndex + 1) * rect.width / (float)platformCount);
			position = new Rect(rect.x + (float)startX, rect.y, (float)(endX - startX), (float)height);
			if (GUI.Toggle(position, m_CurrentSelectPlatform == item.Key, item.Value.OverrideContent, toolbarButton))
			{
				m_CurrentSelectPlatform = item.Key;
			}
			platformIndex++;
		}

		GUILayoutUtility.GetRect(10f, (float)height);
		OverrideContence current = m_PlatformOverrideContence[m_CurrentSelectPlatform];
		string label = "Override for " + current.TargetPlatform;
		bool isOverride = EditorGUILayout.ToggleLeft(label, current.IsOverride, new GUILayoutOption[0]);
		if (isOverride != current.IsOverride)
		{
			current.IsOverride = isOverride;
		}

		EditorGUILayout.HelpBox("如果当前MaxSize为倍率压缩时，NormalMap不受影响(不改变)", MessageType.Info);
		current.MaxSize = EditorGUILayout.IntPopup("Max Size", current.MaxSize, MAXTEXTURESIZESTRINGS, MAXTEXTURESIZEVALUES, new GUILayoutOption[0]);
		current.DefaultFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Default Format", current.DefaultFormat, new GUILayoutOption[0]);
		current.NormalFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Normal Format", current.NormalFormat, new GUILayoutOption[0]);
		EditorGUILayout.EndVertical();

		GUILayout.Space(10);
		EditorGUILayout.EndHorizontal();
	}

	protected void OnQuickButtonChanged()
	{
		if (string.IsNullOrEmpty(m_CurrentSelectedQuickSet))
		{
			m_CurrentSelectedQuickSet = "Charactor";
		}
		switch (m_CurrentSelectedQuickSet)
		{
			case "UIAtlas":
				m_Readable = false;
				m_sRGBTexture = true;
				m_MipmapEnabled = false;
				m_NPOTS = TextureImporterNPOTScale.None;
				break;

			case "Charactor":
				m_Readable = false;
				m_sRGBTexture = true;
				m_MipmapEnabled = true;
				m_NPOTS = TextureImporterNPOTScale.ToNearest;
				break;

			case "OtherModel":
				m_Readable = false;
				m_sRGBTexture = true;
				m_MipmapEnabled = true;
				m_NPOTS = TextureImporterNPOTScale.ToNearest;
				break;

			default:
				break;
		}
	}

	protected void Initialize()
	{
		if (m_QuickSetButtions == null)
		{
			m_QuickSetButtions = new List<string>();
		}
		else
		{
			m_QuickSetButtions.Clear();
		}

		m_QuickSetButtions.Add("UIAtlas");
		m_QuickSetButtions.Add("Charactor");
		m_QuickSetButtions.Add("OtherModel");
		m_CurrentSelectedQuickSet = "Charactor";
		OnQuickButtonChanged();

		if (m_PlatformOverrideContence == null)
		{
			m_PlatformOverrideContence = new Dictionary<string, OverrideContence>();
		}
		else
		{
			m_PlatformOverrideContence.Clear();
		}
		m_PlatformOverrideContence.Add(BuildTarget.Android.ToString(), new OverrideContence(BuildTarget.Android, "Android", TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA));
		m_PlatformOverrideContence.Add(BuildTarget.iOS.ToString(), new OverrideContence(BuildTarget.iOS, "iOS", TextureImporterFormat.PVRTC_RGBA4, TextureImporterFormat.PVRTC_RGB4));
		m_CurrentSelectPlatform = BuildTarget.Android.ToString();
	}

	#endregion GUI

	#region Fuctions
	protected void ChangeSelectedTextureFormatSettings()
	{
		Object[] textures = GetSelectedTextures();

		Selection.objects = new Object[0];
		foreach (Texture2D texture in textures)
		{
			if(!ChangeTextureFormatSettings(texture))
			{
				continue;
			}
		}
	}

	protected bool ChangeTextureFormatSettings(Texture2D texture)
	{
		if(!texture)
		{
			return false;
		}

		try
		{
			string path = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.isReadable = false;
			textureImporter.sRGBTexture = true;
			textureImporter.mipmapEnabled = false;


			textureImporter.SaveAndReimport();
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
		return true;
	}

	protected void PlatformTextureSetting(TextureImporter textureImporter, string platfrom)
	{
		if(m_PlatformOverrideContence == null || !m_PlatformOverrideContence.ContainsKey(platfrom))
		{
			return;
		}

		OverrideContence targetPlatformFomat = m_PlatformOverrideContence[platfrom];
		TextureImporterPlatformSettings sourcePlatformImporterSettings = textureImporter.GetPlatformTextureSettings(platfrom);

		sourcePlatformImporterSettings.overridden = targetPlatformFomat.IsOverride;

		if (targetPlatformFomat.MaxSize <= 3 && targetPlatformFomat.MaxSize != -1)
		{
			sourcePlatformImporterSettings.maxTextureSize = GetLowLevelSize(textureImporter.maxTextureSize, targetPlatformFomat.MaxSize);
		}
		else if(targetPlatformFomat.MaxSize == -1)
		{
			sourcePlatformImporterSettings.maxTextureSize = targetPlatformFomat.MaxSize;
		}
		
		if (textureImporter.textureType == TextureImporterType.Default)
		{
			sourcePlatformImporterSettings.format = targetPlatformFomat.DefaultFormat;
			textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
		}
		else if (textureImporter.textureType == TextureImporterType.NormalMap)
		{
			sourcePlatformImporterSettings.format = targetPlatformFomat.NormalFormat;
			textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
		}
	}

	protected static Object[] GetSelectedTextures()
	{
		return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
	}

	protected int GetLowLevelSize(float size, int times)
	{
		int index = MAXTEXTURESIZEVALUES[MAXTEXTURESIZEVALUES.Length -1];
		for (int i = 0; i < MAXTEXTURESIZEVALUES.Length; i++)
		{
			if(size == MAXTEXTURESIZEVALUES[i])
			{
				index = i;
				break;
			}
		}

		index -= times;
		if(index < 4)
		{
			index = 4;
		}

		return MAXTEXTURESIZEVALUES[index];
	}

	#endregion Fuctions
}