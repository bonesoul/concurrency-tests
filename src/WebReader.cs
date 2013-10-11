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

using System.IO;
using System.Net;

namespace ConcurrencyTests
{
    /// <summary>
    /// A simple web-reader.
    /// </summary>
    public static class WebReader
    {
        public static Result Read(string url, int timeout = 30 * 1000)
        {
            var result = new Result(); // our result object.

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result.Response = reader.ReadToEnd();
                        result.State = States.Success;
                    }
                }
            }
            catch (WebException e)
            {
                result.State = e.Status == WebExceptionStatus.Timeout 
                    ? States.Timeout : States.Failed; // check the exception type and set our result state according.
            }

            return result;
        }


        /// <summary>
        /// A result object provided by the web-reader.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// The response read from web.
            /// </summary>
            public string Response { get; internal set; }

            /// <summary>
            /// The result's state.
            /// </summary>
            public States State { get; internal set; }

            public Result()
            {
                this.State = States.Unknown;
                this.Response = "";
            }
        }

        /// <summary>
        /// The operation's result status.
        /// </summary>
        public enum States
        {
            Unknown,
            Success,
            Failed,
            Timeout
        }
    }
}