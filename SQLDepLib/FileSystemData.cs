using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SQLDepLib
{
    class FileSystemData
    {

        public FSConfFile ConfFile { get; set; }
        public void Load(string filename = "file_system.cfg")
        {
            try
            {
                string json = File.ReadAllText(filename);
                ConfFile = JsonConvert.DeserializeObject<FSConfFile>(json);
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not parse configuration file");
            }


        }
    }

    class FSConfFile
    {
        public string InputDir { get; set; }
        public string FileMask { get; set; }
        public string DefaultSchema { get; set; }
        public string DefaultDatabase { get; set; }
    }
}
