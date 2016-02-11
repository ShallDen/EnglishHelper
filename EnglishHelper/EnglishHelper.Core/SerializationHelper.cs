using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace EnglishHelper.Core
{
    public class SerializationHelper
    {
        public static void Serialize(string filename, Object obj)
        {
            try
            {
                XmlSerializer xmlFormat = new XmlSerializer(obj.GetType());
                
                using (var fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    xmlFormat.Serialize(fStream, obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static Object Deserialize(string filename, Type type)
        {
            try
            {
                XmlSerializer xmlFormat = new XmlSerializer(type);
                using (var fStream = File.OpenRead(filename))
                {
                    return xmlFormat.Deserialize(fStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
