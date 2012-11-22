﻿using Codegen.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast
{
	static public class AstFunctionUtils
	{
		static public AstNodeStmAssignLocal AssignLocal(this AstFunction AstFunction, AstLocal Local, AstNodeExpr Expr)
		{
			if (Local.Type != Expr.Type) throw(new Exception("Local.Type != Expr.Type"));
			return new AstNodeStmAssignLocal(Local, Expr);
		}
	}
}