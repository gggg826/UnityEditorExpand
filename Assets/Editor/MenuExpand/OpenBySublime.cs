/********************************************************************
*	created:	28/9/2017   1:13
*	filename: 	OpenBySublime
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;

public class OpenBySublime: Editor {

	[MenuItem("Assets/Open With SubLime", false, 100)]
	static void SVNUpdate()
	{
		ProcessCommand("subl", string.Format("\"{0}\"", GetTargetPath()));
	}

	private static void ProcessCommand(string command, string argument)
	{
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
		info.Arguments = argument;
		info.CreateNoWindow = false;
		info.ErrorDialog = true;
		info.UseShellExecute = true;

		if (info.UseShellExecute)
		{
			info.RedirectStandardOutput = false;
			info.RedirectStandardError = false;
			info.RedirectStandardInput = false;
		}
		else
		{
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;
			info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
			info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
		}

		System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

		if (!info.UseShellExecute)
		{
			Debug.Log(process.StandardOutput);
			Debug.Log(process.StandardError);
		}
	}

	private static string GetTargetPath()
	{
		string[] selectedGUIDs = Selection.assetGUIDs;
		if (selectedGUIDs.Length > 0 && !string.IsNullOrEmpty(selectedGUIDs[0]))
		{
			return Path.GetFullPath(AssetDatabase.GUIDToAssetPath(selectedGUIDs[0])).Replace("\\","/");
		}
		else
			return null;
	}
}
