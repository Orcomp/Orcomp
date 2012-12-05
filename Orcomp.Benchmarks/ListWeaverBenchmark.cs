
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Orc.Benchmarks
{
    using Orc.Algorithms;

    public static class ListWeaverBenchmark
    {

        public static  void Run()
        {
            const int finalSequenceLength = 10000;

            // Return valid yarn lengths for a given finalSequenceLength
            //Enumerable.Range( 2, finalSequenceLength ).Where( x => (finalSequenceLength - x) % (x - 1) == 0 );

            var yarnLengths = new List<int> { 2, 10, 102, 304, 1112, 3334 };

            long elapsedTimeInMilliseconds;

            foreach (var yarnLength in yarnLengths)
            {
                elapsedTimeInMilliseconds = ListWeaverBenchmark.TimeListWeave(finalSequenceLength, yarnLength, false);
                Console.WriteLine(string.Format("Final sequence length {0}, with {1} yarnLength, took {2} milliseconds, to sequence.", finalSequenceLength, yarnLength, elapsedTimeInMilliseconds));
            }

            foreach (var yarnLength in yarnLengths)
            {
                elapsedTimeInMilliseconds = ListWeaverBenchmark.TimeListWeave(finalSequenceLength, yarnLength, true);
                Console.WriteLine(string.Format("REVERSED: Final sequence length {0}, with {1} yarnLength, took {2} milliseconds, to sequence.", finalSequenceLength, yarnLength, elapsedTimeInMilliseconds));
            }

            elapsedTimeInMilliseconds = ListWeaverBenchmark.TimeListWeave2(finalSequenceLength, false);
            Console.WriteLine(string.Format("Final sequence length {0}, took {1} milliseconds, to sequence.", finalSequenceLength, elapsedTimeInMilliseconds));

            elapsedTimeInMilliseconds = ListWeaverBenchmark.TimeListWeave2(finalSequenceLength, true);
            Console.WriteLine(string.Format("REVERSED: Final sequence length {0}, took {1} milliseconds, to sequence.", finalSequenceLength, elapsedTimeInMilliseconds));

            Console.ReadLine();
        }

        public static long TimeListWeave(int finalSequenceLength, int yarnLength, bool reverse)
        {
            var yarns = GetYarns( finalSequenceLength, yarnLength, reverse );

            var listWeaver = new ListWeaver<int>();

            yarns.ForEach( listWeaver.Yarns.Add );

            Stopwatch s = Stopwatch.StartNew();
            var result = listWeaver.Weave();
            s.Stop();

            // Check that the two sequences are correct.
            Debug.Assert(result.SequenceEqual( Enumerable.Range( 1, finalSequenceLength )));

            return s.ElapsedMilliseconds;
        }

        public static long TimeListWeave2(int finalSequenceLength, bool reverse)
        {
            var listWeaver = new ListWeaver<int>();

            var yarns = new List<List<int>>();

            for(int i=1; i<= finalSequenceLength; i++)
            {
                yarns.Add(Enumerable.Range( 1, i).ToList() );
            }

            if (reverse)
            {
                yarns.Reverse();
            }

            yarns.ForEach(listWeaver.Yarns.Add);

            Stopwatch s = Stopwatch.StartNew();
            var result = listWeaver.Weave();
            s.Stop();

            // Check that the two sequences are correct.
            Debug.Assert(result.SequenceEqual(Enumerable.Range(1, finalSequenceLength)));

            return s.ElapsedMilliseconds;
        }

        public static List<List<int>> GetYarns(int finalSequenceLength, int yarnLength, bool reverse)
        {

            if ((finalSequenceLength - yarnLength) % ( yarnLength -1)  != 0)
            {
                throw new ArgumentException("finalSequenceLength and yarnLength are incompatible");
            }

            var yarnCount = finalSequenceLength / yarnLength;

            var yarns = new List<List<int>>();
            int start = 1;

            int loops = 1 + (finalSequenceLength - yarnLength) / (yarnLength - 1);

            for (int i = 0; i < loops; i++)
            {
                var yarn = Enumerable.Range(start, yarnLength);
                yarns.Add(yarn.ToList());
                start += yarnLength - 1;
            }

            if (reverse)
            {
                yarns.Reverse();
            }

            return yarns;
        }
    }
}
