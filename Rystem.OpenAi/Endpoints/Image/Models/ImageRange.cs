using System;

namespace Rystem.OpenAi.Image
{
    public readonly struct ImageRange
    {
        public Range Horizontal { get; }
        public Range Vertical { get; }
        public ImageRange(Range horizontal, Range vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }
}
