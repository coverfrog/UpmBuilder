#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class UpmGui
{
    private const int ButtonHeight = 25;

    #region Select

    public static void SelectPath(string selectPath, Action onClick)
    {
        EditorGUILayout.BeginHorizontal();

        const int width = 100;
        
        try
        {
            var options0 = new[] { GUILayout.Width(width), GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(false) };

            var isPress = GUILayout.Button("Select Folder", options0);
            if (isPress)
            {
                onClick?.Invoke();
            }

            var style01 = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            var options01 = new[] { GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(true) };
            
            GUILayout.Label(selectPath, style01, options01);
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

    public static void SelectProductName(string packageName, Action onCheck, Action<string> onSet)
    {
        EditorGUILayout.BeginHorizontal();

        try
        {
            onCheck?.Invoke();
            

            var style0 = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            var option0 = new[] { GUILayout.Width(98), GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(false) };
            
            GUILayout.Label("Package Name", style0, option0);

            var style1 = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            var option1 = new[] {  GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(true) };
            
            var tempPackageName = EditorGUILayout.TextField(packageName, style1, option1);
            
            onSet?.Invoke(tempPackageName);
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
    
    public static void SelectRootNameSpace(string packageName, Action onCheck, Action<string> onSet)
    {
        EditorGUILayout.BeginHorizontal();

        try
        {
            onCheck?.Invoke();
            

            var style0 = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            var option0 = new[] { GUILayout.Width(98), GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(false) };
            
            GUILayout.Label("Name Space", style0, option0);

            var style1 = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };

            var option1 = new[] {  GUILayout.Height(ButtonHeight), GUILayout.ExpandWidth(true) };
            
            var tempPackageName = EditorGUILayout.TextField(packageName, style1, option1);
            
            onSet?.Invoke(tempPackageName);
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

    
    #endregion

    #region Check

    public static void Check(Action<bool> onCheck)
    {
        GUILayout.Space(20);

        int failCount = 0;
        
        UpmGuiTool.CheckFileToLabel("package.json"          , UpmWindow.SelectPath, 0, ref failCount);                  
        UpmGuiTool.CheckFileToLabel("README.md"             , UpmWindow.SelectPath, 0, ref failCount);                  
        UpmGuiTool.CheckFileToLabel("CHANGELOG.md"          , UpmWindow.SelectPath, 0, ref failCount);                    
        UpmGuiTool.CheckFileToLabel("LICENSE.md"            , UpmWindow.SelectPath, 0, ref failCount);                     
        UpmGuiTool.CheckFileToLabel("Third Party Notices.md", UpmWindow.SelectPath, 0, ref failCount);                     

        CheckEditorFolder(ref failCount);
        CheckRuntimeFolder(ref failCount);
        CheckTestsFolder(ref failCount);
        
        UpmGuiTool.CheckFolderToLabel("Samples", UpmWindow.SelectPath, 0, out _, ref failCount); 

        CheckDocumentationFolder(ref failCount);
        
        onCheck?.Invoke(failCount == 0);
    }
    
    private static void CheckEditorFolder(ref int failCount)
    {
        UpmGuiTool.CheckFolderToLabel("Editor", UpmWindow.SelectPath, 0, out string p0, ref failCount); 
        UpmGuiTool.CheckFileToLabel($"Unity.{UpmWindow.PackageName}.Editor.asmdef", p0, 1, ref failCount);   
    }

    private static void CheckRuntimeFolder(ref int failCount)
    {
        UpmGuiTool.CheckFolderToLabel("Runtime", UpmWindow.SelectPath, 0, out string p0, ref failCount); 
        UpmGuiTool.CheckFileToLabel($"Unity.{UpmWindow.PackageName}.asmdef", p0, 1, ref failCount);   
    }

    private static void CheckTestsFolder(ref int failCount)
    {
        UpmGuiTool.CheckFolderToLabel("Tests", UpmWindow.SelectPath, 0, out string p0, ref failCount); 

        UpmGuiTool.CheckFolderToLabel("Editor", p0, 1, out string p1Editor, ref failCount);
        UpmGuiTool.CheckFileToLabel($"Unity.{UpmWindow.PackageName}.Editor.Tests.asmdef", p1Editor, 2, ref failCount);
        
        UpmGuiTool.CheckFolderToLabel("Runtime", p0, 1, out string p1Runtime, ref failCount); 
        UpmGuiTool.CheckFileToLabel($"Unity.{UpmWindow.PackageName}.Tests.asmdef", p1Runtime, 2, ref failCount);
    }

    private static void CheckDocumentationFolder(ref int failCount)
    {
        UpmGuiTool.CheckFolderToLabel("Documentation", UpmWindow.SelectPath, 0, out string p0, ref failCount); 
        UpmGuiTool.CheckFileToLabel($"{UpmWindow.PackageName}.md", p0, 1, ref failCount);   
    }
    

    #endregion

    #region Create

    public static void WriteButton(Action onClick)
    {
        GUILayout.Space(20);
        
        var color = Color.red;
        var isCanCreate = false;
        var btnSubject = "Don't Create : ";

        if (string.IsNullOrEmpty(UpmWindow.SelectPath))
        {
            btnSubject += "Folder Not Select";
        }

        else
        {
            if (UpmWindow.CheckAllCorrect)
            {
                btnSubject = "Complete !";
                color = Color.green;
            }
         
            else if (Directory.GetFileSystemEntries(UpmWindow.SelectPath).Length > 0)
            {
                btnSubject += "Folder Is Not Null";
                
                if (GUILayout.Button("Clean", GUILayout.Height(ButtonHeight)))
                {
                    Directory.Delete(UpmWindow.SelectPath, true);
                    Directory.CreateDirectory(UpmWindow.SelectPath);
                    
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            
            else if (!UpmWindow.PackageNameCorrect)
            {
                btnSubject += "Check Package Name";
            }
            
            else if (!UpmWindow.RootNameSpaceCorrect)
            {
                btnSubject += "Check Name Space";
            }
            
            else
            {
                isCanCreate = true;
                btnSubject = "Write";
                color = Color.white;
            }
        }


        var style0 = UpmGuiTool.SetColor(color, new GUIStyle(GUI.skin.button));
        
        if (GUILayout.Button(btnSubject, style0, GUILayout.Height(ButtonHeight)))
        {
            if (!isCanCreate)
            {
                return;
            }
            
            onClick?.Invoke();
        }
    }

    #endregion
   
}
#endif