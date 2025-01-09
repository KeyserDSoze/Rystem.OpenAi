namespace Rystem.OpenAi.Assistant
{
    public sealed class ToolOutputBuilder<T>
    {
        private readonly T _entity;
        private readonly ToolOutputRequest _request;

        internal ToolOutputBuilder(T entity, ToolOutputRequest request)
        {
            _entity = entity;
            _request = request;
        }
        public T And() => _entity;
        public ToolOutputBuilder<T> AddToolOutput(string toolCallId, string output)
        {
            _request.ToolOutputs ??= [];
            _request.ToolOutputs.Add(new ToolOutput
            {
                ToolCallId = toolCallId,
                Output = output
            });
            return this;
        }
    }
}
