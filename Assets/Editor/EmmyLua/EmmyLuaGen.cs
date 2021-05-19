/********************************************************************
	created:	2021/5/18 10:28:16
	file base:	Assets/Editor/EmmyLuaGen.cs
	author:		Bing Lau

	purpose:	自动生成 emmylua 注释
*********************************************************************/

using ACE;
using DG.Tweening;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using BindType = ToLuaMenu.BindType;
using Random = System.Random;

namespace EditorTool
{
    public class EmmyLuaGen
    {
        private HashSet<Type> m_IgnoreType = new HashSet<Type>() {
            typeof(void*), 
            typeof(Type), 
            typeof(MarshalByRefObject),
            typeof(LuaStatePtr), 
            typeof(Assembly),
            typeof(LuaTable), 
            typeof(MethodInfo),
            typeof(object), 
            typeof(Animation), 
            typeof(Random), 
            typeof(Font),
            typeof(ValueType), 
            typeof(RenderTextureDescriptor), 
            typeof(PhysicsRaycaster),
            typeof(PlayableGraph), 
            typeof(ShortcutExtensions), 
            typeof(CommandBuffer),
            typeof(CustomYieldInstruction), 
            typeof(StateMachineBehaviour),
        };

        private HashSet<Type> m_SimpleTypes = new HashSet<Type>() {
            typeof(List<>), 
            typeof(Dictionary<,>), 
            typeof(IEnumerable<>),
            //typeof(ExposedList<>),
        };

        private HashSet<string> m_ClassTypes = new HashSet<string>() {
            "T", 
            "TKey", 
            "TValue",
        };

        private HashSet<string> m_ListStr = new HashSet<string>(){
            "and", 
            "break", 
            "do", 
            "else", 
            "elseif", 
            "end", 
            "false", 
            "for",
            "function", 
            "if", 
            "in", 
            "local", 
            "nil", 
            "not", 
            "or", 
            "repeat", 
            "return", 
            "then", 
            "true", 
            "until", 
            "while"};

        private HashSet<BindType> m_Types = new HashSet<BindType>();

        private List<Type> m_AddTypes = new List<Type>();
        private HashSet<Type> m_HasTypes = new HashSet<Type>();
        private Dictionary<string, Type> m_GenType = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_DelegateTypes = new Dictionary<string, Type>();
        private Dictionary<string, HashSet<Type>> m_AllNamespace = new Dictionary<string, HashSet<Type>>();


        private StringBuilderPro m_SB = new StringBuilderPro();
        private Encoding m_UTF8WithoutBom = new UTF8Encoding(false);

        [MenuItem("Lua/Gen emmylua", false, 102)]
        public static void Execute()
        {
            new EmmyLuaGen().Start();
        }

        private void Start()
        {
            Collect();
            Export();
            var path = LuaConst.luaDir + "/Gen/emmylua/emmyluaAPI.lua";
            File.WriteAllText(path, m_SB.ToString(), m_UTF8WithoutBom);
            Debug.Log("Gen ememylua finish.");
        }

        private void Collect()
        {
            var bindTypes = CustomSettings.customTypeList;
            foreach (var bindType in bindTypes)
            {
                m_Types.Add(bindType);
            }
        }

        private void Export()
        {
            m_SB.Append("-- auto generate. don't modify.").NewLine();
            m_SB.Append("error(\"don't run\")").NewLine(2);
            foreach (var type in m_Types)
            {
                ExportType(type);
            }
            for (int i = 0; i < m_AddTypes.Count; i++)
            {
                ExportType(new BindType(m_AddTypes[i]));
            }
            m_SB.NewLine();
            foreach (var type in m_SimpleTypes)
            {
                m_SB.Append("---@class ").Append(GetDisplayName(type)).NewLine();
            }
            foreach (var classType in m_ClassTypes)
            {
                m_SB.Append("---@generic ").Append(classType).Append(" : any").NewLine();
            }
            m_SB.NewLine();
            ExprotNamespace(m_AllNamespace);
        }

