using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backup_Generator.Core
{
    class SecureBackupProvider
    {
        public Settings Settings { get { return _settings; } }
        private Settings _settings; 
        public bool isRunning { get { return _running; } }
        private bool _running;

        private Thread backupThread; 

        public SecureBackupProvider(Settings settings)
        {
            _settings = settings; 
        }

        public void Start()
        {
            if (isRunning) //if already running exit 
                return;

            _running = true;

            backupThread = new Thread(run); //create a new thread for the backup process 
            backupThread.IsBackground = true; //run it in the background... 
            backupThread.Start(); //run the thread 
        }

        public void Stop()
        {
            if (!isRunning) //if already stopped exit 
                return;

            _running = false; 
        }

        public void decryptBackup(string input, string output)
        {
            FileFunctions.decryptFile(input, output, Settings.Password, "ApacheGoldBloom18321286$$");
        }

        private void run()
        {
            string temp = Settings.destinationDirectory;
            Directory.CreateDirectory(temp);

            if (!Directory.Exists(temp))
                throw new Exception("Could not create required backup paths"); 

            foreach(string path in Settings.Locations) //for each path we want to backup... 
            {
                if(_running)
                    FileFunctions.CopyDir(path, temp);
            }

            //using (ZipFile compressor = new ZipFile())
            //{
            //    compressor.UseUnicodeAsNecessary = true;
            //    compressor.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            //    compressor.AddDirectory(temp);
            //    compressor.Save(Settings.destinationFile + ".zip");
            //}

            ZipFile.CreateFromDirectory(temp, Settings.destinationFile + ".zip", CompressionLevel.Optimal, false);

            Directory.Delete(temp, true);

            FileFunctions.encryptFile(Settings.destinationFile + ".zip", Settings.destinationFile + ".zip.enc", Settings.Password, "ApacheGoldBloom18321286$$");

            File.Delete(Settings.destinationFile + ".zip"); 

            _running = false; 
        }
    }
}
