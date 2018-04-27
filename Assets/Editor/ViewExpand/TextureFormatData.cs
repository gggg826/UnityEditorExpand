/********************************************************************
*	created:	27/4/2018   0:16
*	filename: 	TextureFormatData
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 各平台的压缩参数
/// </summary>
public class PlatformOverrideContence
{
	public string TargetPlatform;
	public GUIContent OverrideContent;
	public bool IsOverride;

	public int MaxSize;

	public TextureImporterFormat DefaultFormat;
	public TextureImporterFormat NormalFormat;

	public PlatformOverrideContence(string target, string tip, TextureImporterFormat defalutFormat, TextureImporterFormat normalFormat, bool isOverride = true)
	{
		TargetPlatform = target;
		OverrideContent = EditorGUIUtility.IconContent(GetPlatformSmallIcon(TargetPlatform), tip);
		DefaultFormat = defalutFormat;
		NormalFormat = normalFormat;

		MaxSize = 512;
		IsOverride = isOverride;
	}

	public PlatformOverrideContence()
	{
	}

	public static string GetPlatformSmallIcon(string targetPlatform)
	{
		switch (targetPlatform)
		{
			//case BuildTarget.StandaloneLinux:
			//case BuildTarget.StandaloneLinux64:
			//case BuildTarget.StandaloneLinuxUniversal:
			//case BuildTarget.StandaloneOSXIntel:
			//case BuildTarget.StandaloneOSXIntel64:
			//case BuildTarget.StandaloneOSXUniversal:
			//case BuildTarget.StandaloneWindows:
			//case BuildTarget.StandaloneWindows64:
			//	return "BuildSettings.Standalone.Small";

			case "iPhone":
			case "iOS":
				return "BuildSettings.iPhone.Small";

			default:
				return string.Format("BuildSettings.{0}.Small", targetPlatform.ToString());
		}
	}
}

public class TextureImporterData
{
	public TextureImporterFormat Textureforamt;
	public TextureImporterNPOTScale NPOTS;
	public bool Readable;
	public bool sRGBTexture;
	public bool MipmapEnabled;
	public bool aTranparency;
	public Dictionary<string, PlatformOverrideContence> PlatformOverrideContences;

	public TextureImporterData()
	{
		//if (PlatformOverrideContences == null)
		//{
		//	PlatformOverrideContences = new Dictionary<string, OverrideContence>();
		//}
		//else
		//{
		//	m_PlatformOverrideContence.Clear();
		//}

		PlatformOverrideContences = new Dictionary<string, PlatformOverrideContence>();

		PlatformOverrideContences.Add(/*BuildTarget.Android.ToString()*/"Standalone", new PlatformOverrideContence("Standalone", "Standalone", TextureImporterFormat.DXT5Crunched, TextureImporterFormat.DXT5, false));
		PlatformOverrideContences.Add(/*BuildTarget.Android.ToString()*/"Android", new PlatformOverrideContence("Android", "Android", TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ETC2_RGBA8));
		// 按官方文档解释来说，应该用iOS，然鹅Inspector面板显示iOS的压缩参数是读取YAML中"iPhone"项，为了安全，"iPhone" "iOS"项都进行修改 [10:50  1/3/2018  BingLau]
		PlatformOverrideContences.Add(/*BuildTarget.iOS.ToString()*/"iPhone", new PlatformOverrideContence("iPhone", "iPhone", TextureImporterFormat.PVRTC_RGBA4, TextureImporterFormat.PVRTC_RGBA4));
		PlatformOverrideContences.Add(/*BuildTarget.iOS.ToString()*/"iOS", new PlatformOverrideContence("iOS", "iOS", TextureImporterFormat.PVRTC_RGBA4, TextureImporterFormat.PVRTC_RGBA4));
	}
}

