using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ObjectMappingTests
	{
		[TestMethod]
		public void ThrowExceptionWithNoUsableCtor()
		{
			throw new System.NotImplementedException();
		}

		[TestMethod]
		public void InjectSameTypes()
		{
			var mapper = new ObjectMapper();
			var output = new SimplePoco();
			var input = new SimplePoco
			{
				Property = 1
			};
			mapper.Inject(input, output);
			Assert.AreEqual(input.Property, output.Property);
		}

		[TestMethod]
		public void InjectIntoExistingSubType()
		{
			var mapper = new ObjectMapper();
			var outputSub = new TargetSubPoco();
			var output = new TargetPoco { Sub = outputSub };
			var input = new SourcePoco { Sub = new SourceSubPoco { Property = 1 } };
			mapper.Inject(input, output);
			Assert.ReferenceEquals(outputSub, output.Sub);
			Assert.AreEqual(outputSub.Property, input.Sub.Property);
		}

		private class SimplePoco
		{
			public int Property { get; set; }
		}

		private class SourcePoco
		{
			public SourceSubPoco Sub { get; set; }
		}

		private class SourceSubPoco
		{
			public int Property { get; set; }
		}

		private class TargetPoco
		{
			public TargetSubPoco Sub { get; set; }
		}

		private class TargetSubPoco
		{
			public int Property { get; set; }
		}
	}
}
