using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class CastNumericTypesTests
	{
		[TestMethod]
		public void MapCastableProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			Assert.AreEqual(2, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q =>
				q is CastExpressionBinding<int, float>
				));
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q =>
				q is CastExpressionBinding<SourceEnum, int>
				));
		}

		[TestMethod]
		public void MappingCastsProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			var source = new SourcePoco
			{
				Castable = 1,
				Enum = SourceEnum.Option2
			};
			var result = new TargetPoco();
			var sourceReader = new ObjectReadWriter(source, TypeModel.GetModelOf<SourcePoco>(), typeof(SourcePoco));
			var targetWriter = new ObjectReadWriter(result, TypeModel.GetModelOf<TargetPoco>(), typeof(TargetPoco));
			mapping.PerformMapping(sourceReader, targetWriter);
			Assert.AreEqual(source.Castable, result.Castable);
			Assert.AreEqual((int)source.Enum, result.Enum);
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
			options.Conventions.Add(CastNumericTypes.Instance);
			var builder = new MappingBuilder(fromPocoModel, toPocoModel, options);
			return builder.BuildMapping();
		}

		private class SourcePoco
		{
			public int Castable { get; set; }
			public SourceEnum Enum { get; set; }
		}

		private class TargetPoco
		{
			public float Castable { get; set; }
			public int Enum { get; set; }
		}

		private enum SourceEnum
		{
			Option1 = 5,
			Option2 = 10
		}
	}
}
