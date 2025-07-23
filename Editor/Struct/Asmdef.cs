using System;
using System.Collections.Generic;

[Serializable]
public class AssemblyDefinition
{
    public string name;
    public string rootNamespace;
    public List<string> references = new List<string>();
    public List<string> includePlatforms = new List<string>();
    public List<string> excludePlatforms = new List<string>();
    public bool allowUnsafeCode;
    public bool overrideReferences;
    public List<string> precompiledReferences = new List<string>();
    public bool autoReferenced = true;
    public List<string> defineConstraints = new List<string>();
    public List<VersionDefine> versionDefines = new List<VersionDefine>();
    public bool noEngineReferences;
}