/*    
 * The MIT License (MIT)
 * 
 * Copyright (c) 2011 - 2013 Hüseyin Uslu - shalafiraistlin@gmail.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software. 

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyTests
{
    class Program
    {
        /// <summary>
        /// Iteration count.
        /// </summary>
        public const int IterationCount = 10;

        /// <summary>
        /// Program entrance.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // list of feeds to fetch.
            string[] feeds = {
                                 "http://www.teamliquid.net/rss/news.xml",
                                 "http://www.diablofans.com/rss/forums/1-diablo-fans-home-page-news/",
                                 "http://feeds.feedburner.com/blizzplanetcom",
                                 "http://www.tehgladiators.com/rss.xml",
                                 "http://www.mmo-champion.com/external.php?do=rss&type=newcontent&sectionid=1&days=120&count=10",
                                 "http://wow.joystiq.com/rss.xml",
                                 "http://www.wowhead.com/blog&rss",
                                 "http://www.tentonhammer.com/wow/all/feed",
                                 "http://feeds.feedburner.com/LookingForGroup",
								  "http://www.d3sanc.com/feed/"	
                             };

            var timeSpanSequentials = new TimeSpan[IterationCount];
            var timeSpanParallelForEachs = new TimeSpan[IterationCount];
            var timeSpanTPLs = new TimeSpan[IterationCount];
            var timeSpanThreadPools = new TimeSpan[IterationCount];
	        var timeSpanThreads = new TimeSpan[IterationCount];

            PrintCPUInfo(); // comment this if you will be running on non-windows environment.

            Console.WriteLine("Will be parsing a total of {0} feeds.", feeds.Length);
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine("Itr.\tSeq.\tPrlEx\tTPL\tTPool\tThread");
            Console.WriteLine("________________________________________________________________________________");

            // run the iterations and measure timings.
            for (int i = 0; i < IterationCount; i++)
            {
                Console.Write("#{0}\t", i + 1);

                var timeSpanSequential = MeasureSequential(feeds);
                timeSpanSequentials[i] = timeSpanSequential;
                Console.Write(String.Format("{0:00}.{1:00}s\t",timeSpanSequential.Seconds, timeSpanSequential.Milliseconds / 10));

                var timeSpanParallelForeach = MeasureParallelForeach(feeds);
                timeSpanParallelForEachs[i] = timeSpanParallelForeach;
                Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanParallelForeach.Seconds, timeSpanParallelForeach.Milliseconds / 10));

                var timeSpanTPL = MeasureTPL(feeds);
                timeSpanTPLs[i] = timeSpanTPL;
                Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanTPL.Seconds, timeSpanTPL.Milliseconds / 10));

                var timeSpanThreadPool = MeasureNativeThreadPool(feeds);
                timeSpanThreadPools[i] = timeSpanThreadPool;
                Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanThreadPool.Seconds, timeSpanThreadPool.Milliseconds / 10));

	            var timeSpanThread = MeasureThreads(feeds);
	            timeSpanThreads[i] = timeSpanThread;
                Console.Write(String.Format("{0:00}.{1:00}s\t\n", timeSpanThread.Seconds, timeSpanThread.Milliseconds / 10));
            }
            
            // calculate the average timings also.
            Console.WriteLine("\n________________________________________________________________________________");
            Console.Write("Avg.\t");

            var timeSpanSequentialAverage = CalculateAverageTimeSpan(timeSpanSequentials);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanSequentialAverage.Seconds, timeSpanSequentialAverage.Milliseconds / 10));

            var timeSpanParallelForeachAverage = CalculateAverageTimeSpan(timeSpanParallelForEachs);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanParallelForeachAverage.Seconds, timeSpanParallelForeachAverage.Milliseconds / 10));

            var timeSpanTPLAverage = CalculateAverageTimeSpan(timeSpanTPLs);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanTPLAverage.Seconds, timeSpanTPLAverage.Milliseconds / 10));

            var timeSpanThreadPoolAvarage = CalculateAverageTimeSpan(timeSpanThreadPools);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanThreadPoolAvarage.Seconds, timeSpanThreadPoolAvarage.Milliseconds / 10));

            var timeSpanThreadAvarage = CalculateAverageTimeSpan(timeSpanThreads);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanThreadAvarage.Seconds, timeSpanThreadAvarage.Milliseconds / 10));

            Console.WriteLine("\n________________________________________________________________________________");
            Console.ReadLine();
        }

        #region Measurement code

        #region sequential

        /// <summary>
        /// Measures sequential version.
        /// </summary>
        /// <param name="feedSources"></param>
        /// <returns></returns>
        private static TimeSpan MeasureSequential(IEnumerable<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var f in feedSources.Select(feedSource => new FeedParser(feedSource)))
            {
                f.Parse();
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;            
        }

        #endregion

        #region Threaded

        /// <summary>
        /// Measures threaded version.
        /// </summary>
        /// <param name="feedSources"></param>
        /// <returns></returns>
		private static TimeSpan MeasureThreads(IList<string> feedSources)
		{
            var stopwatch = new Stopwatch();
            stopwatch.Start();

			var threads = new Thread[feedSources.Count];

			for (var i = 0; i < feedSources.Count; i++)
			{
                var source = feedSources[i]; /* work-around modified closures */
				var feedParser = new FeedParser(source);
				threads[i] = new Thread(() => feedParser.Parse());
				threads[i].Start();
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

            stopwatch.Stop();
            return stopwatch.Elapsed;            
		}

        #endregion

        #region Paralel-Foreach

        private static TimeSpan MeasureParallelForeach(IEnumerable<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.ForEach(feedSources, feedSource =>
                {
                    var f = new FeedParser(feedSource);
                    f.Parse();
                }
            );

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion

        # region Task Parallel Library
        private static TimeSpan MeasureTPL(IList<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tasks = new Task[feedSources.Count];
            for (var i = 0; i < feedSources.Count;i++ )
            {
                var source = feedSources[i]; /* work-around modified closures */
                tasks[i] = Task.Factory.StartNew(() => TPLFeedParserTask(source));
            }

            Task.WaitAll(tasks);

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static void TPLFeedParserTask(string feedSource)
        {
            var f = new FeedParser(feedSource);
            f.Parse();
        }

        #endregion

        #region Thread Pools

        private static TimeSpan MeasureNativeThreadPool(string[] feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var threadPoolDoneEvents = new ManualResetEvent[feedSources.Length];

            for (int i = 0; i < feedSources.Length; i++)
            {
                threadPoolDoneEvents[i] = new ManualResetEvent(false);
                var wrapper = new ThreadPoolDataWrapper(threadPoolDoneEvents[i], feedSources[i]);
                ThreadPool.QueueUserWorkItem(ThreadPoolTask, wrapper);
            }

            WaitHandle.WaitAll(threadPoolDoneEvents);

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static void ThreadPoolTask(object data)
        {
            var f = new FeedParser(((ThreadPoolDataWrapper) data).FeedSource);
            f.Parse();
            var threadPoolDataWrapper = data as ThreadPoolDataWrapper;

            if (threadPoolDataWrapper != null) 
                threadPoolDataWrapper.ResetEvent.Set();
        }

        class ThreadPoolDataWrapper
        {
            public ManualResetEvent ResetEvent { get; set; }
            public string FeedSource { get; private set; }

            public ThreadPoolDataWrapper(ManualResetEvent @event, string feedSource)
            {
                this.ResetEvent = @event;
                this.FeedSource = feedSource;
            }
        }

        #endregion

        private static TimeSpan CalculateAverageTimeSpan(TimeSpan[] timeSpans)
        {
            double miliseconds = timeSpans.Sum(t => t.TotalMilliseconds) / timeSpans.Length;
            return TimeSpan.FromMilliseconds(miliseconds);
        }

        #endregion

        # region CPU-info

        static void PrintCPUInfo()
        {
            /* code taken from: http://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c/2670568#2670568 */

            int physicalCpuCount = 0;
            int coreCount = 0;
            bool coreCountSupported;
            int logicalCpuCount = 0;
            bool logicalCpuCountSupported;

            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                physicalCpuCount = Int32.Parse(item["NumberOfProcessors"].ToString());
            }

            try
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
                coreCountSupported=true;
            }
            catch(Exception) { coreCountSupported = false; }

            try
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
                {
                    logicalCpuCount = Int32.Parse(item["NumberOfLogicalProcessors"].ToString());
                }
                logicalCpuCountSupported = true;
            }
            catch(Exception) { logicalCpuCountSupported = false; }

            Console.WriteLine("Test Environment: {0} physical cpus, {1} cores, {2} logical cpus.", physicalCpuCount, coreCountSupported ? coreCount.ToString() : "NotSupported", logicalCpuCountSupported ? logicalCpuCount.ToString() : "NotSupported");
        }
    }

    #endregion
}
