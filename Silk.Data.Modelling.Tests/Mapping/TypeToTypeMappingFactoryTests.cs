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
				sourceGraph.Graph.SubModel.DeepModel.PropertyA,
				targetGraph.Graph.SubModel.DeepModel.PropertyA
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
			public string PropertyA => "Hello World";
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
			public string PropertyA { get; set; }
		}
	}
}
