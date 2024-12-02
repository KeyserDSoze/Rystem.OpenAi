namespace Rystem.OpenAi
{

    public class ModelName
    {
        internal ModelName(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
        public static implicit operator string(ModelName name)
            => name.Name;
        public static implicit operator ModelName(string name)
            => new(name);
    }
}
