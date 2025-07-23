#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class UpmWindow : EditorWindow
{
    #region Init

    [MenuItem("Cf/Upm Window")]
    public static void Init()
    {
        _ = GetWindow<UpmWindow>();
    }

    #endregion

    #region Data

    private const string KeySelectPath = "cf upm window select path";
    
    public static string SelectPath
    {
        get => EditorPrefs.GetString(KeySelectPath);
        private set => EditorPrefs.SetString(KeySelectPath, value);
    }
    
    private const string KeyPackageName = "cf upm package name";
    
    public static string PackageName
    {
        get => EditorPrefs.GetString(KeyPackageName);
        private set => EditorPrefs.SetString(KeyPackageName, value);
    }

    private const string KeyCheckAllCorrect = "cf upm package correct";
    
    public static bool CheckAllCorrect
    {
        get => EditorPrefs.GetBool(KeyCheckAllCorrect);
        private set => EditorPrefs.SetBool(KeyCheckAllCorrect, value);
    }
    
    
    private const string KeyPackageCorrectName = "cf upm package name correct";
    
    public static bool PackageNameCorrect
    {
        get => EditorPrefs.GetBool(KeyPackageCorrectName);
        private set => EditorPrefs.SetBool(KeyPackageCorrectName, value);
    }
    
    private const string KeyRootNameSpace = "cf upm rootname space";
    
    public static string RootNameSpace
    {
        get => EditorPrefs.GetString(KeyRootNameSpace);
        private set => EditorPrefs.SetString(KeyRootNameSpace, value);
    }
    
    private const string KeyRootNameSpaceCorrect = "cf upm rootname space correct";
    
    public static bool RootNameSpaceCorrect
    {
        get => EditorPrefs.GetBool(KeyRootNameSpaceCorrect);
        private set => EditorPrefs.SetBool(KeyRootNameSpaceCorrect, value);
    }
    
    #endregion

    private void OnGUI()
    {
        Select();
        Check();
        Write();
    }

    private static void Select()
    {
        if (!Directory.Exists(SelectPath))
        {
            SelectPath = null;
        }
        
        UpmGui.SelectPath(SelectPath, () =>
        {
            string path = EditorUtility.OpenFolderPanel("Select", Application.dataPath, "");

            if (string.IsNullOrEmpty(path)) return;

            if (!path.Contains(Application.dataPath)) return;

            SelectPath = path;
        });
        
        UpmGui.SelectProductName(PackageName, () =>
        {
            var condition = !string.IsNullOrEmpty(PackageName);
            string[] split = new string[] { };

            if (condition)
            {
                condition = Regex.IsMatch(PackageName, @"^[a-z0-9_\-\.\/]+$");
            }

            if (condition)
            {
                split = PackageName.Split(new char[] { '.' });
                condition = split.Length >= 3;
            }

            if (condition)
            {
                condition = PackageName.StartsWith("com.");
            }

            if (condition)
            {
                int count = split.Count(str => str.Length <= 1 || str.Contains(" "));
                condition = count == 0;
            }

            PackageNameCorrect = condition;

        }, (str) =>
        {
            PackageName = str;
        });
        
        UpmGui.SelectRootNameSpace(RootNameSpace, () =>
        {
            var condition = !string.IsNullOrEmpty(RootNameSpace);
            string[] split = RootNameSpace.Split(new char[] { '.' });

            if (condition)
            {
                condition = Regex.IsMatch(RootNameSpace, @"^[a-zA-Z0-9_\-\.\/]+$");
            }

            if (condition)
            {
                int count = split.Count(str => str.Length <= 1 || str.Contains(" "));
                condition = count == 0;
            }

            if (condition)
            {
                condition = RootNameSpace.Length > 0;
            }

            RootNameSpaceCorrect = condition;

        }, (str) =>
        {
            RootNameSpace = str;
        });
    }

    private static void Check()
    {
        UpmGui.Check((success) =>
        {
            CheckAllCorrect = success;
        });
    }

    private static void Write()
    {
        UpmGui.WriteButton(() =>
        {
            UpmWrite.Write();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string path = "Assets" + SelectPath[Application.dataPath.Length..];
            
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
        });
    }
}

#endif