using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler
{
    public class Timer
    {

        private DateTime _startTimer;
        private DateTime _endTimer;
        private bool running = false;

        public void StartTimer()
        {
            if (running == false)
            {
                running = true;
                _startTimer = DateTime.Now;
                Console.WriteLine("Timer startet\n");
            }
        }

        public void StopTimer()
        {
            if (running == true)
            {
                running = false;
                _endTimer = DateTime.Now;
                Duration();
            }
        }

        private void Duration()
        {
            var result = _endTimer - _startTimer;

            Console.WriteLine("\nTimer stoppet, det tog: " + result);
        }
    }
}
