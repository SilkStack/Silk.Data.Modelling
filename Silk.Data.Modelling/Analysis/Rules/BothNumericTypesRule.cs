using Silk.Data.Modelling.Analysis.CandidateSources;
using System;
using System.Linq;

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
		IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
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

		public bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			if (!_numericTypes.Contains(intersectCandidate.LeftField.RemoveEnumerableType()) ||
				!_numericTypes.Contains(intersectCandidate.RightField.RemoveEnumerableType()) ||
				intersectCandidate.LeftField.IsEnumerableType != intersectCandidate.RightField.IsEnumerableType)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>.Create(
				intersectCandidate.LeftField, intersectCandidate.RightField,
				intersectCandidate.LeftPath, intersectCandidate.RightPath
				);
			return true;
		}
	}
}
