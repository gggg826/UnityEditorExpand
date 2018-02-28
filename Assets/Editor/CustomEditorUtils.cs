/********************************************************************
*	created:	26/10/2017   11:33
*	filename: 	UIModifierUtils
*	author:		Bing Lau
*
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/

using UnityEngine;

public class CustomEditorUtils
{
	//public static Color DefaultBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
	public static Color DefaultContentColor = new Color(1f, 1f, 1f, 0.7f);

	public static void BeginContents(Color contentsColor)
	{
		//EditorGUI.indentLevel++;
		GUILayout.Space(3f);
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUI.contentColor = contentsColor;
	}

	public static void EndContents(Color contentsColor)
	{
		GUI.contentColor = contentsColor;
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(3f);
		//EditorGUI.indentLevel--;
	}

	public static string DrawHeaderButtons(string[] texts, int padding, string selectionIndex, int minHeight = 20)
	{
		if (texts == null || texts.Length == 0)
			return string.Empty;
		string newSelect = selectionIndex;
		string style = null;
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		for (int index = 0; index < texts.Length; index++)
		{
			if (index == 0 && index != texts.Length - 1)
				style = "ButtonLeft";
			else if (index == texts.Length - 1 && index != 0)
				style = "ButtonRight";
			else
				style = "ButtonMid";

			if (GUILayout.Toggle(newSelect == texts[index], texts[index], style, GUILayout.MinHeight(minHeight)))
				newSelect = texts[index];
			if (newSelect != selectionIndex)
				selectionIndex = newSelect;
		}
		GUILayout.Space(padding);
		GUILayout.EndHorizontal();
		return newSelect;
	}

	public static bool DrawContentHeader(string title, bool state)
	{
		Color temp = GUI.color;
		bool isOn = state;
		if (isOn)
			GUI.color = Color.cyan;
		GUILayout.BeginHorizontal();
		GUI.changed = false;
		title = "<b><size=11>" + title + "</size></b>";
		if (isOn)
			title = "\u25BC " + title;
		else
			title = "\u25BA " + title;
		if (!GUILayout.Toggle(true, title, "dragtab", GUILayout.MinWidth(20f)))
			isOn = !isOn;
		GUILayout.EndHorizontal();
		if (GUI.changed)
			state = !state;
		GUI.color = temp;
		return isOn;
	}

	public static void ProcessCommand(string command, string argument, string workPath = "")
	{
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
		info.Arguments = argument;
		info.CreateNoWindow = false;
		info.ErrorDialog = true;
		info.UseShellExecute = true;
		if (!string.IsNullOrEmpty(workPath))
			info.WorkingDirectory = workPath;

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

		//process.WaitForExit();
		process.Close();
	}
}