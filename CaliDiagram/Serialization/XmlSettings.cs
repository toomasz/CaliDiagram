using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace CaliDiagram.Serialization
{
    /// <summary>
    /// Using data contract serializer to load or save object tree into xml files
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlSettings<T> where T: new()
    {
        DataContractSerializer serializer;
        public XmlSettings(IEnumerable<Type> knownTypes)
        {
            serializer = new DataContractSerializer(typeof(T), knownTypes,
            0x7FFF /*maxItemsInObjectGraph*/,
            false /*ignoreExtensionDataObject*/,
            true /*preserveObjectReferences : this is where the magic happens */,
            null /*dataContractSurrogate*/);
        }
        public T ModelFromSettings(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    return new T();
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    T model = (T)serializer.ReadObject(fs);
                   // model.SettingsFilename = filename;
                    return model;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load settings from " + filename + " !: " + ex.ToString());
                //MessageBox.Show("Failed to load settings from " + filename + " !: " + ex.InnerException.Message);
                return new T();
            }
        }

        public void SaveModel(T appModel, string filename)
        {
            try
            {
                var settings = new XmlWriterSettings { Indent = true };

                using (var w = XmlWriter.Create(filename, settings))
                    serializer.WriteObject(w, appModel);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Failed to save settings to !: " + ex.ToString());
            }
        }
    }
}
