using Codegen.Ast.Nodes;
using Codegen.Ast.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Generators
{
	public class GeneratorIL : Generator
	{
		protected ILGenerator ILGenerator;

		public GeneratorIL(ILGenerator ILGenerator)
		{
			this.ILGenerator = ILGenerator;
		}

		protected void Generate(AstNodeExprImm Item)
		{
			if (Item.Type == typeof(int))
			{
				int IntValue = (int)Item.Value;
				switch (IntValue)
				{
					case -1: ILGenerator.Emit(OpCodes.Ldc_I4_M1); break;
					case 0: ILGenerator.Emit(OpCodes.Ldc_I4_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldc_I4_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldc_I4_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldc_I4_3); break;
					case 4: ILGenerator.Emit(OpCodes.Ldc_I4_4); break;
					case 5: ILGenerator.Emit(OpCodes.Ldc_I4_5); break;
					case 6: ILGenerator.Emit(OpCodes.Ldc_I4_6); break;
					case 7: ILGenerator.Emit(OpCodes.Ldc_I4_7); break;
					case 8: ILGenerator.Emit(OpCodes.Ldc_I4_8); break;
					default: ILGenerator.Emit(OpCodes.Ldc_I4, IntValue); break;
				}
			}
			else
			{
				throw(new NotImplementedException(String.Format("Can't handle immediate type {0}", Item.Type)));
			}
		}

		protected void Generate(AstNodeExprBinop Item)
		{
			//Item.GetType().GenericTypeArguments[0]
			this.Generate(Item.LeftNode);
			this.Generate(Item.RightNode);
			switch (Item.Operator)
			{
				case "+":
					ILGenerator.Emit(OpCodes.Add);
					break;
				default:
					throw(new NotImplementedException(String.Format("Not implemented operator '{0}'", Item.Operator)));
			}
		}
	}
}
