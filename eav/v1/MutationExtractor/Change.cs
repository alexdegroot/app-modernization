using System;

namespace MutationExtractor
{
    public class Change
    {
        public Guid Guid { get; } = Guid.NewGuid();
    }
}