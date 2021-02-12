namespace AdelicSystem.RuleAI
{
    public enum RuleType
    {
        unknown,
        unit
    }

    public enum StatementType
    {
        unknown,
        Gate,
        Inequality,
        Generator,
        Mutator,
        Evaluation
    }

    public enum ActionType
    {
        unknown,
        Movement,
        Targeting,
        Combat,
        Utils
    }
}