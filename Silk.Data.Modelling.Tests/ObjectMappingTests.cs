using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ObjectMappingTests
	{
		[TestMethod]
		public void InjectSameTypes()
		{
			var mapper = new ObjectMapper();
			var output = new SimplePoco();
			var outputReference = output;
			var input = new SimplePoco
			{
				Property = 1
			};
			mapper.Inject(input, output);
			Assert.ReferenceEquals(output, outputReference);
			Assert.AreEqual(input.Property, output.Property);
		}

		[TestMethod]
		public void MappingCreatesOutputObject()
		{
			var mapper = new ObjectMapper();
			var input = new SimplePoco
			{
				Property = 1
			};
			var output = mapper.Map<SimplePoco>(input);
			Assert.IsNotNull(output);
			Assert.AreEqual(input.Property, output.Property);
		}

		[TestMethod]
		public void InjectIntoExistingSubType()
		{
			var mapper = new ObjectMapper();
			var outputSub = new TargetSubPoco();
			var outputSubReference = outputSub;
			var output = new TargetPoco { Sub = outputSub };
			var input = new SourcePoco { Sub = new SourceSubPoco { Property = 1 } };
			mapper.Inject(input, output);
			Assert.ReferenceEquals(outputSub, output.Sub);
			Assert.ReferenceEquals(outputSub, outputSubReference);
			Assert.AreEqual(outputSub.Property, input.Sub.Property);
		}

		[TestMethod]
		public void SubMappingCreatesSubObjects()
		{
			var mapper = new ObjectMapper();
			var input = new SourcePoco
			{
				Sub = new SourceSubPoco
				{
					Property = 1
				}
			};
			var output = mapper.Map<TargetPoco>(input);
			Assert.IsNotNull(output);
			Assert.IsNotNull(output.Sub);
			Assert.AreEqual(input.Sub.Property, output.Sub.Property);
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
