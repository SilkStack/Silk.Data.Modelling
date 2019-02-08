﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping;

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