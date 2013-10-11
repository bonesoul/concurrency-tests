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
using System.Xml.Linq;

namespace ConcurrencyTests
{
    /// <summary>
    /// A simple feed parser.
    /// </summary>
    public class FeedParser
    {
        /// <summary>
        /// The feed url.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Feed's contained stories.
        /// </summary>
        public List<FeedItem> Stories { get; private set; }           
	    
        /// <summary>
        /// Creates a new feed parser instance for given url.
        /// </summary>
        /// <param name="url"></param>
        public FeedParser(string url)
        {
            this.Url = url;
            this.Stories = new List<FeedItem>();
        }

        /// <summary>
        /// Parses the feed.
        /// </summary>
        /// <returns></returns>
		public bool Parse()
		{
			var result = WebReader.Read(this.Url); // read the feed source.

			if (result.State != WebReader.States.Success) 
                return false;

            try
            {
                var xdoc = XDocument.Parse(result.Response); // parse the document.

                if (xdoc.Root == null) 
                    return false;

                var defaultNs = xdoc.Root.GetDefaultNamespace();

                var entries = from item in xdoc.Descendants(defaultNs + "item")
                              select new
                              {
                                  Id = (string)item.Element(defaultNs + "guid") ?? "",
                                  Title = (string)item.Element(defaultNs + "title") ?? "",
                                  Link = (string)item.Element(defaultNs + "link") ?? "",
                              };

                Stories.AddRange(entries.Select(entry => new FeedItem(entry.Title, entry.Id, entry.Link))); // add parsed stories to our list.
                return Stories.Count > 0;
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                return false;
            } 
		}
    }

    /// <summary>
    /// Feed item (story).
    /// </summary>
    public class FeedItem
    {
        /// <summary>
        /// Story title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Story GUID.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Story link.
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Creates a new feed item instance.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <param name="link"></param>
        public FeedItem(string title, string id, string link)
        {
            this.Title = title;
            this.Id = id;
            this.Link = link;
        }
    }
}
