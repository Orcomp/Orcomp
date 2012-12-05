namespace Orc.Entities
{
    using System.Collections.Generic;

    public abstract class Constraint<T>
    {
        protected Constraint()
        {
            this.Items = new Dictionary<string, T>();
        }

        public Dictionary<string, T> Items { get; private set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public string Name { get; set; }

        // Dictionary<unique identifier, T>
    }
}