using SafeILGenerator.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast
{
	public class AstGenerator : IAstGenerator
	{
		private AstGenerator()
		{
		}

		static public AstGenerator Instance = new AstGenerator();
	}
}
