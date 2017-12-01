using System;
using System.IO;
using System.Configuration;

namespace ConsultantPlusTextEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			//Текст, перед которым мы хотим вставить строку
			string textToFind = ConfigurationManager.AppSettings["textToFind"];
			//Папка с файлами, которые хотим отредактировать
			string dir = ConfigurationManager.AppSettings["dir"];
			//Текст, который хотим вставить перед искомым текстом, вместе с текущей датой.
			string textToInsert = ConfigurationManager.AppSettings["textToInsert"];

			var date = DateTime.Now.ToShortDateString();

			string newLine = textToInsert + " - " + date + Environment.NewLine;

			foreach(var file in Directory.GetFiles(dir))
			{
				string text = File.ReadAllText(file);

				text = text.Replace(textToFind, newLine + textToFind);

				File.WriteAllText(file, text);
			}
		}
	}
}
