using MWBot.net;
using MWBot.net.WikiBot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sqwiki
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string Log_Filepath = Utils.Utils.Exepath + "sqlog.psv";
            string ConfigFile = Utils.Utils.Exepath + "Config.cfg";

            String tdir = Utils.Utils.Exepath + Utils.Utils.DirSeparator + "sqs";         
            Bot Workerbot = new Bot(ConfigFile, Log_Filepath);
            string tfilename = Utils.Utils.Exepath + Utils.Utils.DirSeparator + "reqs" + Utils.Utils.DirSeparator  + "sqwiki.runme";

            do
            {
                System.Threading.Thread.Sleep(500);
                if (!(System.IO.File.Exists(tfilename)))
                {
                    continue;
                }
                System.IO.File.Delete(tfilename);
                int sqindex = 1;
                for (int i = 1; i < 350; i++)
                {

                    if (!(System.IO.Directory.Exists(tdir)))
                    {
                        System.IO.Directory.CreateDirectory(tdir);
                    }

                    string pagename = @"Template:SQ/" + i;
                    Page tpage = Workerbot.Getpage(pagename);
                    string ttext = tpage.Content;
                    string tregexp = @"(\<noinclude\>)([\s\S]+?)(\<\/noinclude\>)";
                    ttext = Regex.Replace(ttext, tregexp, "").Trim();
                    MatchCollection tmatches = Regex.Matches(ttext, @"([\n\r]\.\.\.)([\s\S]+?)(&#8239;)");

                    foreach (Match tmatch in tmatches)
                    {
                        string tsq = tmatch.ToString().Trim().Replace("&#8239;", "");
                        tsq = Utils.Utils.ReplaceFirst(tsq, "...", "¿Sabías que");
                        tsq = tsq.Replace("'''", "");
                        tsq = tsq.Replace("''", "");
                        tsq = Regex.Replace(tsq, @"(&.{4};)", @" ");
                        MatchCollection links = Regex.Matches(tsq, @"(\[\[)(.+?)(\]\])");

                        foreach (Match link in links)
                        {
                            Tuple<String, String> tval = GetLinkText(link.Value);
                            tsq = tsq.Replace(link.Value, tval.Item2);
                        }
                        string fpagename = GetLinkText(links[0].Value).Item1;
                        Page sqpage = Workerbot.Getpage(fpagename);

                        if (!(String.IsNullOrWhiteSpace(sqpage.Thumbnail)))
                        {
                            Image pimg = new Image(sqpage.Thumbnail, ref Workerbot);

                            string tlic = pimg.License;
                            string tlicurl = " (" + pimg.LicenseUrl + ")";
                            if (tlic.ToLower().Contains("public domain"))
                            {
                                tlic = "En dominio público.";
                                tlicurl = "";
                            }

                            tsq = tsq + Environment.NewLine + Environment.NewLine +
                                @"Enlace al artículo: " + sqpage.URL + Environment.NewLine +
                                @"Enlace a la imagen completa: " + pimg.Uri.OriginalString + Environment.NewLine + Environment.NewLine +
                                @"Imagen por: " + pimg.Author.TrimEnd('.') + "." + Environment.NewLine +
                                @"Licencia: " + tlic + tlicurl;

                            string thtm = Properties.Resources.txthead + tsq + Properties.Resources.txttail;
                            System.IO.File.WriteAllText(tdir + Utils.Utils.DirSeparator + sqindex + ".htm", thtm);
                            System.IO.File.WriteAllText(tdir + Utils.Utils.DirSeparator + sqindex + ".c.htm", pimg.Uri.OriginalString);
                            pimg.Save(tdir + Utils.Utils.DirSeparator + sqindex + ".png");
                            sqindex += 1;
                        }

                    }
                }
                sqindex = 0;
                System.IO.File.Delete(Log_Filepath);
                System.IO.File.Create(Log_Filepath).Close();
            } while (true);
        }

        private static Tuple<String, String> GetLinkText(string tlink)
        {
            string tstr = tlink.Replace("[[", "").Replace("]]", "");
            if (tstr.Contains("|"))
            {
                Tuple<String, String> tval = new Tuple<String, String>(tstr.Split('|')[0].Trim(), tstr.Split('|')[1].Trim());
                return tval;
            }
            return new Tuple<String, String>(tstr, tstr);
        }

        private string[] AnalyzeLine(string ttext)
        {
            List<string> tlist = new List<string>();
                                          



            return tlist.ToArray();
        }



    }



}