public class TextureFormatData
{
	public static readonly int[] MAXTEXTURESIZEVALUES = new int[]
	{
			-1,// 不改变图片最大Size
			1, // 最大Size向下降一级
			2, // 最大Size向下降二级
			3, // 最大Size向下降三级
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

	public TextureImporterData TargetImporterData;

	public List<string> QuickSetResolutions;
	public string CurrentSelectedQuickSetResolution;

	public void Initialize()
	{
		if (TargetImporterData == null)
		{
			TargetImporterData = new TextureImporterData();
		}
		if (QuickSetResolutions == null)
		{
			QuickSetResolutions = new List<string>();
		}
		else
		{
			QuickSetResolutions.Clear();
		}

		QuickSetResolutions.Add("UIAtlas");
		QuickSetResolutions.Add("Charactor");
		QuickSetResolutions.Add("OtherModel");
		CurrentSelectedQuickSetResolution = "UIAtlas";
		OnQuickButtonChanged();
	}

	#region Fuctions

	public void ChangeSelectedTextureFormatSettings(Object[] textures, TextureImporterData targetImpoterData)
	{
		if (textures == null)
		{
			return;
		}

		Selection.objects = new Object[0];
		foreach (Texture2D texture in textures)
		{
			if (!ChangeTextureFormatSettings(texture, targetImpoterData))
			{
				continue;
			}
		}
	}

	protected bool ChangeTextureFormatSettings(Texture2D texture, TextureImporterData targetImpoterData, bool isUsingUnityDeaultCompress = true)
	{
		if (!texture)
		{
			return false;
		}

		try
		{
			string path = AssetDatabase.GetAssetPath(texture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			//if(isUsingUnityDeaultCompress)
			//{
			//	if (targetImpoterData.PlatformOverrideContences != null)
			//	{
			//		foreach (var item in targetImpoterData.PlatformOverrideContences)
			//		{
			//			UnOverridePlatformTextureSetting(textureImporter, item.Key);
			//		}
			//	}
			//	textureImporter.SaveAndReimport();
			//}
			//return true;
			textureImporter.isReadable = targetImpoterData.Readable;
			textureImporter.sRGBTexture = targetImpoterData.sRGBTexture;
			textureImporter.mipmapEnabled = targetImpoterData.MipmapEnabled;
			textureImporter.alphaIsTransparency = targetImpoterData.aTranparency;

			if (targetImpoterData.PlatformOverrideContences != null)
			{
				foreach (var item in targetImpoterData.PlatformOverrideContences)
				{
					PlatformTextureSetting(textureImporter, item.Key, item.Value, isUsingUnityDeaultCompress);
				}
			}

			textureImporter.SaveAndReimport();
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
		return true;
	}

	protected void UnOverridePlatformTextureSetting(TextureImporter textureImporter, string platfrom)
	{
		TextureImporterPlatformSettings sourcePlatformImporterSettings = textureImporter.GetPlatformTextureSettings(platfrom);

		sourcePlatformImporterSettings.overridden = false;
		textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
	}

	protected void PlatformTextureSetting(TextureImporter textureImporter, string platfrom, PlatformOverrideContence targetPlatformFomat, bool isUsingUnityDefaultCompress)
	{
		//if (m_PlatformOverrideContence == null || !m_PlatformOverrideContence.ContainsKey(platfrom))
		//{
		//	return;
		//}

		//OverrideContence targetPlatformFomat = m_PlatformOverrideContence[platfrom];
		TextureImporterPlatformSettings sourcePlatformImporterSettings = textureImporter.GetPlatformTextureSettings(platfrom);

		sourcePlatformImporterSettings.overridden = targetPlatformFomat.IsOverride;

		if (targetPlatformFomat.MaxSize <= 3 && targetPlatformFomat.MaxSize != -1)
		{
			sourcePlatformImporterSettings.maxTextureSize = GetLowLevelSize(textureImporter.maxTextureSize, targetPlatformFomat.MaxSize);
		}
		else if (targetPlatformFomat.MaxSize != -1)
		{
			sourcePlatformImporterSettings.maxTextureSize = targetPlatformFomat.MaxSize;
		}

		//if(isUsingUnityDefaultCompress)
		//{
		//	textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
		//	return;
		//}

		if (textureImporter.textureType == TextureImporterType.Default)
		{
			// 优化策略，只修改不是ETC或者PVRTC压缩格式的图为指定的压缩格式 [11:49  27/4/2018  BingLau]
			if (CheckNeedChangeFormat(sourcePlatformImporterSettings.format))
			{
				sourcePlatformImporterSettings.format = targetPlatformFomat.DefaultFormat;
			}
			textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
		}
		else if (textureImporter.textureType == TextureImporterType.NormalMap)
		{
			if (CheckNeedChangeFormat(sourcePlatformImporterSettings.format))
			{
				sourcePlatformImporterSettings.format = targetPlatformFomat.NormalFormat;
			}
			textureImporter.SetPlatformTextureSettings(sourcePlatformImporterSettings);
		}
	}

	protected bool CheckNeedChangeFormat(TextureImporterFormat format)
	{
		string strFormat = format.ToString();
		return !strFormat.Contains("ETC") && !strFormat.Contains("PVRTC") && !strFormat.Contains("AutomaticCompressed");
	}

	protected int GetLowLevelSize(float size, int times)
	{
		int index = MAXTEXTURESIZEVALUES[MAXTEXTURESIZEVALUES.Length - 1];
		for (int i = 0; i < MAXTEXTURESIZEVALUES.Length; i++)
		{
			if (size == MAXTEXTURESIZEVALUES[i])
			{
				index = i;
				break;
			}
		}

		index -= times;
		if (index < 4)
		{
			index = 4;
		}

		return MAXTEXTURESIZEVALUES[index];
	}

	public void OnQuickButtonChanged(string currentSelectedQuickSetResolution = "UIAtlas")
	{
		//if (string.IsNullOrEmpty(m_CurrentSelectedQuickSet))
		//{
		//	m_CurrentSelectedQuickSet = "Charactor";
		//}
		switch (currentSelectedQuickSetResolution)
		{
			case "UIAtlas":
				TargetImporterData.Readable = false;
				TargetImporterData.sRGBTexture = true;
				TargetImporterData.MipmapEnabled = false;
				TargetImporterData.aTranparency = true;
				TargetImporterData.NPOTS = TextureImporterNPOTScale.ToNearest;
				if (TargetImporterData.PlatformOverrideContences != null)
				{
					foreach (var item in TargetImporterData.PlatformOverrideContences)
					{
						item.Value.MaxSize = -1;
					}
				}
				break;

			case "Charactor":
				TargetImporterData.Readable = false;
				TargetImporterData.sRGBTexture = true;
				TargetImporterData.MipmapEnabled = true;
				TargetImporterData.aTranparency = true;
				TargetImporterData.NPOTS = TextureImporterNPOTScale.ToNearest;
				if (TargetImporterData.PlatformOverrideContences != null)
				{
					foreach (var item in TargetImporterData.PlatformOverrideContences)
					{
						item.Value.MaxSize = 512;
					}
				}
				break;

			case "OtherModel":
				TargetImporterData.Readable = false;
				TargetImporterData.sRGBTexture = true;
				TargetImporterData.MipmapEnabled = true;
				TargetImporterData.aTranparency = true;
				TargetImporterData.NPOTS = TextureImporterNPOTScale.ToNearest;
				if (TargetImporterData.PlatformOverrideContences != null)
				{
					foreach (var item in TargetImporterData.PlatformOverrideContences)
					{
						item.Value.MaxSize = 512;
					}
				}
				break;

			default:
				break;
		}
	}

	#endregion Fuctions
}