namespace Rystem.PlayFramework
{
    [Flags]
    public enum AiResponseStatus
    {
        Request = 1,
        SceneRequest = 2,
        Running = 4,
        FunctionStreamRequest = 8,
        FunctionRequest = 16,
        FinishedOk = 32,
        FinishedNoTool = 64,
        FinishedWarning = 128,
        FinishedError = 256,
    }
}
