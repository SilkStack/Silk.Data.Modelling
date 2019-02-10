using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Analysis.Rules
{
	[TestClass]
	public class ConvertableWithTryParseTests
	{
		private static readonly TypeModel<LeftModel> LeftTypeModel = TypeModel.GetModelOf<LeftModel>();
		private static readonly TypeModel<RightModel> RightTypeModel = TypeModel.GetModelOf<RightModel>();

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_String_To_Int_DataTypes()
		{
			var rule = new ConvertableWithTryParse<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.IntFromString)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.IntFromString)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.IntFromString)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.IntFromString)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsTrue(result, "Rule returned an invalid result.");
			Assert.IsNotNull(intersectedFields, "Rule returned an invalid intersected field.");
		}

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_String_To_Enum_DataTypes()
		{
			var rule = new ConvertableWithTryParse<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.EnumFromString)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.EnumFromString)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.EnumFromString)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.EnumFromString)) }
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
			var rule = new ConvertableWithTryParse<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
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
			public string IntFromString { get; }
			public string EnumFromString { get; }
			public string NonConvertable { get; }
		}

		private class RightModel
		{
			public int IntFromString { get; }
			public MyEnum EnumFromString { get; }
			public object NonConvertable { get; }
		}

		private enum MyEnum
		{
			SomeValue
		}
	}
}
