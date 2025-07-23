using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class UpmWrite
{
    public static void Write()
    {
        WriteClass("package.json", UpmWindow.SelectPath, new Manifest()
        {
            name = UpmWindow.PackageName,
            version = "1.0.0",
            displayName = UpmWindow.PackageName,
            description = UpmWindow.PackageName
        });

        WriteFile("README.md", UpmWindow.SelectPath);
        WriteFile("CHANGELOG.md", UpmWindow.SelectPath);
        WriteFile("LICENSE.md", UpmWindow.SelectPath);
        WriteFile("Third Party Notices.md", UpmWindow.SelectPath);

        WriteEditorFolder();
        WriteRuntimeFolder();
        WriteTestsFolder();

        WriteFolder("Samples", UpmWindow.SelectPath, out _);
        
        WriteDocumentationFolder();
    }
    
    private static void WriteFile(string str, string parent)
    {
        string path = Path.Combine(parent, str);
        
        File.WriteAllText(path, "");
    }
    
    private static void WriteFolder(string str, string parent, out string path)
    {
        path = Path.Combine(parent, str);

        Directory.CreateDirectory(path);
    }

    private static void WriteClass<T>(string str, string parent, T t) where T : class
    {
        string path = Path.Combine(parent, str);
        
        string contents = JsonConvert.SerializeObject(t, Formatting.Indented);
        
        File.WriteAllText(path, contents);
    }
    
    private static void WriteEditorFolder()
    {
        WriteFolder("Editor", UpmWindow.SelectPath, out string p0);
        WriteClass($"Unity.{UpmWindow.PackageName}.Editor.asmdef", p0, new AssemblyDefinition()
        {
            name = $"{UpmWindow.PackageName}.Editor",
            rootNamespace = $"{UpmWindow.RootNameSpace}.Editor",
            references = new List<string>()
            {
                UpmWindow.PackageName  
            },
            includePlatforms = new List<string>()
            {
                "Editor"    
            },
        }); 
    }

    private static void WriteRuntimeFolder()
    {
        WriteFolder("Runtime", UpmWindow.SelectPath, out string p0);
        WriteClass($"Unity.{UpmWindow.PackageName}.asmdef", p0, new AssemblyDefinition()
        {
            name = $"{UpmWindow.PackageName}",
            rootNamespace = $"{UpmWindow.RootNameSpace}",
        });
    }

    private static void WriteTestsFolder()
    {
        WriteFolder("Tests", UpmWindow.SelectPath, out string p0);
        
        WriteFolder("Editor", p0, out string p1Editor);
        WriteClass($"Unity.{UpmWindow.PackageName}.Editor.Tests.asmdef", p1Editor, new AssemblyDefinition()
        {
            name = $"{UpmWindow.PackageName}.Editor.Tests",
            rootNamespace = $"{UpmWindow.RootNameSpace}.Editor.Tests",
            references = new List<string>()
            {
                UpmWindow.PackageName,
                $"{UpmWindow.PackageName}.Tests"
            },
            includePlatforms = new List<string>()
            {
                "Editor"    
            },
        });
        
        WriteFolder("Runtime", p0, out string p1Runtime);
        WriteClass($"Unity.{UpmWindow.PackageName}.Tests.asmdef", p1Runtime, new AssemblyDefinition()
        {
            name = $"{UpmWindow.PackageName}.Tests",
            rootNamespace = $"{UpmWindow.RootNameSpace}.Tests",
            references = new List<string>()
            {
                UpmWindow.PackageName,
            },
        });
    }

    private static void WriteDocumentationFolder()
    {
        WriteFolder("Documentation", UpmWindow.SelectPath, out string p0);
        WriteFile($"{UpmWindow.PackageName}.md", p0);
    }
}