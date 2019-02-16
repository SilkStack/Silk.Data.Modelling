using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.Analysis.Rules;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Analysis.Rules
{
	[TestClass]
	public class ExplicitCastRuleTests
	{
		private static readonly TypeModel<LeftModel> LeftTypeModel = TypeModel.GetModelOf<LeftModel>();
		private static readonly TypeModel<RightModel> RightTypeModel = TypeModel.GetModelOf<RightModel>();

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_Castable_From_Source_DataTypes()
		{
			var rule = new ExplicitCastRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField, CastableSourceType, CastableTargetType>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.CastFromSource)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.CastFromSource)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.CastFromSource)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.CastFromSource)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsTrue(result, "Rule returned an invalid result.");
			Assert.IsNotNull(intersectedFields, "Rule returned an invalid intersected field.");
		}

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_Castable_From_Target_DataTypes()
		{
			var rule = new ExplicitCastRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField, CastableTargetType, CastableSourceType>(
				new FieldPath<TypeModel, PropertyInfoField>(
					LeftTypeModel,
					LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.CastFromTarget)),
					new[] { LeftTypeModel.Fields.First(q => q.FieldName == nameof(LeftModel.CastFromTarget)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					RightTypeModel,
					RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.CastFromTarget)),
					new[] { RightTypeModel.Fields.First(q => q.FieldName == nameof(RightModel.CastFromTarget)) }
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
			var rule = new ExplicitCastRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
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
			public CastableSourceType CastFromSource { get; }
			public CastableTargetType CastFromTarget { get; }
			public object NonConvertable { get; }
		}

		private class RightModel
		{
			public CastableTargetType CastFromSource { get; }
			public CastableSourceType CastFromTarget { get; }
			public int NonConvertable { get; }
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
	}
}
