using Codegen.Ast.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Codegen.Ast.Generators
{
	public abstract class Generator
	{
		private Dictionary<Type, MethodInfo> GenerateMappings = new Dictionary<Type, MethodInfo>();

		public Generator()
		{
			foreach (
				var Method
				in
				this.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(Method => Method.ReturnType == typeof(void))
					.Where(Method => Method.GetParameters().Count() == 1)
			)
			{
				GenerateMappings[Method.GetParameters().First().ParameterType] = Method;
			}
		}

		/// <summary>
		/// Determine dinamically which method to call.
		/// </summary>
		/// <param name="AstNode"></param>
		public void Generate(AstNode AstNode)
		{
			//if (AstNode == null) return;

			var AstNodeType = AstNode.GetType();
			if (!GenerateMappings.ContainsKey(AstNodeType))
			{
				foreach (var GenerateMapping in GenerateMappings)
				{
					Console.WriteLine(GenerateMapping);
				}
				throw(new NotImplementedException(String.Format("Don't know how to generate {0}", AstNodeType)));
			}
			GenerateMappings[AstNodeType].Invoke(this, new object[] { AstNode });
		}
	}
}
