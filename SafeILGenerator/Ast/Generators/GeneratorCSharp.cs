using SafeILGenerator.Ast.Generators;
using SafeILGenerator.Ast.Nodes;
using SafeILGenerator.Ast.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeILGenerator.Ast.Generators
{
	public class GeneratorCSharp : Generator<GeneratorCSharp>
	{
		protected IndentedStringBuilder Output;

		public override GeneratorCSharp Reset()
		{
			Output = new IndentedStringBuilder();
			return this;
		}

		protected virtual void _Generate(AstNodeExprImm Item)
		{
			var ItemType = Item.Type;
			var ItemValue = Item.Value;
			string StringValue = ItemValue.ToString();

			if (Item.Value is bool)
			{
				StringValue = StringValue.ToLower();
			}
			else if (Item.Value is IntPtr)
			{
				StringValue = String.Format("0x{0:X}", ((IntPtr)Item.Value).ToInt64());
			}
			else if (Item.Value is string)
			{
				StringValue = String.Format("{0}", AstStringUtils.ToLiteral(Item.Value as string));
			}
			else if (!AstUtils.IsTypeSigned(ItemType))
			{
				//StringValue = String.Format("0x{0:X8}", ItemValue);
				if (Convert.ToInt64(ItemValue) > 9)
				{
					StringValue = String.Format("0x{0:X}", ItemValue);
				}
			}
			Output.Write(StringValue);
		}

		protected virtual void _Generate(AstNodeStmComment Comment)
		{
			Output.Write("// " + Comment.CommentText + "\n");
		}

		protected virtual void _Generate(AstNodeExprBinop Item)
		{
			Output.Write("(");
			this.Generate(Item.LeftNode);
			Output.Write(" " + Item.Operator + " ");
			this.Generate(Item.RightNode);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeStmGotoIfTrue Goto)
		{
			Output.Write("if (");
			Generate(Goto.Condition);
			Output.Write(") ");
			Output.Write("goto ");
			Output.Write("Label_" + Goto.AstLabel.Name);
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeStmGotoIfFalse Goto)
		{
			Output.Write("if (!(");
			Generate(Goto.Condition);
			Output.Write(")) ");
			Output.Write("goto ");
			Output.Write("Label_" + Goto.AstLabel.Name);
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeStmGotoAlways Goto)
		{
			Output.Write("goto ");
			Output.Write("Label_" + Goto.AstLabel.Name);
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeStmLabel Label)
		{
			Output.Write("Label_" + Label.AstLabel.Name);
			Output.Write(":;");
		}

		protected virtual void _Generate(AstNodeStmExpr Stat)
		{
			Generate(Stat.AstNodeExpr);
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeExprTerop Item)
		{
			Output.Write("(");
			this.Generate(Item.Cond);
			Output.Write(" ? ");
			this.Generate(Item.True);
			Output.Write(" : ");
			this.Generate(Item.False);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeStmEmpty Empty)
		{
		}

		protected virtual void _Generate(AstNodeExprUnop Item)
		{
			Output.Write("(" + Item.Operator);
			this.Generate(Item.RightNode);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeStmIfElse IfElse)
		{
			Output.Write("if (");
			this.Generate(IfElse.Condition);
			Output.Write(") ");
			this.Generate(IfElse.True);
			if (IfElse.False != null)
			{
				Output.Write(" else ");
				this.Generate(IfElse.False);
			}
		}

		protected virtual void _Generate(AstNodeStmReturn Return)
		{
			Output.Write("return");
			if (Return.Expression != null)
			{
				Output.Write(" ");
				this.Generate(Return.Expression);
			}
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeStmAssign Assign)
		{
			Generate(Assign.LValue);
			Output.Write(" = ");
			Generate(Assign.Value);
			Output.Write(";");
		}

		protected virtual void _Generate(AstNodeExprIndirect Assign)
		{
			Output.Write("*(");
			Generate(Assign.PointerExpression);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeExprGetAddress GetAddress)
		{
			Output.Write("&");
			Generate(GetAddress.Expression);
		}

		protected virtual void _Generate(AstNodeExprFieldAccess FieldAccess)
		{
			Generate(FieldAccess.Instance);
			Output.Write(".");
			Output.Write(FieldAccess.Field.Name);
		}

		protected virtual void _Generate(AstNodeExprArgument Argument)
		{
			Output.Write(Argument.AstArgument.Name);
		}

		protected virtual void _Generate(AstNodeExprCast Cast)
		{
			Output.Write("(");
			Output.Write("(" + Cast.CastedType.Name + ")");
			Generate(Cast.Expr);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeExprCallTail Tail)
		{
			Output.Write("__tail_call(");
			Generate(Tail.Call);
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeExprCallStatic Call)
		{
			Output.Write(Call.MethodInfo.DeclaringType.Name + "." + Call.MethodInfo.Name);
			Output.Write("(");
			for (int n = 0; n < Call.Parameters.Length; n++)
			{
				if (n != 0) Output.Write(", ");
				Generate(Call.Parameters[n]);
			}
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeExprCallInstance Call)
		{
			Generate(Call.Instance);
			Output.Write("." + Call.MethodInfo.Name);
			Output.Write("(");
			for (int n = 0; n < Call.Parameters.Length; n++)
			{
				if (n != 0) Output.Write(", ");
				Generate(Call.Parameters[n]);
			}
			Output.Write(")");
		}

		protected virtual void _Generate(AstNodeStmContainer Container)
		{
			Output.Write("{\n");
			Output.Indent(() =>
			{
				foreach (var Node in Container.Nodes)
				{
					Generate(Node);
					Output.Write("\n");
				}
			});
			Output.Write("}\n");
		}

		public override string ToString()
		{
			return Output.ToString();
		}
	}
}
