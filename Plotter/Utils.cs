using System.Diagnostics;

namespace Main
{
    public static class Utils
    {
        /// <summary>
        /// Blocks the thread for a specific time
        /// </summary>
        /// <param name="microseconds">Time in microseconds</param>
        public static void UDelay(long microseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long v = (microseconds * Stopwatch.Frequency) / 1000000L;
            while (stopwatch.ElapsedTicks < v) ;
        }
    }
}