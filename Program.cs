using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backup_Generator.Core;

namespace Backup_Generator
{
    class Program
    {
        static string entryFile = "source.txt";
        static string destinationFile = "destination.txt";
        static string keyFile = "key.txt"; 

        static void Main(string[] args)
        {
            //Credits of application 
            Console.Title = "Backup Generator 1.0 Beta";
            Console.WriteLine("Backup Generator 1.0 Beta");
            Console.WriteLine("Developed by Jordan Hook");
            Console.WriteLine("===");

            //some variables for settings;
            SecureBackupProvider sbp; 
            Settings setting; 
            string dstFolder = string.Empty;
            List<string> srcFolders = new List<string>(); 

            //Load the settings information 
            Console.WriteLine("Loading backup configurations...");
            dstFolder = backupDestination(); //load the destination path for the config file.. 
            setting = new Settings(dstFolder, keyFileLoad()); //initialize a new settings class... (can set a new password where AAAbbb###123 is) 

            Console.WriteLine("\nBackup Destination: {0}", dstFolder);
            Console.WriteLine("\nBackup locations: ");
            backupEntries(ref srcFolders); 

            foreach(string location in srcFolders)
            {
                Console.WriteLine(location);
                setting.addPath(location); 
            }

            Console.WriteLine("\n===");

            sbp = new SecureBackupProvider(setting); //create a new secure provider

            if(args.Length == 0) //no arguments so just create a backup 
            {
                sbp.Start();

                Console.WriteLine("Creating backup..."); 

                while(sbp.isRunning)
                {
                    System.Threading.Thread.Sleep(1000); 
                }

                Console.WriteLine("Task complete, a backup was generator!");
            }
            else if (args.Length == 1)
            {
                Console.WriteLine("Decrypting backup file: " + args[0]);

                sbp.decryptBackup(args[0], args[0] + ".decrypted.zip");

                Console.WriteLine("Decrypted the backup: " + args[0] + ".decrypted.zip");
            }
            else if (args.Length == 2)
            {
                if (args[0] == "-decrypt" || args[0] == "-d")
                {
                    Console.WriteLine("Decrypting backup file: " + args[1]);
                    sbp.decryptBackup(args[1], args[1] + ".decrypted.zip"); 
                }

                Console.WriteLine("Decrypted the backup: " + args[1] + ".decrypted.zip"); 

            }


            Console.WriteLine("\n\n\nPress any key to exit...");
            Console.ReadKey(); 
        }

        static void backupEntries(ref List<string> entries)
        {
            string location = string.Empty; 
            using (StreamReader sr = new StreamReader(entryFile))
            {
                while(sr.Peek() > 0) //read the whole file... 
                {
                    location = sr.ReadLine(); //read the entries... 

                    if (Directory.Exists(location)) //only add valid locations to backup... 
                        entries.Add(location); //add the entry to the list... 
                }
            }

        }

        static string keyFileLoad()
        {
            return File.ReadAllText(keyFile);
        }

        static string backupDestination()
        {
            return File.ReadAllText(destinationFile); 
        }
    }
}
