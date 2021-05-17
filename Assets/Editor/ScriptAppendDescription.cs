
/********************************************************************
*	created:	28/7/2020   1:03
*	filename: 	ScriptAppendDescription
*	author:		Bing Lau
*	
*	purpose:	https://github.com/gggg826/UnityEditorExpand
*********************************************************************/
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

public class ScriptAppendDescription : UnityEditor.AssetModificationProcessor
{
    private static void OnWillCreateAsset(string path)
    {
        string AuthorName = SystemInfo.deviceName;
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string strContent = File.ReadAllText(path);
            if (strContent.Replace(" ", "").StartsWith("/***"))
            {
                return;
            }
            System.Text.StringBuilder strNote1 = new System.Text.StringBuilder();
            strNote1.Append("/********************************************************************\r\n");
            strNote1.AppendFormat("	created:	{0}\r\n", DateTime.Now.ToString());
            strNote1.AppendFormat("	file base:	{0}\r\n", path);
            strNote1.AppendFormat("	author:		{0}\r\n\r\n", AuthorName);
            strNote1.Append("	purpose:	\r\n");
            strNote1.Append("*********************************************************************/\r\n");

            string[] lines = File.ReadAllLines(path);
            string line = string.Empty;
            int i = 0;
            for (i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                if (line.Trim(' ').StartsWith("using "))
                {
                    strNote1.Append(line);
                    strNote1.Append("\r\n");
                    continue;
                }
                if(line.Trim(' ').Length == 0)
                {
                    strNote1.Append(line);
                    strNote1.Append("\r\n");
                    continue;
                }

                strNote1.Append("namespace ACE\r\n{\r\n");
                break;
            }
            for(; i < lines.Length; i++)
            {
                strNote1.Append("   ");
                strNote1.Append(lines[i]);
                strNote1.Append("\r\n");
            }
            strNote1.Append("}");
            File.WriteAllText(path, strNote1.ToString());
            AssetDatabase.Refresh();
        }
    }
}