using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
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
			Assert.IsTrue(mapping.Bindings.Any(q => q.FromPath.SequenceEqual(new[] { "Integer" }) &&
				q.ToPath.SequenceEqual(new[] { "Integer" }) &&
				q is CopyBinding<int>
				));
			Assert.IsTrue(mapping.Bindings.Any(q => q.FromPath.SequenceEqual(new[] { "String" }) &&
				q.ToPath.SequenceEqual(new[] { "String" }) &&
				q is CopyBinding<string>
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

			var convention = new CopySameTypes();
			var builder = new MappingBuilder(fromPocoModel, toPocoModel);
			builder.AddConvention(CopySameTypes.Instance);
			return builder.BuildMapping();
		}

		private class SimplePoccWithReadWriteProperties
		{
			public int Integer { get; set; }
			public string String { get; set; }
		}
	}
}
