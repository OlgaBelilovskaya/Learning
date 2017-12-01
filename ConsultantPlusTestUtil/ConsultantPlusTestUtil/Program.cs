using System;
using System.Configuration;
using System.IO;

namespace ConsultantPlusTestUtil
{
    class Program
	{
		public class FileCopier
		{
			//Откуда копируем
			private readonly string _dir1;
			//Куда копируем
			private readonly string _dir2;
			//Папка, которую копируем
			private readonly string _mainDirName;

			/// <summary>
			/// Папки в _dir2
			/// </summary>
			private string[] _folders;

			public FileCopier(string dir1, string dir2, string mainDirName)
			{
				_dir1 = Path.Combine(dir1, mainDirName);
				_dir2 = dir2;
				_mainDirName = mainDirName;

				//Если в _dir2 нет подпапок, прерываем вполнение программы
				_folders = Directory.GetDirectories(_dir2);

				InitialChecks();
			}

			private void CheckFolder(string dir)
			{
				if(!Directory.Exists(dir))
				{
					Console.WriteLine($"Folder {dir} doesn't exist.");
					return;
				}
			}

			private void InitialChecks()
			{
				//Если директории _dir1 или _dir2 не существует, прерываем выполнение программы
				CheckFolder(_dir1);
				CheckFolder(_dir2);

				//Если в _dir2 нет подпапок, прерываем вполнение программы
				if(_folders.Length == 0)
				{
					Console.WriteLine($"Folder {_dir2} doesn't have subfolders.");
					return;
				}

				//Если в _dir2 не 3 подпапки, прерываем вполнение программы
				if(_folders.Length != 3)
				{
					Console.WriteLine($"Folder {_dir2} should have 3 subfolders.");
					return;
				}
			}
			
			/// <summary>
			/// Удаляет самую старую папку из _dir2 и переименовывает самую молодую папку, добавляя в название вчерашнюю дату.
			/// </summary>
			private void RenameNewestAndDeleteOldestFolder()
			{
				DateTime oldestTime = DateTime.Now;
				string oldestFolder = _folders[0];

				DateTime youngestTime = DateTime.MinValue;
				string youngestFolder = _folders[0];

				//Найдем самую старую и самую новую папку
				foreach(string folder in _folders)
				{
					var creationTime = Directory.GetCreationTime(folder);

					if(creationTime <= oldestTime)
					{
						oldestTime = creationTime;
						oldestFolder = folder;
					}

					if(creationTime >= youngestTime)
					{
						youngestTime = creationTime;
						youngestFolder = folder;
					}
				}

				//По идее время создания папок не должно совпадать
				if(oldestFolder == youngestFolder)
				{
					Console.WriteLine("The newest and oldest folders should not be the same.");
					return;
				}

				//Удалим самую старую папку
				try
				{
					Directory.Delete(oldestFolder, true);
				}
				catch(IOException e)
				{
					Console.WriteLine(e.Message);
				}

				//Переименуем самую новую папку из <name> в <name>_xxxx
				var yesterday = DateTime.Now.Date.AddDays(-1);
				string yesterdayStr = (yesterday.Month).ToString() + (yesterday.Day).ToString();

				string newFolder = String.Concat(youngestFolder, "_", yesterdayStr);

				try
				{
					Directory.Move(youngestFolder, newFolder);
				}
				catch(IOException e)
				{
					Console.WriteLine("There was an error while renaming: " + e.Message);
				}

			}

			/// <summary>
			/// Копирует папку _mainDirName из _dir1 в _dir2
			/// </summary>
			private void CopyFolder()
			{
				string[] sourcefiles = Directory.GetFiles(_dir1);

				var targetFolder = Path.Combine(_dir2, _mainDirName);

				if(!Directory.Exists(targetFolder))
				{
					Directory.CreateDirectory(targetFolder);
				}

				foreach(string file in sourcefiles)
				{
					var fileName = Path.GetFileName(file);
					var destFile = Path.Combine(targetFolder, fileName);
					File.Copy(file, destFile, true);
				}
			}

			public void Run()
			{
				RenameNewestAndDeleteOldestFolder();
				CopyFolder();
			}
		}

        static void Main()
        {
			string sourcePath = ConfigurationManager.AppSettings["dir1"];
			string targetPath = ConfigurationManager.AppSettings["dir2"];
			string dataFolder = ConfigurationManager.AppSettings["dataFoldername"];

			FileCopier filecopier = new FileCopier(sourcePath, targetPath, dataFolder);

			filecopier.Run();

			Console.WriteLine("Folder's copied. Press any key to exit.");
            Console.ReadKey();
        }
    }
}

