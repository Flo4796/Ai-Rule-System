using System.Collections.Generic;
using UnityEditor;

namespace AdelicSystem.RuleAI.Editor
{
    /// <summary>
    /// Represents EditorWindow data regarding session related variables.
    /// </summary>
    public class SessionData
    {
        public int ToolbarIndex;
        public bool RuleEditUnsaved;
        public Dictionary<TypeFilter, bool> isProfileTypeToggled = new Dictionary<TypeFilter, bool>();
        public Dictionary<StatementType, bool> isStatementTypeToggled = new Dictionary<StatementType, bool>();
        public Dictionary<ActionType, bool> isActionTypeToggled = new Dictionary<ActionType, bool>();
        public SerializedObject selectedController = null;
        public SerializedProperty selectedRule = null;
    }
}
