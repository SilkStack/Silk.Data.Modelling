using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Analysis
{
	[TestClass]
	public class TypeToTypeIntersectionAnalyzerTests
	{
		[TestMethod]
		public void CreateIntersection_Finds_SameName_And_DataType()
		{
			var analyzer = new TypeToTypeIntersectionAnalyzer(
				new[] { new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>() },
				new[] { new SameDataTypeRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>() }
				);
			var leftModel = TypeModel.GetModelOf<Model>();
			var rightModel = TypeModel.GetModelOf<Model>();
			var intersection = analyzer.CreateIntersection(leftModel, rightModel);

			var testFieldIntersect = intersection.IntersectedFields.FirstOrDefault(
				intersect => intersect.LeftField.FieldName == nameof(Model.Property) && intersect.RightField.FieldName == nameof(Model.Property)
				);
			Assert.IsNotNull(testFieldIntersect, "Property intersection not found on intersection.");
		}

		private class Model
		{
			public string Property { get; set; }
		}
	}
}
