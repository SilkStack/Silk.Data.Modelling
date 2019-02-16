using Silk.Data.Modelling.Analysis.CandidateSources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for types that can be coverted with a TryParse method.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class ConvertableWithTryParse<TLeftModel, TLeftField, TRightModel, TRightField> :
		IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public override bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			if (intersectCandidate.LeftField.FieldDataType == intersectCandidate.RightField.FieldDataType ||
				intersectCandidate.LeftField.RemoveEnumerableType() != typeof(string) ||
				intersectCandidate.LeftField.IsEnumerableType != intersectCandidate.RightField.IsEnumerableType)
			{
				intersectedFields = null;
				return false;
			}

			var tryParseMethod = GetTryParseMethod(
				intersectCandidate.LeftField.RemoveEnumerableType(),
				intersectCandidate.RightField.RemoveEnumerableType()
				);
			if (tryParseMethod == null)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = BuildIntersectedFields(intersectCandidate);
			return true;
		}

		protected override TryConvertDelegate<TFrom, TTo> TryConvertFactory<TFrom, TTo>()
		{
			var tryParseMethod = GetTryParseMethod(typeof(TFrom), typeof(TTo));

			var fromParameter = Expression.Parameter(typeof(TFrom), "from");
			var toParameter = Expression.Parameter(typeof(TTo).MakeByRefType(), "to");

			Expression body;
			if (tryParseMethod.DeclaringType == typeof(Enum))
				body = Expression.Call(tryParseMethod, fromParameter, Expression.Constant(false), toParameter);
			else
				body = Expression.Call(tryParseMethod, fromParameter, toParameter);

			return Expression.Lambda<TryConvertDelegate<TFrom,TTo>>(body, fromParameter, toParameter).Compile();
		}

		private MethodInfo GetTryParseMethod(Type sourceType, Type toType)
		{
			if (toType.IsEnum)
				return typeof(Enum).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.First(q => q.Name == nameof(Enum.TryParse) && q.IsStatic && q.GetParameters().Length == 3 && q.IsGenericMethodDefinition)
					.MakeGenericMethod(toType);
			return toType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(
				q => q.Name == "TryParse" && q.IsStatic && q.GetParameters().Length == 2
			);
		}
	}
}
