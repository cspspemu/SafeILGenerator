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
			Node.TransformNodes((Node2) =>
			{
				Parameters.Add(Serialize(Node2));
				return Node2;
			});
			return Node.GetType().Name + "(" + String.Join(", ", Parameters) + ")";
		}
	}
}
