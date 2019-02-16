using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Analysis.Rules
{
	[TestClass]
	public class ConvertableWithToStringRuleTests
	{
		private static readonly TypeModel<LeftModel> LeftTypeModel = TypeModel.GetModelOf<LeftModel>();
		private static readonly TypeModel<RightModel> RightTypeModel = TypeModel.GetModelOf<RightModel>();

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_Convertable_DataTypes()
		{
			var rule = new ConvertableWithToStringRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField, float, string>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.Convertable)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.Convertable)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.Convertable)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.Convertable)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsTrue(result, "Rule returned an invalid result.");
			Assert.IsNotNull(intersectedFields, "Rule returned an invalid intersected field.");
		}

		[TestMethod]
		public void IsValidIntersection_Returns_False_For_Non_Convertable_DataTypes()
		{
			var rule = new ConvertableWithToStringRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField, object, int>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.NonConvertable)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.NonConvertable)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.NonConvertable)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.NonConvertable)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsFalse(result, "Rule returned an invalid result.");
			Assert.IsNull(intersectedFields, "Rule returned an intersected field.");
		}

		private class LeftModel
		{
			public float Convertable { get; }
			public object NonConvertable { get; }
		}

		private class RightModel
		{
			public string Convertable { get; }
			public int NonConvertable { get; }
		}
	}
}
