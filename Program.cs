using CommandLine;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace tail2slack
{
    class Program
    {
        static string ModuleFolder;

        static void Main(string[] args)
        {
            ModuleFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            try
            {
                Parser.Default.ParseArguments<Options>(args)
                                  .WithParsed<Options>(o =>
                                  {
                                      Launch(o);
                                  });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        static void Launch(Options options)
        {
            if (options.ConfigFile == null)
                options.ConfigFile = Path.Combine(ModuleFolder, "config.json");

            var config=JsonConvert.DeserializeObject<Config>(File.ReadAllText(options.ConfigFile));
            FileWatcher fw = new FileWatcher();
            fw.StartWatching(config);

        }
    }
}
