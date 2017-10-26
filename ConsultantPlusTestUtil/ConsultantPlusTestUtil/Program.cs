using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantPlusTestUtil
{
    class Program
    {
        static void Main()
        {
            string fileName = "test.txt";
            string sourcePath = @"C:\Users\erokh\OneDrive\Документы\OlechkaBe\Disk1";
            string targetPath = @"C:\Users\erokh\OneDrive\Документы\OlechkaBe\Disk2";

            const string MainDirectoryName = "WRK";

            var date = DateTime.Now.Date;
            string dateStr = (date.Month).ToString() + (date.Day).ToString();

            //System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(targetPath);

            //Если директории targetPath не существует, прерываем выполнение программы
            if (!System.IO.Directory.Exists(targetPath))
            {
                Console.WriteLine($"Directory doesn't exist: {targetPath}");
                return;
            }

            //Считаем названия всех папок внутри targetPath
            string[] folders = System.IO.Directory.GetDirectories(targetPath);

            if(folders.Length == 0)
            {
                Console.WriteLine($"There're no files in {targetPath}");
            }

            DateTime oldestTime = DateTime.Now;
            string oldestFolder = folders[0];

            DateTime newestTime = DateTime.MinValue;
            string earliestFolder = folders[0];

            //Найдем самую старую и самую новую папку
            foreach (string folder in folders)
            {
                var creationTime = System.IO.Directory.GetCreationTime(folder);

                if (creationTime <= oldestTime)
                {
                    oldestTime = creationTime;
                    oldestFolder = folder;
                }

                if (creationTime >= newestTime)
                {
                    newestTime = creationTime;
                    earliestFolder = folder;
                }
            }

            //Предполагается, что время создания папок не совпадает 
            if (oldestFolder == earliestFolder)
            {
                Console.WriteLine("The newest and oldest folders should not be the same.");
                return;
            }
            
            //Удалим самую старую папку
            try
            {
                System.IO.Directory.Delete(oldestFolder, true);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

            //Переименуем самую новую папку из WRK в WRK_xx.xx
            var yesterday = DateTime.Now.Date.AddDays(-1);
           
            string yesterdayStr = (yesterday.Month).ToString() + (yesterday.Day).ToString();

            string newFolder = String.Concat(earliestFolder, "_", yesterdayStr);
                
            try
            {                
                System.IO.Directory.Move(earliestFolder, newFolder);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
            
            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            //if (!System.IO.Directory.Exists(targetPath))
            //{
            //    System.IO.Directory.CreateDirectory(targetPath);
            //}
                      
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(sourcePath, MainDirectoryName));

                foreach (string file in files)
                {
                    fileName = System.IO.Path.GetFileName(file);
                    var destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(file, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}

