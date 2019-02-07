using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping;

namespace Silk.Data.Modelling.Tests
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
		}

		private class SourceSuperModel
		{
			public SourceSubModel SubModel { get; } = new SourceSubModel();
		}

		private class SourceSubModel
		{
			public string PropertyA => "Hello World";
		}

		private class TargetSuperModel
		{
			public TargetSubModel SubModel { get; set; }
		}

		private class TargetSubModel
		{
			public string PropertyA { get; set; }
		}
	}
}
