/********************************************************************
	created:	2021/5/19 17:50:57
	file base:	Assets/Editor/RemoveLuaBom.cs
	author:		Bing Lau

	purpose:    去除 Lua 文件中 BOM 尾
*********************************************************************/

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class RemoveLuaBom
    {
        [MenuItem("Lua/Remove Lua BOM", false, 102)]
        public static void Remove()
        {
            var files = Directory.GetFiles(LuaConst.luaDir,
                "*.lua", SearchOption.AllDirectories);
            var processCount = 0;
            try
            {
                EditorUtility.DisplayProgressBar("running", "Remove Lua BOM", 0);
                var count = 0;
                var length = files.Length;

                foreach (var file in files)
                {
                    var bytes = File.ReadAllBytes(file);
                    //0xEF 0xBB 0xBF
                    if (bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
                    {
                        var copy = new byte[bytes.Length - 3];
                        Array.Copy(bytes, 3, copy, 0, copy.Length);
                        File.WriteAllBytes(file, copy);
                        ++processCount;
                    }
                    EditorUtility.DisplayProgressBar("running",
                        string.Format("Remove Lua BOM ... ({0}/{1})", ++count, length),
                        1f * count / length);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            Debug.Log("remove bom count: " + processCount);
        }
    }
}