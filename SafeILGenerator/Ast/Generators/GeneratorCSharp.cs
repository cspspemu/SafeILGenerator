using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Generators
{
	public class GeneratorCSharp : Generator<GeneratorCSharp>
	{
		protected StringBuilder Output = new StringBuilder();

		public override GeneratorCSharp Reset()
		{
			Output = new StringBuilder();
			return this;
		}

		protected void _Generate(AstNodeExprImm Item)
		{
			var ItemType = Item.Type;
			var ItemValue = Item.Value;
			string StringValue = ItemValue.ToString();

			if (Item.Value is bool)
			{
				StringValue = StringValue.ToLower();
			}
			else if (!AstUtils.IsTypeSigned(ItemType))
			{
				//StringValue = String.Format("0x{0:X8}", ItemValue);
				if (Convert.ToInt64(ItemValue) > 9)
				{
					StringValue = String.Format("0x{0:X}", ItemValue);
				}
			}
			Output.Append(StringValue);
		}

		protected void _Generate(AstNodeExprBinop Item)
		{
			Output.Append("(");
			this.Generate(Item.LeftNode);
			Output.Append(" " + Item.Operator + " ");
			this.Generate(Item.RightNode);
			Output.Append(")");
		}

		protected void _Generate(AstNodeStmExpr Stat)
		{
			Generate(Stat.AstNodeExpr);
			Output.Append(";");
		}

		protected void _Generate(AstNodeExprTerop Item)
		{
			Output.Append("(");
			this.Generate(Item.Cond);
			Output.Append(" ? ");
			this.Generate(Item.True);
			Output.Append(" : ");
			this.Generate(Item.False);
			Output.Append(")");
		}

		protected void _Generate(AstNodeStmEmpty Empty)
		{
		}

		protected void _Generate(AstNodeExprUnop Item)
		{
			Output.Append("(" + Item.Operator);
			this.Generate(Item.RightNode);
			Output.Append(")");
		}

		protected void _Generate(AstNodeStmIfElse IfElse)
		{
			Output.Append("if (");
			this.Generate(IfElse.Condition);
			Output.Append(") ");
			this.Generate(IfElse.True);
			if (IfElse.False != null)
			{
				Output.Append(" else ");
				this.Generate(IfElse.False);
			}
		}

		protected void _Generate(AstNodeStmReturn Return)
		{
			Output.Append("return");
			if (Return.Expression != null)
			{
				Output.Append(" ");
				this.Generate(Return.Expression);
			}
			Output.Append(";");
		}

		protected void _Generate(AstNodeStmAssign Assign)
		{
			Generate(Assign.LValue);
			Output.Append(" = ");
			Generate(Assign.Value);
			Output.Append(";");
		}

		protected void _Generate(AstNodeExprIndirect Assign)
		{
			Output.Append("*(");
			Generate(Assign.PointerExpression);
			Output.Append(")");
		}

		protected void _Generate(AstNodeExprGetAddress GetAddress)
		{
			Output.Append("&");
			Generate(GetAddress.Expression);
		}

		protected void _Generate(AstNodeExprFieldAccess FieldAccess)
		{
			Generate(FieldAccess.Instance);
			Output.Append(".");
			Output.Append(FieldAccess.Field.Name);
		}

		protected void _Generate(AstNodeExprArgument Argument)
		{
			Output.Append(Argument.AstArgument.Name);
		}

		protected void _Generate(AstNodeExprCast Cast)
		{
			Output.Append("(" + Cast.CastedType.Name + ")");
			Generate(Cast.Expr);
		}

		protected void _Generate(AstNodeExprCallTail Tail)
		{
			Output.Append("__tail_call(");
			Generate(Tail.Call);
			Output.Append(")");
		}

		protected void _Generate(AstNodeExprCallStatic Call)
		{
			Output.Append(Call.MethodInfo.DeclaringType.Name + "." + Call.MethodInfo.Name);
			Output.Append("(");
			for (int n = 0; n < Call.Parameters.Length; n++)
			{
				if (n != 0) Output.Append(", ");
				Generate(Call.Parameters[n]);
			}
			Output.Append(")");
		}

		protected void _Generate(AstNodeExprCallInstance Call)
		{
			Generate(Call.Instance);
			Output.Append("." + Call.MethodInfo.Name);
			Output.Append("(");
			for (int n = 0; n < Call.Parameters.Length; n++)
			{
				if (n != 0) Output.Append(", ");
				Generate(Call.Parameters[n]);
			}
			Output.Append(")");
		}

		protected void _Generate(AstNodeStmContainer Container)
		{
			Output.Append("{\n");
			foreach (var Node in Container.Nodes)
			{
				Output.Append("\t");
				Generate(Node);
				Output.Append("\n");
			}
			Output.Append("}\n");
		}

		public override string ToString()
		{
			return Output.ToString();
		}
	}
}
