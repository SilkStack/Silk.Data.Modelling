using Silk.Data.Modelling.Analysis.CandidateSources;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for finding candidates with castable numeric types.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class BothNumericTypesRule<TLeftModel, TLeftField, TRightModel, TRightField> :
		IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		private static Type[] _numericTypes = new[]
		{
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(decimal),
			typeof(float),
			typeof(double)
		};

		private static bool IsNumericType(Type type)
		{
			return type.IsEnum || _numericTypes.Contains(type);
		}

		public override bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			if (intersectCandidate.LeftField.FieldDataType == intersectCandidate.RightField.FieldDataType ||
				!IsNumericType(intersectCandidate.LeftField.RemoveEnumerableType()) ||
				!IsNumericType(intersectCandidate.RightField.RemoveEnumerableType()) ||
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
					Expression.Assign(toParameter, Expression.Convert(fromParameter, typeof(TTo))),
					Expression.Constant(true)
					), fromParameter, toParameter
				);
			return lambda.Compile();
		}
	}
}
