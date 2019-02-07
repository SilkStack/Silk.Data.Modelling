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
				TypeModel.GetModelOf<SourceModel>(),
				TypeModel.GetModelOf<TargetModel>()
				);
			var factory = new TypeToTypeMappingFactory();
			var mapping = factory.CreateMapping(intersection);
		}

		private class SourceModel
		{
			public string PropertyA => "Hello World";
		}

		private class TargetModel
		{
			public string PropertyA { get; set; }
		}
	}
}
