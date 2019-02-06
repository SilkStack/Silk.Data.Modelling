using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.CandidateSources;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ExactPathMatchCandidateSourceTests
	{
		private readonly static TypeModel<LeftModel> LeftTypeModel = TypeModel.GetModelOf<LeftModel>();
		private readonly static TypeModel<RightModel> RightTypeModel = TypeModel.GetModelOf<RightModel>();

		[TestMethod]
		public void GetIntersectCandidates_Includes_Candidates_With_Same_Names()
		{
			var candidateSource = new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			var testCandidate = candidates.FirstOrDefault(candidate => candidate.LeftField.FieldName == candidate.RightField.FieldName);
			Assert.IsNotNull(testCandidate, "No intersect candidate with identical names found.");
		}

		[TestMethod]
		public void GetIntersectCandidates_Includes_Nested_Candidates_With_Identical_Paths()
		{
			var candidateSource = new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			//  should find `Length` or similar fields on `string` from `SomeNameProperty`.
			var testCandidate = candidates.FirstOrDefault(candidate =>
				candidate.LeftPath.Fields.Count > 1 &&
				candidate.LeftPath.Fields.Select(q => q.FieldName).SequenceEqual(
					candidate.RightPath.Fields.Select(q => q.FieldName)
					)
				);
			Assert.IsNotNull(testCandidate, "No intersect candidate with identical paths found.");
		}

		[TestMethod]
		public void GetIntersectCandidates_Excludes_Candidates_With_Different_Names()
		{
			var candidateSource = new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			var testCandidate = candidates.FirstOrDefault(candidate => candidate.LeftField.FieldName != candidate.RightField.FieldName);
			Assert.IsNull(testCandidate, "Intersect candidate with mismatched names found.");
		}

		[TestMethod]
		public void GetIntersectCandidates_Stops_At_MaxDepth()
		{
			var candidateSource = new ExactPathMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
			{
				MaxDepth = 1
			};
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, LeftTypeModel);
			Assert.AreEqual(2, candidates.Count(), "Too many candidates found.");
		}

		private class LeftModel
		{
			public string SameNameProperty { get; set; }

			public string LeftDifferentNameProperty { get; set; }
		}

		private class RightModel
		{
			public string SameNameProperty { get; set; }

			public string RightDifferentNameProperty { get; set; }
		}
	}
}
