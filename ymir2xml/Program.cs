using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ymir2xml
{
    class Program
    {

        static bool parseRegenfile(string file)
        {
            string[] lines = System.IO.File.ReadAllLines(file);

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlElement root = doc.CreateElement("ymir2xml");
            XmlElement meta = doc.CreateElement("metadata");

            XmlElement output = doc.CreateElement("out");
            output.InnerText = file;

            XmlElement type = doc.CreateElement("type");
            type.InnerText = "regen";

            meta.AppendChild(output);
            meta.AppendChild(type);
            root.AppendChild(meta);

            XmlElement regen = doc.CreateElement("regen");

            foreach (string line in lines)
            {
                string[] data = line.Split(new char[] { '\t' });

                XmlElement mob = doc.CreateElement("mob");
                mob.SetAttribute("p", data[0]);

                XmlElement tmp;

                if (data.Length != 11) continue;

                tmp = doc.CreateElement("x");
                tmp.InnerText = data[1].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("y");
                tmp.InnerText = data[2].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("dx");
                tmp.InnerText = data[3].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("dy");
                tmp.InnerText = data[4].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("t");
                tmp.InnerText = data[7].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("c");
                tmp.InnerText = data[8].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("n");
                tmp.InnerText = data[9].Trim();
                mob.AppendChild(tmp);

                tmp = doc.CreateElement("v");
                tmp.InnerText = data[10].Trim();
                mob.AppendChild(tmp);

                regen.AppendChild(mob);
            }

            root.AppendChild(regen);
            doc.AppendChild(root);
            doc.Save(file.Substring(0, file.Length - 4) + ".xml");

            return true;
        }

        static bool parseGroupfile(string file)
        {

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlElement root = doc.CreateElement("ymir2xml");
            XmlElement meta = doc.CreateElement("metadata");

            XmlElement output = doc.CreateElement("out");
            output.InnerText = file;

            XmlElement type = doc.CreateElement("type");
            type.InnerText = "group";

            meta.AppendChild(output);
            meta.AppendChild(type);
            root.AppendChild(meta);

            XmlElement groups = doc.CreateElement("groups");

            string pattern = @"Group" + "\t" + @".+?\{.+?\}";
            System.Text.RegularExpressions.Regex expression = new System.Text.RegularExpressions.Regex(pattern,System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.MatchCollection matches = expression.Matches(System.IO.File.ReadAllText(file)/*.Replace("\n", "").Replace("\r", "")*/);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {

                XmlElement group = doc.CreateElement("group");
                
                string code = match.ToString().Trim();
                string groupname = code.Substring(6, code.IndexOf("{") - 6).Trim();

                code = code.Substring(groupname.Length+10,code.Length - groupname.Length - 10).Trim();

                foreach (string line in code.Split(new char[] { '\n' }))
                {
                    string line2 = line.Trim();

                    if (line2 == "") continue;

                    if (line2 == "{" || line2 == "}") continue;

                    string[] data = line2.Split(new char[] { '\t' });

                    XmlElement mob = doc.CreateElement("mob");

                    switch (data[0])
                    {
                        case "Vnum":
                            group.SetAttribute("name", groupname);
                            group.SetAttribute("vnum",data[1]);
                            mob = null;
                            break;

                        case "Leader":
                            mob.InnerText = data[2];
                            mob.SetAttribute("name", data[1]);
                            break;

                        default:
                            mob.InnerText = data[2];
                            mob.SetAttribute("name", data[1]);
                            break;
                    }

                    if (mob != null) group.AppendChild(mob); 

                }

                groups.AppendChild(group);
            }
            Console.WriteLine("count: " + matches.Count);

            root.AppendChild(groups);
            doc.AppendChild(root);
            doc.Save(file.Substring(0, file.Length - 4) + ".xml");

            return true;
        }

        static bool parseGroupGroupfile(string file)
        {
            throw new NotImplementedException();
            //return true;
        }

        static bool parseSetting(string file)
        {
            string[] lines = System.IO.File.ReadAllLines(file);

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlElement root = doc.CreateElement("ymir2xml");
            XmlElement meta = doc.CreateElement("metadata");

            XmlElement output = doc.CreateElement("out");
            output.InnerText = file;

            XmlElement type = doc.CreateElement("type");
            type.InnerText = "setting";

            meta.AppendChild(output);
            meta.AppendChild(type);
            root.AppendChild(meta);

            XmlElement setting = doc.CreateElement("setting");

            foreach (string line in lines)
            {
                string[] data = line.Split(new char[] { '\t' });
                XmlElement tmp;

                if (data.Length > 0 && data[0].Trim() == "") continue;

                tmp = doc.CreateElement(data[0]);

                if (data[0] == "MapSize" || data[0] == "BasePosition")
                {
                    XmlElement tmp2 = doc.CreateElement("x");
                    tmp2.InnerText = data[1];
                    tmp.AppendChild(tmp2);

                    tmp2 = doc.CreateElement("y");
                    tmp2.InnerText = data[2];
                    tmp.AppendChild(tmp2);
                }
                else
                {
                    tmp.InnerText = data[1];
                }

                setting.AppendChild(tmp);
            }

            root.AppendChild(setting);
            doc.AppendChild(root);
            doc.Save(file.Substring(0, file.Length - 4) + ".xml");

            return true;
        }

        static bool parseXML(string file)
        {

            string outfile = "", type = "";

            System.IO.FileStream outstream = null;
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNode root = doc.GetElementsByTagName("ymir2xml")[0];

            foreach(XmlNode node in root.ChildNodes)
            {
                if (node.Name == "metadata")
                {
                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "out")
                        {
                            outfile = node2.InnerText;
                            outstream = new System.IO.FileStream(outfile, System.IO.FileMode.Create);
                        }
                        else if (node2.Name == "type")
                        {
                            type = node2.InnerText;
                        }
                    }
                }

                else if (type == "regen" && node.Name == "regen")
                {
                    foreach (XmlNode mobdata in node.ChildNodes)
                    {
                        string prefix = mobdata.Attributes["p"].Value,
                            x = "",
                            y = "",
                            dx = "",
                            dy = "",
                            time = "",
                            chance = "",
                            count = "",
                            vnum = "";

                        foreach (XmlNode data in mobdata.ChildNodes)
                        {
                            switch (data.Name)
                            {
                                case "x":
                                    x = data.InnerText;
                                    break;

                                case "y":
                                    y = data.InnerText;
                                    break;

                                case "dx":
                                    dx = data.InnerText;
                                    break;

                                case "dy":
                                    dy = data.InnerText;
                                    break;

                                case "t":
                                    time = data.InnerText;
                                    break;

                                case "c":
                                    chance = data.InnerText;
                                    break;

                                case "n":
                                    count = data.InnerText;
                                    break;

                                case "v":
                                    vnum = data.InnerText;
                                    break;
                            }
                        }
                        byte[] buffer = Encoding.UTF8.GetBytes(prefix + "\t" + x + "\t" + y + "\t" + dx + "\t" + dy + "\t0\t0\t" + time + "\t" + chance + "\t" + count + "\t" + vnum + "\n");
                        outstream.Write(buffer, 0, buffer.Length);
                    }
                }
                else if (type == "setting" && node.Name == "setting")
                {
                    foreach(XmlElement node2 in node.ChildNodes)
                    {
                        byte[] buffer;
                        if (node2.Name == "BasePosition" || node2.Name == "MapSize")
                        {
                            buffer = Encoding.UTF8.GetBytes(node2.Name + "\t" + node2.ChildNodes[0].InnerText + "\t" + node2.ChildNodes[1].InnerText + "\n");
                        }
                        else
                        {
                            buffer = Encoding.UTF8.GetBytes(node2.Name + "\t" + node2.InnerText + "\n");
                        }
                        outstream.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            outstream.Flush();
            outstream.Close();

            return true;
        }

        static void Main(string[] args)
        {

            Console.WriteLine("ymir2xml converter by xenor");
            Console.WriteLine("Copyright (c) 2012 - All Rights reserved.");
            Console.WriteLine("-----------------------------------------");

            args = new string[] { "group.txt" };

            if (args.Length == 0)
            {
                Console.WriteLine("Drag and drop files to the executable to begin.");
            }
            else
            {
                foreach (string filename in args)
                {
                    string relFilename = filename;
                    try
                    {
                        relFilename = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\").MakeRelativeUri(new Uri(filename)).ToString();
                    }
                    catch { }

                    if (relFilename.Substring(relFilename.Length - 3, 3) == "xml")
                    {
                        Console.WriteLine("parse xml file");
                        parseXML(filename);
                    }
                    else if ((relFilename.Length >= 4 && relFilename.Substring(0, 4) == "boss") || (relFilename.Length >= 3 && relFilename.Substring(0, 3) == "npc") || (relFilename.Length >= 5 && relFilename.Substring(0, 5) == "regen") || (relFilename.Length >= 5 && relFilename.Substring(0, 5) == "stone"))
                    {
                        Console.WriteLine("parse regen file");
                        parseRegenfile(filename);
                    }
                    else if (relFilename.Length >= 11 && relFilename.Substring(0, 11) == "group_group")
                    {
                        Console.WriteLine("parse group_group file");
                        parseGroupGroupfile(filename);
                    }
                    else if (relFilename.Length >= 5 && relFilename.Substring(0, 5) == "group")
                    {
                        Console.WriteLine("parse group file");
                        parseGroupfile(filename);
                    }
                    else if (relFilename.Length >= 7 && (relFilename.Substring(0, 7) == "Setting" || relFilename.Substring(0, 7) == "setting"))
                    {
                        Console.WriteLine("parse settings file");
                        parseSetting(filename);
                    }
                    else
                    {
                        Console.WriteLine(relFilename + " - Unknown filetype");
                    }
                }
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
