using System.IO;
using System;
using System.Diagnostics;
using System.Threading;
using Core.Model.Business;

namespace Core.Model.Service.SaveStrategy
{
    public class CompleteSave : Strategy
    {
        private const String logFilePath = "..\\..\\..\\log.json";
        private const String StatePausePath = "ThreadState.txt";
        private DateTime NowDate = DateTime.Now;
        public static int test = 0;

        public  static void ThreadState ( int i)
        {
            test = i;
        }
        public override void SaveAlgorithm(string name, string sourceDirectory, string destinationDirectory, string lastCompleteDirectory, Nko obj)
        {

           string CalculatorOn = "";
            // Check if the calculator is running 

            if (CalculatorOn == "Calculator")
            {
                Trace.WriteLine("Your calculator is on, ");
            }
            else
            {

                Trace.WriteLine(name);
                Trace.WriteLine(sourceDirectory);
                Thread.Sleep(2000);
                Trace.WriteLine(destinationDirectory);
                Trace.WriteLine(lastCompleteDirectory);
                Thread.Sleep(2000);
                // Start the backup
                // We get the today date for the name of our backups  
                String date = NowDate.ToString("yyyy-MM-dd,HH.mm.ss");
                // Here we get the informations about the source directory and the Destination Directory and he we get all the source file from the source directory
                string SourceDirectory = sourceDirectory;
                string DestinationDirectory = destinationDirectory + name + " Le." + date;

                string[] originalFiles = Directory.GetFiles(SourceDirectory, "*", SearchOption.AllDirectories);
                // We check if the source directory is empty to prevent the creation of empty backups
                Thread.Sleep(2000);
                Trace.WriteLine("on continu");

                if (originalFiles.Length != 0)
                {
                    System.IO.Directory.CreateDirectory(DestinationDirectory);
                    Nko.limit = 1000;
                    // Here we copy the file in the directory that we created just above
                    Array.ForEach(originalFiles, (originalFileLocation) =>
                    {
                        Thread.Sleep(2000);
                        while ( test == 1)
                        {
                            Trace.WriteLine("PAUSE");
                            Thread.Sleep(2000);
                            
                        }
                        if ( test == 3)
                        {
                            Trace.WriteLine("stop");
                        }
                        bool isProcessAlive = ProcessList.IsProcessExist();
                        Trace.WriteLine(isProcessAlive);
                        while (isProcessAlive == true )
                        {

                            Thread.Sleep(2000);
                            isProcessAlive = ProcessList.IsProcessExist();
                            if (isProcessAlive == false)
                            {
                                Trace.WriteLine("ON PEUT CONTINUER");
                            }
                        }


                        
                        Trace.WriteLine("paS PAUSE");
                        FileInfo sourceFile = new FileInfo(originalFileLocation);
                        FileInfo destFile = new FileInfo(originalFileLocation.Replace(SourceDirectory, DestinationDirectory));
                        // Here we copy everything and we create directory if needed
                        if (destFile.Exists && sourceFile.Length < Nko.limit)
                        {
                            sourceFile.CopyTo(destFile.FullName, true);
                        }
                        else if (!destFile.Exists && sourceFile.Length < Nko.limit)
                        {
                            Directory.CreateDirectory(destFile.DirectoryName);
                            sourceFile.CopyTo(destFile.FullName, false);
                        }
                        if (sourceFile.Length > Nko.limit)
                        {
                            Monitor.Enter(obj);
                            try
                            {
                                Trace.WriteLine("Je suis dans le monitor");
                                if (destFile.Exists)
                                { 
                                    sourceFile.CopyTo(destFile.FullName, true);
                                } 
                                else if (!destFile.Exists)
                                {
                                    Directory.CreateDirectory(destFile.DirectoryName);
                                    sourceFile.CopyTo(destFile.FullName, false);
                                }
                               // Thread.Sleep(10000);
                            }
                            // catch blocks go here.
                            finally
                            {
                                Trace.WriteLine("Fin du monitor");
                                Monitor.Exit(obj);
                            }
                        }

                    });
                }
                else if (originalFiles.Length == 0)
                // We check if the source directory is empty
                {
                    Trace.WriteLine("Source directory empty");
                }
                else
                {
                    Trace.WriteLine("error");
                }
                CryptoSoft.CryptoSoft.Crypto(DestinationDirectory);

                long size = 0;
                long currentSize = 0;
                int i = -1;
                int j = 0;

                // Counts the number of files in the source folder
                foreach (string srcFile in originalFiles)
                {
                    size = new FileInfo(srcFile).Length;
                }

                bool status = true;

                // Write Info for StateFile when Job is On
                while (i < j)
                {
                    for (j = 0; j <= originalFiles.Length; j++)
                    {
                        string srcFile = originalFiles[j];
                        currentSize = new FileInfo(srcFile).Length;

                        SaveState.OpenFile();
                        SaveState.SaveTime(date);
                        SaveState.SaveName(DestinationDirectory);
                        SaveState.SaveStatus(status);
                        SaveState.FileCount(originalFiles);
                        SaveState.SaveSize(size);
                        SaveState.SaveProgress(j, originalFiles);
                        SaveState.FileCountLeft(j, originalFiles);
                        SaveState.SaveSizeLeft(size, currentSize);
                        SaveState.SaveSourceFile(j, originalFiles);
                        SaveState.SaveDestination(DestinationDirectory);
                        SaveState.CloseFile();
                        i++;
                    }
                }

                // Write Info for StateFile when Job is Off
                status = false;

                SaveState.OpenFile();
                SaveState.SaveTime(date);
                SaveState.SaveName(DestinationDirectory);
                SaveState.SaveStatus(status);
                SaveState.CloseFile();

                for (int x = 0; x < originalFiles.Length; x++)
                {
                    string srcFile = originalFiles[x];
                    currentSize = new FileInfo(srcFile).Length;
                    long msbefore = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    using (StreamWriter w = File.AppendText(logFilePath))
                    {
                        Log.Logger(name, srcFile, DestinationDirectory, currentSize, DateTimeOffset.Now.ToUnixTimeMilliseconds() - msbefore, w);
                    }
                }
            }
        }

    }
}





