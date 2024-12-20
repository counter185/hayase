using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hayase
{
    internal class HayaseTimer
    {
        private ulong startTime;
        private bool started = false;

        static ulong timeNow() => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public void Start()
        {
            startTime = timeNow();
            started = true;
        }

        public ulong Elapsed()
        {
            if (!started)
            {
                throw new InvalidOperationException("Timer not started");
            }
            return timeNow() - startTime;
        }

        public void Reset()
        {
            started = false;
        }

        public double PercentElapsed(ulong duration)
        {
            return Math.Min(1.0, (double)Elapsed() / duration);
        }
    }
}