        private void ExprotNamespace(Dictionary<string, HashSet<Type>> allNamespace)
        {
            m_SB.Append("-- namespace").NewLine();
            var hashLine = new HashSet<string>();
            foreach (var key in allNamespace.Keys)
            {
                var names = key.Split('.');
                var nametmp = "";
                foreach (var name in names)
                {
                    if (string.IsNullOrEmpty(nametmp))
                    {
                        nametmp += name;
                    }
                    else
                    {
                        nametmp += "." + name;
                    }
                    hashLine.Add(nametmp);
                }
            }
            var output = hashLine.ToList();
            output.Sort();
            foreach (var line in output)
            {
                m_SB.Append(line).Append(" = {}").NewLine();
            }
            m_SB.NewLine();
            m_SB.Append("-- class").NewLine();
            var allType = new List<Type>();
            foreach (var types in allNamespace.Values)
            {
                foreach (var type in types)
                {
                    allType.Add(type);
                }
            }
            allType.Sort(delegate (Type type, Type type1)
            {
                var f1 = type.FullName.Replace('+', '.');
                var f2 = type1.FullName.Replace('+', '.');
                return f1.CompareTo(f2);
            });
            foreach (var type in allType)
            {
                m_SB.Append(type.FullName.Replace('+', '.')).Append(" = ").Append(type.Name).NewLine();
            }
            m_SB.NewLine();
        }

        private void ExportType(BindType bindType)
        {
            var type = bindType.type;

            if (!NeedExportType(type))
            {
                return;
            }

            ToLuaExport.Clear();
            ToLuaExport.className = bindType.name;
            ToLuaExport.type = bindType.type;
            ToLuaExport.isStaticClass = bindType.IsStatic;
            ToLuaExport.baseType = bindType.baseType;
            ToLuaExport.wrapClassName = bindType.wrapName;
            ToLuaExport.libClassName = bindType.libName;
            ToLuaExport.extendList = bindType.extendList;

            ExportType(type);

            ToLuaExport.Clear();

            AddType(type.BaseType);
        }

        private void AddType(Type type)
        {
            if (type != null && NeedExportType(type))
            {
                if (type.IsArray)
                {
                    type = type.GetElementType();
                    if (!NeedExportType(type) || type.IsArray)
                    {
                        return;
                    }
                    if (type.IsGenericParameter)
                    {
                        m_GenType[type.Name] = type;
                        return;
                    }
                }
                if (type.IsGenericParameter || type.IsGenericType)
                {
                    m_GenType[type.Name] = type;
                    return;
                }
                if (typeof(System.MulticastDelegate).IsAssignableFrom(type))
                {
                    m_DelegateTypes[type.Name] = type;
                    return;
                }
                if (!m_AddTypes.Contains(type))
                {
                    m_AddTypes.Add(type);
                }
            }
        }

        private bool NeedExportType(Type type)
        {
            bool value = true;
            if (type == null)
            {
                return false;
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type.IsInterface)
            {
                return false;
            }
            if (type.IsGenericType)
            {
                return false;
            }
            if (m_IgnoreType.Contains(type))
            {
                return false;
            }
            if (m_SimpleTypes.Contains(type))
            {
                return false;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    value = false;
                    break;

                case TypeCode.Boolean:
                    value = false;
                    break;

                case TypeCode.String:
                    value = false;
                    break;
            }
            return value;
        }

        private void ExportType(Type type)
        {
            if (m_HasTypes.Contains(type))
            {
                return;
            }
            m_HasTypes.Add(type);

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                HashSet<Type> namespaceTypes;
                if (!m_AllNamespace.TryGetValue(type.Namespace, out namespaceTypes))
                {
                    namespaceTypes = new HashSet<Type>();
                    m_AllNamespace[type.Namespace] = namespaceTypes;
                }
                namespaceTypes.Add(type);
            }

            m_SB.Append("---@class ").Append(type.Name);
            if (NeedExportType(type.BaseType))
            {
                m_SB.Append(" : ").Append(type.BaseType.Name);
            }
            m_SB.NewLine();

            foreach (var property in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    m_SB.Append("---@field ").Append(property.Name).Space().Append(GetLuaType(property.PropertyType));
                    m_SB.Append(" @[static]");
                    m_SB.NewLine();
                    AddType(property.PropertyType);
                }
            }

