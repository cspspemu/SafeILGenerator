using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Serializers
{
	public class AstSerializer
	{
		static public string Serialize(AstNode Node)
		{
			var Parameters = new List<string>();
			foreach (var Child in Node.Childs)
			{
				Parameters.Add(Serialize(Child));
			}
			return Node.GetType().Name + "(" + String.Join(", ", Parameters) + ")";
		}
	}
}
