namespace Rystem.PlayFramework
{
    public enum AiResponseStatus
    {
        Starting,
        Running,
        FunctionStreamRequest,
        FunctionRequest,
        FinishedOk,
        FinishedNoTool,
        FinishedWarning,
        FinishedError,
    }
}
