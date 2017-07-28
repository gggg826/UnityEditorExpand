/********************************************************************
*	created:	28/7/2017   22:44
*	filename: 	EditorSVNHelper
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorSVNHelper : Editor
{
    [MenuItem("Assets/SVN/Update", false, 100)]
    static void SVNUpdate()
    {
        ProcessCommand("TortoiseProc.exe", "/command:update /path:" + GetTargetPath() + " /closeonend:0");
    }

    [MenuItem("Assets/SVN/Commit", false, 100)]
    static void SVNCommit()
    {
        ProcessCommand("TortoiseProc.exe", "/command:commit /path:" + GetTargetPath());
    }

    [MenuItem("Assets/SVN/Revert", false, 151)]
    static void SVNRevert()
    {
        ProcessCommand("TortoiseProc.exe", "/command:revert /path:" + GetTargetPath());
    }
    
    [MenuItem("Assets/SVN/CleanUp", false, 200)]
    static void SVNCleanUp()
    {
        ProcessCommand("TortoiseProc.exe", "/command:cleanup /path:" + GetTargetPath());
    }

    [MenuItem("Assets/SVN/Log", false, 200)]
    static void SVNLog()
    {
        ProcessCommand("TortoiseProc.exe", "/command:log /path:" + GetTargetPath());
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

        process.WaitForExit();
        process.Close();
    }

    private static string GetTargetPath()
    {
        // 只返回当前选择的文件/文件夹，若未选择则返回Assets目录 [7/28/2017 BingLau]
        string targetPath = System.IO.Path.GetFullPath(Application.dataPath);
        string[] selectedGUIDs = Selection.assetGUIDs;
        if (selectedGUIDs.Length > 0)
        {
            List<string> list = new List<string>();
            for (int GUIDIndex = 0; GUIDIndex < selectedGUIDs.Length; GUIDIndex++)
            {
                string path = Path.GetFullPath(AssetDatabase.GUIDToAssetPath(selectedGUIDs[GUIDIndex]));
                if (!string.IsNullOrEmpty(path))
                {
                    list.Add(path);
                    // 加入对应meta文件检测 [7/29/2017 BingLau]
                    list.Add(path + ".meta");
                }

            }
            targetPath = string.Join("*", list.ToArray());
        } 
        else
            targetPath = System.IO.Path.GetFullPath(Application.dataPath);
        return targetPath;

        // 默认路径是Unity工程目录下 Assets+ProjectSettings[7/28/2017 BingLau]
        //System.IO.DirectoryInfo root = System.IO.Directory.GetParent(Application.dataPath);
        //List<string> pathList = new List<string>();
        //pathList.Add(root + "/Assets");
        //pathList.Add(root + "/ProjectSettings");
        //return string.Join("*", pathList.ToArray());
    }
}
