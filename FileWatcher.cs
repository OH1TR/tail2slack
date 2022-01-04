using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace tail2slack
{
    class FileWatcher
    {
        public void StartWatching(Config config)
        {
            foreach (var f in config.Watch)
            {
                FileInfo fi = new FileInfo(f.Filename);
                if (fi.Exists)
                {
                    f.CurrentPosition = fi.Length;
                    f.LastSeenLength = fi.Length;
                }
                else
                {
                    f.CurrentPosition = 0;
                    f.LastSeenLength = 0;
                }
            }

            while (true)
            {
                foreach (var f in config.Watch)
                {
                    var lines = GetLines(f);
                    foreach (var line in lines)
                    {
                        bool toSlack = true;

                        foreach (var rule in f.Match)
                        {
                            bool match=Regex.IsMatch(line, rule.Match);

                            if(match&&rule.Action=="ACCEPT")
                            {
                                toSlack = true;
                                break;
                            }

                            if (match && rule.Action == "DROP")
                            {
                                toSlack = false;
                                break;
                            }
                        }

                        if(toSlack)
                        {
                            SendSlack(config,"tail2slack", line);
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);

            }
        }

        public void SendSlack(Config config, string area, string line)
        {
            SlackClient sc = new SlackClient(config.SlackAPI);
            sc.PostMessage(new Payload() { Channel = config.SlackChannel, Username = area, Text = line });
            System.Threading.Thread.Sleep(500);
        }

        public List<string> GetLines(FileWatch fw)
        {
            List<string> result = new List<string>();
            FileInfo fi = new FileInfo(fw.Filename);

            if (!fi.Exists)
            {
                fw.LastSeenLength = 0;
                return result;
            }

            if (fi.Length < fw.LastSeenLength)
            {
                fw.CurrentPosition = 0;
            }

            if (fi.Length > fw.LastSeenLength)
            {
                byte[] by;
                using (BinaryReader b = new BinaryReader(File.Open(fw.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete)))
                {
                    b.BaseStream.Seek(fw.CurrentPosition, SeekOrigin.Begin);
                    by = b.ReadBytes((int)(fi.Length - fw.CurrentPosition));
                }

                int pos = 0;
                for (int i = 0; i < by.Length; i++)
                {
                    if (by[i] == 10 || by[i] == 13)
                    {
                        if (pos < i)
                        {
                            result.Add(Encoding.ASCII.GetString(by, pos, i - pos));
                            Console.WriteLine(":" + Encoding.ASCII.GetString(by, pos, i - pos) + "");
                        }
                        pos = i + 1;
                    }
                }
                fw.CurrentPosition += pos;
            }
            fw.LastSeenLength = fi.Length;

            return result;
        }
    }
}
