using System;
using System.Collections.Generic;
using System.Text;

namespace tail2slack
{
    public class LineMatch
    {
        public string Match { get; set; }

        public string Action { get; set; }

    }
    public class FileWatch
    {
        public string Filename { get; set; }
        public LineMatch[] Match { get; set; }

        public long CurrentPosition;
        public long LastSeenLength;
    }

    public class Config
    {
        public string SlackAPI { get; set; }
        public string SlackChannel { get; set; }
        public FileWatch[] Watch { get; set; }
    }
}
/*
 
{ SlackAPI:"",
Watch : {
Filename:"",
Match {}
}

 
 */
