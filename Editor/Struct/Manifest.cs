using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

[Serializable]
public class Manifest
{
    public string name;
    public string version;
    public string displayName;
    public string description;
    public string unity;
    public string unityRelease;
    public Dictionary<string, string> dependencies = new Dictionary<string, string>();
    public List<string> keywords = new List<string>();
    public AuthorInfo author = new AuthorInfo();
}
