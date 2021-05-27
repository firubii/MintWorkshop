using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MintWorkshop
{
    public class Config
    {
        public readonly string ConfigVersion = "0.5";

        public bool UppercaseMnemonics = false;
        public bool OptimizeOnBuild = false;
        public float FontSize = 9;

        public Config()
        {
            UppercaseMnemonics = false;
            OptimizeOnBuild = false;
            FontSize = 9;
        }

        public void Load(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException("Could not find config file");

            XmlDocument xml = new XmlDocument();
            xml.Load(filepath);

            if (xml["Config"].GetAttribute("version") != ConfigVersion)
            {
                Console.WriteLine("Incorrect configuration version detected! Regenerating config.xml.");
                Save(filepath);
                return;
            }

            UppercaseMnemonics = bool.Parse(xml["Config"]["UppercaseMnemonics"].InnerText);
            OptimizeOnBuild = bool.Parse(xml["Config"]["OptimizeOnBuild"].InnerText);
            FontSize = float.Parse(xml["Config"]["FontSize"].InnerText);
        }

        public void Save(string filepath)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", ""));

            XmlElement root = xml.CreateElement("Config");
            root.SetAttribute("version", ConfigVersion);

            XmlElement uppercase = xml.CreateElement("UppercaseMnemonics");
            uppercase.InnerText = UppercaseMnemonics.ToString();
            root.AppendChild(uppercase);

            XmlElement optimize = xml.CreateElement("OptimizeOnBuild");
            optimize.InnerText = OptimizeOnBuild.ToString();
            root.AppendChild(optimize);

            XmlElement fontsize = xml.CreateElement("FontSize");
            fontsize.InnerText = FontSize.ToString();
            root.AppendChild(fontsize);

            xml.AppendChild(root);

            xml.Save(filepath);
        }
    }
}
