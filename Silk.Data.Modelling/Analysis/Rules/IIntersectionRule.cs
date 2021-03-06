﻿using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for validating intersect candidates and producing IntersectFields instances.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public interface IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Determine if an intersection candidate matches the rule and provide an IntersectedField if it does.
		/// </summary>
		/// <param name="intersectCandidate"></param>
		/// <param name="intersectedFields"></param>
		/// <returns></returns>
		bool IsValidIntersection(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate,
			out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields
			);
	}
}
