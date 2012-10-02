using System.Collections.Generic;

namespace Orcomp.Entities
{
    public abstract class Constraint<T>
    {
        protected Constraint()
        {
            Items = new Dictionary<string, T>();
        }

        public Dictionary<string, T> Items { get; private set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public string Name { get; set; }

        // Dictionary<unique identifier, T>
    }
}