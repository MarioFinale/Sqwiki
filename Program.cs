using System;
using System.Linq;
using MWBot.net.WikiBot;
using MWBot.net;
using System.Text.RegularExpressions;
using System.Net;

namespace Sqwiki
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalVars.Log_Filepath = GlobalVars.Exepath + "sqlog.psv";
            String tdir = GlobalVars.Exepath + GlobalVars.DirSeparator + "sqs";
            ConfigFile cfg = new ConfigFile(GlobalVars.ConfigFilePath);
            Bot Workerbot = new Bot(cfg);
           
            do {
                System.Threading.Thread.Sleep(500);
                if (!(System.IO.File.Exists(GlobalVars.Exepath + GlobalVars.DirSeparator + "sqwiki.runme"))){
                    continue;
                }
                System.IO.File.Delete(GlobalVars.Exepath + GlobalVars.DirSeparator + "sqwiki.runme");
                int sqindex = 1;
                for (int i = 1; i < 350; i++) {

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

                    foreach (Match tmatch in tmatches) {
                        string tsq = tmatch.ToString().Trim().Replace("&#8239;", "");
                        tsq = Utils.ReplaceFirst(tsq, "...", "¿Sabías que");
                        tsq = tsq.Replace("'''", "");
                        tsq = tsq.Replace("''", "");
                        tsq = Regex.Replace(tsq, @"(&.{4};)", @" ");
                        MatchCollection links = Regex.Matches(tsq, @"(\[\[)(.+?)(\]\])");
                       
                        foreach (Match link in links) {
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
                            System.IO.File.WriteAllText(tdir + GlobalVars.DirSeparator + sqindex + ".htm",thtm);
                            System.IO.File.WriteAllText(tdir + GlobalVars.DirSeparator + sqindex + ".c.htm", pimg.Uri.OriginalString);
                            pimg.Save(tdir + GlobalVars.DirSeparator + sqindex + ".png");

                            sqindex += 1;
                        }
                       
                    }                               
                }
                sqindex = 0;
                System.IO.File.Delete(GlobalVars.Log_Filepath);
                System.IO.File.Create(GlobalVars.Log_Filepath).Close();
            } while (true);
        }
       static Tuple<String, String> GetLinkText(string tlink)
        {
            string tstr = tlink.Replace("[[", "").Replace("]]", "");
            if (tstr.Contains("|")) {
                Tuple<String, String> tval = new Tuple<String, String>(tstr.Split('|')[0].Trim(), tstr.Split('|')[1].Trim());
                return tval;
            }
            return new Tuple<String, String>(tstr, tstr);
        }
    }
}
