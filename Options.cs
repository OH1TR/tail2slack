using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace tail2slack
{
    public class Options
    {
        [Option('c', "config", Required = false, HelpText = "Config file")]
        public string ConfigFile { get; set; }

    }
}
