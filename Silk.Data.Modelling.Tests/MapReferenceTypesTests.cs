using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class MapReferenceTypesTests
	{
		[TestMethod]
		public void MapReferenceTypesBindsReadWritePropertiesWithIdenticalNames()
		{
			var mapping = CreateMapping<SourceParentPoco, TargetParentPoco>();
			Assert.AreEqual(1, mapping.Bindings.Length);

			var binding = (MappingBinding)mapping.Bindings[0];
			Assert.IsInstanceOfType(binding, typeof(SubmappingBinding<SourceSubType, TargetSubType>));

			var submappingBinding = (SubmappingBinding<SourceSubType, TargetSubType>)binding;
			Assert.IsNotNull(submappingBinding.Mapping);

			Assert.AreEqual(1, submappingBinding.Mapping.Bindings.Length);

			binding = (MappingBinding)submappingBinding.Mapping.Bindings[0];
		}

		[TestMethod]
		public void CreatingBindingsOnRecursiveTypesDoesntStackOverflow()
		{
			CreateMapping<RecursiveSourceType, RecursiveTargetType>();
		}

		private Mapping.Mapping CreateMapping<T>()
		{
			return CreateMapping<T, T>();
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>()
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var builder = new MappingBuilder(fromPocoModel, toPocoModel);
			builder.AddConvention(CopySameTypes.Instance);
			builder.AddConvention(MapReferenceTypes.Instance);
			return builder.BuildMapping();
		}

		private class SourceParentPoco
		{
			public SourceSubType Member { get; set; }
		}

		private class SourceSubType
		{
			public int OneToOneMapping { get; set; }
		}

		private class TargetParentPoco
		{
			public TargetSubType Member { get; set; }
		}

		private class TargetSubType
		{
			public int OneToOneMapping { get; set; }
		}

		private class RecursiveSourceType
		{
			public RecursiveSourceType Member { get; set; }
		}

		private class RecursiveTargetType
		{
			public RecursiveTargetType Member { get; set; }
		}
	}
}
