namespace Rystem.OpenAi.Files
{
    public enum PurposeFileUpload
    {
        Assistants,
        AssistantsOutput,
        Batch,
        BatchOutput,
        FineTune,
        FineTuneResults,
        Vision
    }
    public static class PurposeFileUploadExtensions
    {

        private const string AssistantsLabel = "assistants";
        private const string AssistantsOutputLabel = "assistants_output";
        private const string BatchLabel = "batch";
        private const string BatchOutputLabel = "batch_output";
        private const string FineTuneResultsLabel = "fine-tune-results";
        private const string VisionLabel = "vision";
        private const string FineTuneLabel = "fine-tune";

        public static string ToLabel(this PurposeFileUpload purpose)
        {
            var currentPurpose = purpose switch
            {
                PurposeFileUpload.Assistants => AssistantsLabel,
                PurposeFileUpload.AssistantsOutput => AssistantsOutputLabel,
                PurposeFileUpload.Batch => BatchLabel,
                PurposeFileUpload.BatchOutput => BatchOutputLabel,
                PurposeFileUpload.FineTuneResults => FineTuneResultsLabel,
                PurposeFileUpload.Vision => VisionLabel,
                _ => FineTuneLabel
            };
            return currentPurpose;
        }
    }
}
