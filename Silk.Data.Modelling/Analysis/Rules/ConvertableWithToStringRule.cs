using Silk.Data.Modelling.Analysis.CandidateSources;
using System;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule that creates candidates when types can be converted by calling ToString().
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class ConvertableWithToStringRule<TLeftModel, TLeftField, TRightModel, TRightField> :
		IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public override bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			if (intersectCandidate.LeftField.FieldDataType == intersectCandidate.RightField.FieldDataType ||
				intersectCandidate.RightField.RemoveEnumerableType() != typeof(string) ||
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
					Expression.Assign(toParameter, Expression.Call(
						fromParameter, typeof(TFrom).GetMethod(nameof(object.ToString), new Type[0])
						)),
					Expression.Constant(true)
					), fromParameter, toParameter
				);
			return lambda.Compile();
		}
	}
}
