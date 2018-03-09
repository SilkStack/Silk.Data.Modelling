using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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

		[TestMethod]
		public void MapFlattenedProperties()
		{
			var mapper = new ObjectMapper();
			var input = new SourcePoco
			{
				Sub = new SourceSubPoco
				{
					Property = 1
				}
			};
			var output = mapper.Map<FlattenedPoco>(input);
			Assert.IsNotNull(output);
			Assert.AreEqual(input.Sub.Property, output.SubProperty);
		}

		[TestMethod]
		public void MapInflatedProperties()
		{
			var mapper = new ObjectMapper();
			var input = new FlattenedPoco { SubProperty = 1 };
			var output = mapper.Map<SourcePoco>(input);
			Assert.IsNotNull(output);
			Assert.IsNotNull(output.Sub);
			Assert.AreEqual(input.SubProperty, output.Sub.Property);
		}

		[TestMethod]
		public void MapRecursiveObjects()
		{
			var mapper = new ObjectMapper();
			var input = new RecursiveSource
			{
				Property = 1,
				Sub = new RecursiveSource
				{
					Property = 2,
					Sub = new RecursiveSource
					{
						Property = 3
					}
				}
			};
			var output = mapper.Map<RecursiveTarget>(input);
			Assert.IsNotNull(output);
			Assert.IsNotNull(output.Sub);
			Assert.IsNotNull(output.Sub.Sub);
			Assert.IsNull(output.Sub.Sub.Sub);
			Assert.AreEqual(1, output.Property);
			Assert.AreEqual(2, output.Sub.Property);
			Assert.AreEqual(3, output.Sub.Sub.Property);
		}

		[TestMethod]
		public void MapArrayToCollection()
		{
			var mapper = new ObjectMapper();
			var input = new PocoWithIntArray
			{
				Source = new[] { 1, 2, 3, 4, 5 }
			};
			var output = mapper.Map<PocoWithIntCollection>(input);
			Assert.IsNotNull(output.Source);
			Assert.AreEqual(input.Source.Length, output.Source.Count);
			Assert.IsTrue(input.Source.SequenceEqual(output.Source));
		}

		[TestMethod]
		public void MapCollectionToArray()
		{
			var mapper = new ObjectMapper();
			var input = new PocoWithIntCollection
			{
				Source = new List<int> { 1, 2, 3, 4, 5 }
			};
			var output = mapper.Map<PocoWithIntArray>(input);
			Assert.IsNotNull(output.Source);
			Assert.AreEqual(input.Source.Count, output.Source.Length);
			Assert.IsTrue(input.Source.SequenceEqual(output.Source));
		}

		[TestMethod]
		public void MapSubMappingEnumerable()
		{
			var mapper = new ObjectMapper();
			var input = new PocoWithSubBindingArray
			{
				Sources = new SourceSubPoco[]
				{
					new SourceSubPoco { Property = 1 },
					new SourceSubPoco { Property = 2 },
					new SourceSubPoco { Property = 3 }
				}
			};
			var output = mapper.Map<PocoWithSubBindingCollection>(input);
			Assert.IsNotNull(output);
			Assert.IsNotNull(output.Sources);
			Assert.AreEqual(input.Sources.Length, output.Sources.Count);
			Assert.IsTrue(input.Sources.Select(q => q.Property).SequenceEqual(output.Sources.Select(q => q.Property)));
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

		private class FlattenedPoco
		{
			public int SubProperty { get; set; }
		}

		private class RecursiveSource
		{
			public int Property { get; set; }
			public RecursiveSource Sub { get; set; }
		}

		private class RecursiveTarget
		{
			public int Property { get; set; }
			public RecursiveTarget Sub { get; set; }
		}

		private class PocoWithIntArray
		{
			public int[] Source { get; set; }
		}

		private class PocoWithIntCollection
		{
			public ICollection<int> Source { get; set; }
		}

		private class PocoWithSubBindingArray
		{
			public SourceSubPoco[] Sources { get; set; }
		}

		private class PocoWithSubBindingCollection
		{
			public ICollection<TargetSubPoco> Sources { get; set; }
		}
	}
}
