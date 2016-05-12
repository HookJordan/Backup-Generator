using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Backup_Generator.Core
{
    class FileFunctions
    {
        //use this function to copy directory from src to dst.
        public static void CopyDir(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory + "\\" + diSource.Name + "\\");

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void encryptFile(string input, string output, string pubkey, string privkey)
        {
            Encryption crypt = new Encryption();
            crypt.PublicKey = pubkey;
            crypt.PrivateKey = privkey;


            byte[] buffer = new byte[8192], enc;//size of the buffer, encryption will add + 1 to the size of the buffer... 
            int read = 1, bufferSize = 0;
            using (FileStream src = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.None)) //open the source file 
            {
                using (FileStream dst = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //dst.WriteByte((byte)bufferSize); //write the size of the buffer 
                    while(read > 0)
                    {
                        read = src.Read(buffer, 0, buffer.Length); //read chunk by chunk 

                        if (read != buffer.Length)
                            Array.Resize(ref buffer, read); //resize the buffer if it's different... 

                        enc = crypt.Encrypt(buffer);

                        if (bufferSize != enc.Length)
                        {
                            bufferSize = enc.Length;
                            //Console.WriteLine(bufferSize);
                        }

                        if (read > 0)
                        {
                            dst.Write(enc, 0, enc.Length);
                        }
                    }
                }
            }
        }

        public static void decryptFile(string input, string output, string pubkey, string privkey)
        {
            Encryption crypt = new Encryption();
            crypt.PublicKey = pubkey;
            crypt.PrivateKey = privkey;

            byte[] buffer;
            int read = 1;

            using (FileStream src = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.None)) //open the source file 
            {
                using (FileStream dst = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //bufferSize = (int)src.ReadByte(); //read first byte for file size
                    while (read > 0)
                    {
                        buffer = new byte[8200]; 
                        read = src.Read(buffer, 0, buffer.Length); //read chunk by chunk 

                        if (read != buffer.Length)
                            Array.Resize(ref buffer, read); //resize the buffer if it's different... 

                        if (buffer.Length > 0)
                        {
                            buffer = crypt.Decrypt(buffer);

                            dst.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
}
