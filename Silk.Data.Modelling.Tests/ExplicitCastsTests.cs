using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ExplicitCastsTests
	{
		[TestMethod]
		public void MapCastableProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is ExplicitCastBinding<SourceCastType, TargetCastType>
				));
		}

		[TestMethod]
		public void MappingCastsProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			var source = new SourcePoco
			{
				Castable = new SourceCastType { Value = 1 }
			};
			var result = new TargetPoco();
			var sourceReader = new ObjectReadWriter(source, TypeModel.GetModelOf<SourcePoco>(), typeof(SourcePoco));
			var targetWriter = new ObjectReadWriter(result, TypeModel.GetModelOf<TargetPoco>(), typeof(TargetPoco));
			mapping.PerformMapping(sourceReader, targetWriter);
			Assert.IsNotNull(result.Castable);
			Assert.AreEqual(source.Castable.Value, result.Castable.Value);
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
			options.Conventions.Add(CopyExplicitCast.Instance);
			var builder = new MappingBuilder(fromPocoModel, toPocoModel, options);
			return builder.BuildMapping();
		}

		private class SourcePoco
		{
			public SourceCastType Castable { get; set; }
		}

		private class TargetPoco
		{
			public TargetCastType Castable { get; set; }
		}

		private class SourceCastType
		{
			public int Value { get; set; }

			public static explicit operator TargetCastType(SourceCastType source)
			{
				if (source == null)
					return null;
				return new TargetCastType { Value = source.Value };
			}
		}

		private class TargetCastType
		{
			public int Value { get; set; }
		}
	}
}
