using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SQLDepLib
{
    public class FileSystemData
    {

        public FSConfFile ConfFile { get; set; }

        public FileSystemData()
        {
            ConfFile = new FSConfFile();
        }

        public void Load(string filename = "file_system.cfg")
        {
            try
            {
                string json = File.ReadAllText(filename);
                ConfFile = JsonConvert.DeserializeObject<FSConfFile>(json);
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not parse configuration file. Have you created file_system.conf");
            }
        }

        public void Save(string filename = "file_system.cfg")
        {
            try
            {
                string json = JsonConvert.SerializeObject(ConfFile);
                File.WriteAllText(filename, json);
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not save configuration file");
            }
        }
    }

    public class FSConfFile
    {
        public string InputDir { get; set; }
        public string FileMask { get; set; }
        public string DefaultSchema { get; set; }
        public string DefaultDatabase { get; set; }
    }
}
