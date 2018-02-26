using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class UtilityTests
	{
		[TestMethod]
		public void GetPathsWorksAsExpected()
		{
			var pathString = "TopMiddleBottomLast";
			var expectedValues = new string[][]
			{
				"TopMiddleBottomLast".Split('.'),
				"TopMiddleBottom.Last".Split('.'),
				"TopMiddle.BottomLast".Split('.'),
				"TopMiddle.Bottom.Last".Split('.'),
				"Top.MiddleBottomLast".Split('.'),
				"Top.MiddleBottom.Last".Split('.'),
				"Top.Middle.BottomLast".Split('.'),
				"Top.Middle.Bottom.Last".Split('.')
			};
			var paths = ConventionUtilities.GetPaths(pathString).ToArray();
			Assert.AreEqual(expectedValues.Length, paths.Length);
			Assert.AreEqual(paths.Length, paths.GroupBy(q => string.Join('.', q)).Count());

			foreach (var expectedValue in expectedValues)
			{
				Assert.IsTrue(paths.Any(q => q.SequenceEqual(expectedValue)));
			}
		}
	}
}
