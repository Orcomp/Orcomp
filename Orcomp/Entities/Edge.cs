using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Orcomp.Interfaces;

namespace Orcomp.Entities
{
    public class Edge<T> where T : IDateRange
    {
        public T DateRange { get; private set; }

        public EdgeType EdgeType { get; private set; }

        public DateTime DateTime { get { return EdgeType == EdgeType.Start ? DateRange.StartTime : DateRange.EndTime;}}

        public Edge(T dateRange, EdgeType edgeType)
        {
            DateRange = dateRange;
            EdgeType = edgeType;
        }
    }
}
