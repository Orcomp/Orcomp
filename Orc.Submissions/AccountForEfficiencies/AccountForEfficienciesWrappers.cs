namespace Orc.Submissions.AccountForEfficiencies
{
    using System.Collections.Generic;

    using Orc.Entities;
    using Orc.Interface;

    //public class vrstks : IAccountForEfficiencies
    //{
    //    public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
    //    {
    //        return initialInterval.vrstks(dateIntervalEfficiencies, fixedEndPoint);
    //    }
    //}

    public class vrstks2 : IAccountForEfficiencies
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
        {
            return initialInterval.vrstks2(dateIntervalEfficiencies, fixedEndPoint);
        }
    }

    //public class thcristo : IAccountForEfficiencies
    //{
    //    public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
    //    {
    //        return initialInterval.thcristo(dateIntervalEfficiencies, fixedEndPoint);
    //    }
    //}

    //public class thcristo2: IAccountForEfficiencies
    //{
    //    public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
    //    {
    //        return initialInterval.thcristo2(dateIntervalEfficiencies, fixedEndPoint);
    //    }
    //}

    public class thcristo3 : IAccountForEfficiencies
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
        {
            return initialInterval.thcristo3(dateIntervalEfficiencies, fixedEndPoint);
        }
    }

    public class Quicks01lver : IAccountForEfficiencies
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
        {
            return initialInterval.Quicks01ver(dateIntervalEfficiencies, fixedEndPoint);
        }
    }

    //public class Quicks01lver2 : IAccountForEfficiencies
    //{
    //    public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
    //    {
    //        return initialInterval.Quicks01lver2(dateIntervalEfficiencies, fixedEndPoint);
    //    }
    //}

    //public class MoustafaS : IAccountForEfficiencies
    //{
    //    public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
    //    {
    //        return initialInterval.MoustafaS(dateIntervalEfficiencies, fixedEndPoint);
    //    }
    //}

    public class MoustafaS2 : IAccountForEfficiencies
    {
        public DateInterval AccountForEfficiencies(DateInterval initialInterval, List<DateIntervalEfficiency> dateIntervalEfficiencies, FixedEndPoint fixedEndPoint)
        {
            return initialInterval.MoustafaS2(dateIntervalEfficiencies, fixedEndPoint);
        }
    }
}