namespace NPerf.Fixture.AList
{
    using System.Collections;

    public static class Helpers
    {
        public static void Sort(IList list)
        {
            if (list is AListInt)
            {
                ((AListInt)list).Sort();
            }
            else if (list is SystemListInt)
            {
                ((SystemListInt)list).Sort();
            }
        }
    }
}
