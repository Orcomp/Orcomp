namespace Orc.Interface
{
    using System.Collections.Generic;

    using Orc.Entities;

    public interface IAccountForEfficiencies
    {
        DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint);
    }
}