            foreach (var property in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    m_SB.Append("---@field ").Append(property.Name).Space().Append(GetLuaType(property.FieldType));
                    m_SB.Append(" @[static]");
                    m_SB.NewLine();
                    AddType(property.FieldType);
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    m_SB.Append("---@field ").Append(property.Name).Space().Append(GetLuaType(property.PropertyType));
                    m_SB.NewLine();
                    AddType(property.PropertyType);
                }
            }

            foreach (var property in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    m_SB.Append("---@field ").Append(property.Name).Space().Append(GetLuaType(property.FieldType));
                    m_SB.NewLine();
                    AddType(property.FieldType);
                }
            }

            if (string.IsNullOrEmpty(type.Namespace))
            {
                m_SB.Append(type.Name).Append(" = {}").NewLine();
            }
            else
            {
                m_SB.Append("local ").Append(type.Name).Append(" = {}").NewLine();
            }

            foreach (var property in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    ExportMethod(type, property, ":");
                }
            }

            foreach (var property in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (!IsObsolete(property) && property.DeclaringType == type)
                {
                    ExportMethod(type, property, ".");
                }
            }

            m_SB.NewLine();
        }

        private void ExportMethod(Type type, MethodInfo property, string lua)
        {
            var paramInfos = property.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                m_SB.Append("---@param ").Append(ConverLuaKeyword(paramInfos[i].Name)).Append(" ").Append(GetLuaType(paramInfos[i].ParameterType)).NewLine();
            }
            if (property.ReturnType != typeof(void))
            {
                m_SB.Append("---@return ").Append(GetLuaType(property.ReturnType)).NewLine();
                AddType(property.ReturnType);
            }
            m_SB.Append("function ").Append(type.Name).Append(lua).Append(property.Name).Append("(");
            for (int i = 0; i < paramInfos.Length; i++)
            {
                m_SB.Append(ConverLuaKeyword(paramInfos[i].Name));
                if (i != paramInfos.Length - 1)
                {
                    m_SB.Append(", ");
                }
            }
            m_SB.Append(") end");
            m_SB.NewLine();
        }

        public string ConverLuaKeyword(string name)
        {
            if (m_ListStr.Contains(name))
            {
                return name + name[name.Length - 1];
            }
            return name;
        }

        private string GetLuaTypeDefaultValue(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return "0";

                case TypeCode.Boolean:
                    return "false";

                case TypeCode.String:
                    return "\"\"";
            }
            return "nil";
        }

        private bool IsObsolete(PropertyInfo memberInfo)
        {
            return ToLuaExport.IsObsolete(memberInfo);
        }

        private bool IsObsolete(MemberInfo memberInfo)
        {
            return ToLuaExport.IsObsolete(memberInfo);
        }

        private bool IsObsolete(MethodInfo memberInfo)
        {
            if (memberInfo.Name.StartsWith("get_"))
            {
                return true;
            }
            if (memberInfo.Name.StartsWith("set_"))
            {
                return true;
            }
            if (memberInfo.Name.StartsWith("op_"))
            {
                return true;
            }
            return ToLuaExport.IsObsolete(memberInfo);
        }

        private string GetLuaType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return "number";

                case TypeCode.Boolean:
                    return "boolean";

                case TypeCode.String:
                    return "string";
            }
            return GetDisplayName(type);
        }

        private void AppendCSharpType(string format, Type type)
        {
            string value = null;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    value = "byte"; break;

                case TypeCode.Char:
                    value = "char"; break;

                case TypeCode.Decimal:
                    value = "decimal"; break;

                case TypeCode.Double:
                    value = "double"; break;

                case TypeCode.Int16:
                    value = "short"; break;

                case TypeCode.Int32:
                    value = "int"; break;

                case TypeCode.Int64:
                    value = "long"; break;

                case TypeCode.SByte:
                    value = "sbyte"; break;

                case TypeCode.Single:
                    value = "float"; break;

                case TypeCode.UInt16:
                    value = "UInt16"; break;

                case TypeCode.UInt32:
                    value = "UInt32"; break;

                case TypeCode.UInt64:
                    value = "UInt64"; break;
            }
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            m_SB.AppendFormat(format, value);
        }

        public string GetDisplayName(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                return string.Format("{0}", GetLuaType(t.GetGenericArguments()[0]));
            if (t.IsGenericType && t.Name.Contains('`'))
            {
                var list = new List<string>();
                foreach (Type tGenericTypeArgument in t.GetGenericArguments())
                {
                    list.Add(GetLuaType(tGenericTypeArgument));
                }
                return string.Format("{0}<{1}>",
                    t.Name.Remove(t.Name.IndexOf('`')),
                    string.Join(",", list.ToArray()));
            }
            if (t.IsArray)
                return string.Format("{0}[{1}]",
                    GetLuaType(t.GetElementType()),
                    new string(',', t.GetArrayRank() - 1));
            return t.Name;
        }
    }
}