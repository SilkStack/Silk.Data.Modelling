using System;
using System.Collections.Generic;
using System.Linq;
using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Analysis.Rules
{
	public class TypeConverterRule<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		private readonly ITypeConverter[] _typeConverters;

		public TypeConverterRule(IEnumerable<ITypeConverter> typeConverters)
		{
			_typeConverters = typeConverters.ToArray();
		}

		public bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			var converter = _typeConverters.FirstOrDefault(q =>
				q.FromType == intersectCandidate.LeftField.RemoveEnumerableType() &&
				q.ToType == intersectCandidate.RightField.RemoveEnumerableType());
			if (converter == null)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = BuildIntersectedFields(intersectCandidate, converter);
			return true;
		}

		protected IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> BuildIntersectedFields(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate,
			ITypeConverter converter
			)
		{
			var builder = new IntersectedFieldBuilder(candidate, this, converter);
			candidate.Dispatch(builder);
			return builder.IntersectedFields;
		}

		private class IntersectedFieldBuilder : IIntersectCandidateGenericExecutor
		{
			private readonly IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> _candidate;
			private readonly IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField> _rule;
			private readonly ITypeConverter _converter;

			public IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> IntersectedFields { get; private set; }

			public IntersectedFieldBuilder(
				IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate,
				IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField> rule,
				ITypeConverter converter
				)
			{
				_candidate = candidate;
				_rule = rule;
				_converter = converter;
			}

			void IIntersectCandidateGenericExecutor.Execute<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData>(
				IntersectCandidate<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData> intersectCandidate
				)
			{
				var converter = _converter as ITypeConverter<TLeftData, TRightData>;
				if (converter == null)
					throw new InvalidOperationException("Type converter isn't of the correct type.");

				IntersectedFields = new IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
					_candidate.LeftField, _candidate.RightField,
					_candidate.LeftPath, _candidate.RightPath,
					_rule.GetType(),
					new TryConvertFactory<TLeftData, TRightData>(() => converter.TryConvert)
					);
			}
		}
	}
}
