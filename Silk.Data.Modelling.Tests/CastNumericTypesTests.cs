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
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => q.FromPath.SequenceEqual(new[] { "Castable" }) &&
				q.ToPath.SequenceEqual(new[] { "Castable" }) &&
				q is CastExpressionBinding<int, float>
				));
		}

		[TestMethod]
		public void MappingCastsProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			var source = new SourcePoco
			{
				Castable = 1
			};
			var result = new TargetPoco();
			var sourceReader = new ObjectReadWriter(source, TypeModel.GetModelOf<SourcePoco>(), typeof(SourcePoco));
			var targetWriter = new ObjectReadWriter(result, TypeModel.GetModelOf<TargetPoco>(), typeof(TargetPoco));
			mapping.PerformMapping(sourceReader, targetWriter);
			Assert.AreEqual(source.Castable, result.Castable);
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
			builder.AddConvention(CastNumericTypes.Instance);
			return builder.BuildMapping();
		}

		private class SourcePoco
		{
			public int Castable { get; set; }
		}

		private class TargetPoco
		{
			public float Castable { get; set; }
		}
	}
}
