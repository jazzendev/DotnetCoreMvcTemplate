using System;
using System.Threading;

namespace MyTemplate.Core.Utility
{
    /// <summary>
    /// http://stackoverflow.com/questions/13252520/system-random-default-constructor-system-clock-resolution
    /// </summary>
    public static class RandomHelper
    {
        private static int seedCounter = new Random().Next();

        [ThreadStatic]
        private static Random _rng;

        public static Random Instance
        {
            get
            {
                int seed = Interlocked.Increment(ref seedCounter);
                return new Random(seed);
            }
        }
    }
}
