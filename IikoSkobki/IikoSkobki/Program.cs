using System;
using System.Collections.Generic;
using System.Linq;

namespace IikoSkobki
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Enter text of brackets");
			var text = Console.ReadLine().ToCharArray();

			Stack<char> brackets = new Stack<char>();

			char item = '\0';

			for(int i = 0; i < text.Count(); i++)
			{
				item = text[i];
				
				if(item == '(' || item == '[')
				{
					brackets.Push(item);
					continue;				
				}

				//Коды '[' и ']' отличаются на 2
				if(brackets.Count == 0 || Math.Abs((int)brackets.Pop() - (int)item) > 2)
				{
					Console.WriteLine("Brackets are unpaired.");
					Console.ReadKey();
					return;
				}		
			}

			if(brackets.Count > 0)
			{
				Console.WriteLine("Brackets are unpaired.");				
			}
			else
			{
				Console.WriteLine("Brackets are okok.");
			}

			Console.ReadKey();
		}
	}
}
