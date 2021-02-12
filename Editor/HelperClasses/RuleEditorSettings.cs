using System.IO;
using UnityEngine;

namespace AdelicSystem.RuleAI.Editor
{

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
            if (File.Exists(path))
            {
                string jsonSettings = File.ReadAllText(path);
                instance = JsonUtility.FromJson<RuleEditorSettings>(jsonSettings);
            }
            else
            {
                instance = new RuleEditorSettings();
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
}