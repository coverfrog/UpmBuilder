#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UpmGuiTool
{
    public static GUIStyle SetColor(Color color, GUIStyle style)
    {
        style.normal = new GUIStyleState()
        {
            textColor = color
        };
        
        style.focused = new GUIStyleState()
        {
            textColor = color
        };
        
        style.hover = new GUIStyleState()
        {
            textColor = color
        };

        return style;
    }

    private static void Label(string str, bool isSuccess)
    {
        Color color = isSuccess ? Color.green : Color.red;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        
        GUILayout.Label(str, SetColor(color, style));
    }

    private static void Indent(int indent, Action content)
    {
        try
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < indent; i++)
            {
                GUILayout.Label("", GUILayout.Width(20));
            }

            content?.Invoke();
        }
        catch
        {
            // ignore
        }
        finally
        {
            EditorGUILayout.EndHorizontal();
        }
    }
    
    public static void CheckFileToLabel(string str, string parent, int indent, ref int failCount)
    {
        string path = Path.Combine(parent, str);

        bool isSuccess = File.Exists(path);
        
        Indent(indent, () =>
        {
            Label(str, isSuccess);
        });
        
        if (isSuccess) return;

        ++failCount;
    }
    
    public static void CheckFolderToLabel(string str, string parent, int indent, out string path, ref int failCount)
    {
        path = Path.Combine(parent, str);

        bool isSuccess = Directory.Exists(path);
        
        Indent(indent, () =>
        {
            Label(str, isSuccess);
        });
        
        if (isSuccess) return;

        ++failCount;
    }
}

#endif