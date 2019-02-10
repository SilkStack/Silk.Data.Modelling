using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.Rules;
using Silk.Data.Modelling.Analysis.CandidateSources;
using System.Linq;
using System;

namespace Silk.Data.Modelling.Tests.Analysis.Rules
{
	[TestClass]
	public class SameDataTypeRuleTests
	{
		private readonly static TypeModel<int?> NullableTypeModel = TypeModel.GetModelOf<int?>();

		[TestMethod]
		public void IsValidIntersection_Returns_True_For_Same_DataTypes()
		{
			var rule = new SameDataTypeRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				new FieldPath<TypeModel, PropertyInfoField>(
					NullableTypeModel,
					NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)),
					new[] { NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					NullableTypeModel,
					NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)),
					new[] { NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsTrue(result, "Rule returned an invalid result.");
			Assert.IsNotNull(intersectedFields, "Rule returned an invalid intersected field.");
		}

		[TestMethod]
		public void IsValidIntersection_Returns_False_For_Mismatched_DataTypes()
		{
			var rule = new SameDataTypeRule<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidate = new IntersectCandidate<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				new FieldPath<TypeModel, PropertyInfoField>(
					NullableTypeModel,
					NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)),
					new[] { NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.HasValue)) }
					),
				new FieldPath<TypeModel, PropertyInfoField>(
					NullableTypeModel,
					NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.Value)),
					new[] { NullableTypeModel.Fields.First(q => q.FieldName == nameof(Nullable<int>.Value)) }
					),
				null
				);

			var result = rule.IsValidIntersection(candidate, out var intersectedFields);

			Assert.IsFalse(result, "Rule returned an invalid result.");
			Assert.IsNull(intersectedFields, "Rule returned an intersected field.");
		}
	}
}
