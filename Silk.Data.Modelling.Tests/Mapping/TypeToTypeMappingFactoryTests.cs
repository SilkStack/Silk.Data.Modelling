using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Mapping
{
	[TestClass]
	public class TypeToTypeMappingFactoryTests
	{
		[TestMethod]
		public void MapTypes()
		{
			var analyzer = new TypeToTypeIntersectionAnalyzer();
			var intersection = analyzer.CreateIntersection(
				TypeModel.GetModelOf<SourceSuperModel>(),
				TypeModel.GetModelOf<TargetSuperModel>()
				);
			var factory = new TypeToTypeMappingFactory();
			var mapping = factory.CreateMapping(intersection);

			var sourceGraph = new ObjectGraphReaderWriter<SourceSuperModel>(new SourceSuperModel());
			var targetGraph = new ObjectGraphReaderWriter<TargetSuperModel>(new TargetSuperModel());
			mapping.Map(sourceGraph, targetGraph);

			Assert.AreEqual(
				sourceGraph.Graph.SubModel.DeepModel.CallToString.ToString(),
				targetGraph.Graph.SubModel.DeepModel.CallToString
				);
			Assert.AreEqual(
				(int)sourceGraph.Graph.SubModel.DeepModel.CastEnum,
				targetGraph.Graph.SubModel.DeepModel.CastEnum
				);
			Assert.AreEqual(
				(int)sourceGraph.Graph.SubModel.DeepModel.CastNumeric,
				targetGraph.Graph.SubModel.DeepModel.CastNumeric
				);
			Assert.AreEqual(
				sourceGraph.Graph.SubModel.DeepModel.Copy,
				targetGraph.Graph.SubModel.DeepModel.Copy
				);
			Assert.IsNotNull(
				targetGraph.Graph.SubModel.DeepModel.ExplicitCastSource
				);
			Assert.IsNotNull(
				targetGraph.Graph.SubModel.DeepModel.ExplicitCastTarget
				);
			Assert.AreEqual(
				sourceGraph.Graph.SubModel.DeepModel.TryParseEnum,
				targetGraph.Graph.SubModel.DeepModel.TryParseEnum.ToString()
				);
			Assert.AreEqual(
				sourceGraph.Graph.SubModel.DeepModel.TryParseInt,
				targetGraph.Graph.SubModel.DeepModel.TryParseInt.ToString()
				);
		}

		[TestMethod]
		public void MapEnumerables()
		{
			var analyzer = new TypeToTypeIntersectionAnalyzer();
			var intersection = analyzer.CreateIntersection(
				TypeModel.GetModelOf<SourceEnumerable>(),
				TypeModel.GetModelOf<TargetEnumerable>()
				);
			var factory = new TypeToTypeMappingFactory();
			var mapping = factory.CreateMapping(intersection);

			var sourceGraph = new ObjectGraphReaderWriter<SourceEnumerable>(new SourceEnumerable());
			var targetGraph = new ObjectGraphReaderWriter<TargetEnumerable>(new TargetEnumerable());
			mapping.Map(sourceGraph, targetGraph);

			Assert.IsTrue(sourceGraph.Graph.SourceArray.SequenceEqual(
				targetGraph.Graph.SourceArray
				));
			Assert.IsTrue(sourceGraph.Graph.SourceList.SequenceEqual(
				targetGraph.Graph.SourceList
				));
		}

		[TestMethod]
		public void MapNestedEnumerables()
		{
			var analyzer = new TypeToTypeIntersectionAnalyzer();
			var intersection = analyzer.CreateIntersection(
				TypeModel.GetModelOf<SourceEnumerableSuperType>(),
				TypeModel.GetModelOf<TargetEnumerableSuperType>()
				);
			var factory = new TypeToTypeMappingFactory();
			var mapping = factory.CreateMapping(intersection);

			var sourceGraph = new ObjectGraphReaderWriter<SourceEnumerableSuperType>(new SourceEnumerableSuperType());
			var targetGraph = new ObjectGraphReaderWriter<TargetEnumerableSuperType>(new TargetEnumerableSuperType());
			mapping.Map(sourceGraph, targetGraph);

			Assert.IsTrue(sourceGraph.Graph.Sub.Select(q => q.Data).SequenceEqual(
				targetGraph.Graph.Sub.Select(q => q.Data)
				));
		}

		private class SourceEnumerable
		{
			public int[] SourceArray => new[] { 1, 2, 3, 4, 5 };
			public List<int> SourceList => new List<int>(SourceArray);
		}

		private class TargetEnumerable
		{
			public List<int> SourceArray { get; set; }
			public int[] SourceList { get; set; }
		}

		private class SourceSuperModel
		{
			public SourceSubModel SubModel { get; } = new SourceSubModel();
		}

		private class SourceSubModel
		{
			public SourceDeepModel DeepModel { get; } = new SourceDeepModel();
		}

		private class SourceDeepModel
		{
			public string Copy => "Hello World";
			public int CallToString => 5;
			public float CastNumeric => 3.0f;
			public CastableSourceType ExplicitCastSource => new CastableSourceType();
			public CastableTargetType ExplicitCastTarget => new CastableTargetType();
			public string TryParseInt => "10";
			public string TryParseEnum => "SomeValue";
			public EnumType CastEnum => EnumType.SomeNotValue;
		}

		private class TargetSuperModel
		{
			public TargetSubModel SubModel { get; set; }
		}

		private class TargetSubModel
		{
			public TargetDeepModel DeepModel { get; set; }
		}

		private class TargetDeepModel
		{
			public string Copy { get; set; }
			public string CallToString { get; set; }
			public int CastNumeric { get; set; }
			public CastableTargetType ExplicitCastSource { get; set; }
			public CastableSourceType ExplicitCastTarget { get; set; }
			public int TryParseInt { get; set; }
			public EnumType TryParseEnum { get; set; }
			public int CastEnum { get; set; }
		}

		private class CastableSourceType
		{
			public static explicit operator CastableTargetType(CastableSourceType a)
			{
				return new CastableTargetType();
			}

			public static explicit operator CastableSourceType(CastableTargetType a)
			{
				return new CastableSourceType();
			}
		}

		private class CastableTargetType
		{
		}

		private enum EnumType
		{
			SomeValue = 100,
			SomeNotValue = 200
		}

		private class SourceEnumerableSuperType
		{
			public SourceEnumerableSubType[] Sub { get; } = new[] {
				new SourceEnumerableSubType(),
				new SourceEnumerableSubType(),
				new SourceEnumerableSubType()
			};
		}

		private class SourceEnumerableSubType
		{
			private static int _counter;

			public int Data { get; } = _counter++;
		}

		private class TargetEnumerableSuperType
		{
			public TargetEnumerableSubType[] Sub { get; set; }
		}

		private class TargetEnumerableSubType
		{
			public int Data { get; set; }
		}
	}
}
