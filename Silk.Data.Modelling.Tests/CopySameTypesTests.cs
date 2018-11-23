using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class CopySameTypesTests
	{
		[TestMethod]
		public void CopySameTypesBindsReadWritePropertiesWithIdenticalNames()
		{
			var mapping = CreateMapping<SimplePoccWithReadWriteProperties>();
			Assert.AreEqual(2, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is CopyBinding<int>
				));
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is CopyBinding<string>
				));
		}

		[TestMethod]
		public void DontCopyDifferentTypesWithIdenticalNames()
		{
			var mapping = CreateMapping<SimplePoccWithReadWriteProperties, SimplePoccWithDifferentReadWriteProperties>();
			Assert.AreEqual(0, mapping.Bindings.Length);
		}

		[TestMethod]
		public void CopyEnumerablesOfSameTypesWithIdenticalNames()
		{
			var mapping = CreateMapping<SourcePocoWithEnumerables, TargetPocoWithEnumerables>();
			Assert.AreEqual(3, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is EnumerableBinding<List<int>, List<int>, int, int>
				));
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is EnumerableBinding<int[], ICollection<int>, int, int>
				));
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is EnumerableBinding<IEnumerable<int>, int[], int, int>
				));
		}

		private Mapping.Mapping CreateMapping<T>()
		{
			return CreateMapping<T, T>();
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>()
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var options = new MappingOptions();
			options.Conventions.Add(CopySameTypes.Instance);
			var builder = new MappingBuilder(fromPocoModel, toPocoModel, options);
			return builder.BuildMapping();
		}

		private class SimplePoccWithReadWriteProperties
		{
			public int Integer { get; set; }
			public string String { get; set; }
		}

		private class SimplePoccWithDifferentReadWriteProperties
		{
			public long Integer { get; set; }
			public char[] String { get; set; }
		}

		private class SourcePocoWithEnumerables
		{
			public List<int> ListOfInt { get; set; }
			public int[] CollectionOfInt { get; set; }
			public IEnumerable<int> Ints { get; set; }
		}

		private class TargetPocoWithEnumerables
		{
			public List<int> ListOfInt { get; set; }
			public ICollection<int> CollectionOfInt { get; set; }
			public int[] Ints { get; set; }
		}
	}
}
