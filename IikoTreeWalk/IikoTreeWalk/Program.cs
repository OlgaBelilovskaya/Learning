using System;
using System.Collections.Generic;
using System.Linq;


namespace IikoTreeWalk
{
	class Program
	{
		public class Node
		{
			private readonly List<Node> _children;

			private string _name;

			public Node(string name)
			{
				_children = new List<Node>();
				_name = name;
			}

			public void Print()
			{
				Console.WriteLine(this._name + Environment.NewLine);
			}

			public void AddChild(string name)
			{
				_children.Add(new Node(name));
			}

			public int GetChildNodesCount()
			{
				return _children.Count;
			}

			public Node GetChildNodeByIndex(int index)
			{
				if(index < _children.Count())
				{
					return _children[index];
				}

				return null;
			}

			public void TreeWalk()
			{
				this.Print();

				for(int i = 0; i < this.GetChildNodesCount(); i++)
				{
					this.GetChildNodeByIndex(i).TreeWalk();
				}
			}
		}

		static void Main(string[] args)
		{

			int i = 0;
			try
			{
				i++;
			}
			catch(Exception ex)
			{ }
			finally
			{
				i++;
			}


			var root = new Node("1");

			root.AddChild("2");
			root.AddChild("3");
			root.AddChild("4");

			root.GetChildNodeByIndex(0).AddChild("5");
			root.GetChildNodeByIndex(0).AddChild("6");

			root.GetChildNodeByIndex(0).GetChildNodeByIndex(0).AddChild("7");
			root.GetChildNodeByIndex(2).AddChild("8");
			root.GetChildNodeByIndex(2).AddChild("9");
			root.GetChildNodeByIndex(2).AddChild("10");

			root.GetChildNodeByIndex(2).GetChildNodeByIndex(1).AddChild("11");
			root.GetChildNodeByIndex(2).GetChildNodeByIndex(1).AddChild("12");
			
			root.TreeWalk();
		}
	}
}
