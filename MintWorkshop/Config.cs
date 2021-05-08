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
        public readonly string ConfigVersion = "0.4";

        public bool UppercaseMnemonics = false;
        public float FontSize = 9;

        public Config()
        {
            UppercaseMnemonics = false;
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

            XmlElement fontsize = xml.CreateElement("FontSize");
            fontsize.InnerText = FontSize.ToString();
            root.AppendChild(fontsize);

            xml.AppendChild(root);

            xml.Save(filepath);
        }
    }
}
