using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace DiagramLib
{
    
    public class Settings<T> where T: new()
    {
        public static T ModelFromSettings(string filename, IEnumerable<Type> knownTypes)
        {
            try
            {
                var ser = new DataContractSerializer(typeof(T), knownTypes,
            0x7FFF /*maxItemsInObjectGraph*/,
            false /*ignoreExtensionDataObject*/,
            true /*preserveObjectReferences : this is where the magic happens */,
            null /*dataContractSurrogate*/);
                if (!File.Exists(filename))
                    return new T();
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    T model = (T)ser.ReadObject(fs);
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

        public static void SaveModel(T appModel, string filename, IEnumerable<Type> knownTypes )
        {
            try
            {
         
                var ser = new DataContractSerializer(typeof(T), knownTypes,
            0x7FFF /*maxItemsInObjectGraph*/,
            false /*ignoreExtensionDataObject*/,
            true /*preserveObjectReferences : this is where the magic happens */,
            null /*dataContractSurrogate*/);
                var settings = new XmlWriterSettings { Indent = true };

                using (var w = XmlWriter.Create(filename, settings))
                    ser.WriteObject(w, appModel);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Failed to save settings to !: " + ex.ToString());
            }
        }
    }
}
