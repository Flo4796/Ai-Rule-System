using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RuleEditorSettings
{
    private static RuleEditorSettings instance;
    public static RuleEditorSettings Instance
    {
        get
        { 
            if (instance == null)
            {
                Load();
            }
            return instance;
        }
    }

    private static void Load()
    {
        string path = Application.dataPath.Replace("Assets", "ProjectSettings\\RuleEditorSettings.json");
        string jsonSettings = File.ReadAllText(path);
        if(jsonSettings!= string.Empty)
        {
            instance = JsonUtility.FromJson<RuleEditorSettings>(jsonSettings);
        }
        else
        {
            instance =  new RuleEditorSettings();
        }
    }
    
    public void Save()
    {
        string path = Application.dataPath.Replace("Assets", "ProjectSettings\\RuleEditorSettings.json");
        string jsonSettings = JsonUtility.ToJson(instance);
        File.WriteAllText(path, jsonSettings);
    }

    public Filter[] ProfileFilters = new Filter[0] { };
    public Filter[] StatementFilters = new Filter[0] { };
    public Filter[] ActionFilters = new Filter[0] { };

}
