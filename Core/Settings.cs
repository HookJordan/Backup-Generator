using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backup_Generator.Core
{
    class Settings
    {
        public List<string> Locations { get { return paths; } } 
        private List<string> paths;

        public string Password { get { return passkey; } }
        private string passkey;

        public string destinationDirectory { get { return Path.Combine(destination, DateTime.Now.ToString("MM-dd-yyyy") + "\\"); } }
        public string destinationFile { get { return Path.Combine(destination, DateTime.Now.ToString("MM-dd-yyyy") + ".backup"); } }
        private string destination; 

        public Settings(string destinationDirectory, string password = "AAAbbb###123")
        {
            paths = new List<string>();
            passkey = password;

            destination = destinationDirectory; 
        }

        public bool addPath(string path)
        {
            if (!Directory.Exists(path)) //check if it's a real path 
                return false;

            if (paths.Contains(path)) //check if the list already has that path 
                return false;

            paths.Add(path); //add the path 

            return true; 
        }

        public bool removePath(string path)
        {
            if (!paths.Contains(path))
                return false; //no path to remove 

            paths.Remove(path); //remove the path 

            return true; 
        }
    }
}
