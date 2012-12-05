namespace Nperf.Fixture.IAccountForEfficiencies
{
    using System.Collections.Generic;

    using Orc.Interval;

    public interface IAccountForEfficiencies
    {
        DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint);
    }
}
