using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;

namespace BimlWatcher
{
    public class Watcher
    {
        static string debugDir = "";

        public static void Main()
        {
            Run();

        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            // If a directory is not specified, exit program.
            if (args.Length != 2)
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: BimlWatcher.exe (output directory)");
                return;
            }

            debugDir = args[1];
            if (!Directory.Exists(debugDir))
                Directory.CreateDirectory(debugDir);

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            var lastWrittenDir = "";
            DateTime lastWriteTime = DateTime.Now.AddDays(-365);
            foreach (var dir in Directory.GetDirectories(Environment.ExpandEnvironmentVariables("%TEMP%") + "\\Varigence"))
            {
                if (Directory.GetLastWriteTime(dir) > lastWriteTime)
                {
                    lastWriteTime = Directory.GetLastWriteTime(dir);
                    lastWrittenDir = dir;
                }
            }
            Console.WriteLine("Watching " + lastWrittenDir);
            watcher.Path = lastWrittenDir;

            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            
            // Only watch cs files.
            watcher.Filter = "*.cs";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            if ( (e.ChangeType == WatcherChangeTypes.Changed) || (e.ChangeType == WatcherChangeTypes.Created))
                File.Copy(e.FullPath, debugDir + "\\" + e.Name, true);
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

       
    }
}
