using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis.CandidateSources;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Analysis.CandidateSources
{
	[TestClass]
	public class FlattenedNameMatchCandidateSourceTests
	{
		private readonly static TypeModel<LeftModel> LeftTypeModel = TypeModel.GetModelOf<LeftModel>();
		private readonly static TypeModel<RightModel> RightTypeModel = TypeModel.GetModelOf<RightModel>();

		[TestMethod]
		public void GetIntersectCandidates_Includes_Candidates_With_Same_Flattened_Path()
		{
			var candidateSource = new FlattenedNameMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			var testCandidate = candidates.FirstOrDefault(candidate =>
				string.Join(".", candidate.LeftPath.Fields.Select(q => q.FieldName)) == "SubProperty" &&
				string.Join(".", candidate.RightPath.Fields.Select(q => q.FieldName)) == "Sub.Property"
			);
			Assert.IsNotNull(testCandidate, "No intersect candidate with identical flat paths found.");
		}

		[TestMethod]
		public void GetIntersectCandidates_Ignores_Candidates_With_Mismatched_Flattened_Path()
		{
			var candidateSource = new FlattenedNameMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			var testCandidate = candidates.FirstOrDefault(candidate =>
				string.Join(".", candidate.LeftPath.Fields.Select(q => q.FieldName)) == "SubProperty" &&
				string.Join(".", candidate.RightPath.Fields.Select(q => q.FieldName)) == "Sub.Mismatched"
			);
			Assert.IsNull(testCandidate, "Intersect candidate with mismatched flat paths found.");
		}

		[TestMethod]
		public void GetIntersectCandidates_Stops_At_MaxDepth()
		{
			var candidateSource = new FlattenedNameMatchCandidateSource<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>
			{
				MaxDepth = 1
			};
			var candidates = candidateSource.GetIntersectCandidates(LeftTypeModel, RightTypeModel);
			var testCandidate = candidates.FirstOrDefault(candidate =>
				string.Join(".", candidate.LeftPath.Fields.Select(q => q.FieldName)) == "SubProperty" &&
				string.Join(".", candidate.RightPath.Fields.Select(q => q.FieldName)) == "Sub.Property"
			);
			Assert.IsNull(testCandidate, "Intersect candidate should be null");
		}

		private class LeftModel
		{
			public string SubProperty { get; set; }
		}

		private class RightModel
		{
			public SubRightModel Sub { get; set; }
		}

		private class SubRightModel
		{
			public string Property { get; set; }
			public string Mismatched { get; set; }
		}
	}
}
