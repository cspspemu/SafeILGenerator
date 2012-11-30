using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafeILGenerator.Utils;

namespace SafeILGenerator.Tests.Util
{
	[TestClass]
	public class ILFieldInfoTest
	{
		public int Test;

		[TestMethod]
		public void TestGetFieldInfo()
		{
			Assert.AreEqual(
				typeof(ILFieldInfoTest).GetField("Test"),
				ILFieldInfo.GetFieldInfo(() => Test)
			);
		}
	}
}
