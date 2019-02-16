using Silk.Data.Modelling.Analysis.CandidateSources;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for matched data types.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class SameDataTypeRule<TLeftModel, TLeftField, TRightModel, TRightField> :
		IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public override bool IsValidIntersection(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate,
			out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields
			)
		{
			if (intersectCandidate.LeftField.RemoveEnumerableType() != intersectCandidate.RightField.RemoveEnumerableType() ||
				intersectCandidate.LeftField.IsEnumerableType != intersectCandidate.RightField.IsEnumerableType)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = BuildIntersectedFields(intersectCandidate);
			return true;
		}

		protected override TryConvertDelegate<TFrom, TTo> TryConvertFactory<TFrom, TTo>()
		{
			var fromParameter = Expression.Parameter(typeof(TFrom));
			var toParameter = Expression.Parameter(typeof(TTo).MakeByRefType());

			var lambda = Expression.Lambda<TryConvertDelegate<TFrom, TTo>>(
				Expression.Block(
					Expression.Assign(toParameter, fromParameter),
					Expression.Constant(true)
					), fromParameter, toParameter
				);
			return lambda.Compile();
		}
	}
}
