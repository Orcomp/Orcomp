namespace Orc.Submissions
{
    using System.Collections.Generic;

    using Orc.Entities;

    public interface IAccountForEfficienciesWrapper
    {
        DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies);
    }
}
