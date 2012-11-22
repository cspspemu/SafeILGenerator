using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Nodes
{
	public abstract class AstNodeExpr : AstNode
	{
		public Type Type { get { return UncachedType; } }
		protected abstract Type UncachedType { get; }
	}
}
