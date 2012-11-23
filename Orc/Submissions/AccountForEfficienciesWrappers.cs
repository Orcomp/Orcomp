namespace Orc.Submissions
{
    using System.Collections.Generic;

    using Orc.Entities;

    public class vrstks_AccountForEfficiencies : IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.vrstks(dateIntervalEfficiencies);
        }
    }

    public class vrstks2_AccountForEfficiencies : IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.vrstks2(dateIntervalEfficiencies);
        }
    }

    public class thcristo_AccountForEfficiencies : IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.thcristo(dateIntervalEfficiencies);
        }
    }

    public class thcristo2_AccountForEfficiencies: IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.thcristo2(dateIntervalEfficiencies);
        }
    }

    public class Quicks01lver2_AccountForEfficiencies : IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.Quicks01lver2(dateIntervalEfficiencies);
        }
    }

    public class MoustafaS_AccountForEfficiencies : IAccountForEfficienciesWrapper
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies)
        {
            return initialInterval.MoustafaS(dateIntervalEfficiencies);
        }
    }
}