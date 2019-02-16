using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Analysis.Rules
{
	public abstract class IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public abstract bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields);

		protected IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> BuildIntersectedFields(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate
			)
		{
			var builder = new IntersectedFieldBuilder(candidate, this);
			candidate.Dispatch(builder);
			return builder.IntersectedFields;
		}

		protected abstract TryConvertDelegate<TFrom, TTo> TryConvertFactory<TFrom, TTo>();

		private class IntersectedFieldBuilder : IIntersectCandidateGenericExecutor
		{
			private readonly IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> _candidate;
			private readonly IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField> _rule;

			public IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> IntersectedFields { get; private set; }

			public IntersectedFieldBuilder(
				IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate,
				IntersectionRuleBase<TLeftModel, TLeftField, TRightModel, TRightField> rule
				)
			{
				_candidate = candidate;
				_rule = rule;
			}

			void IIntersectCandidateGenericExecutor.Execute<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData>(
				IntersectCandidate<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData> intersectCandidate
				)
			{
				IntersectedFields = new IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
					_candidate.LeftField, _candidate.RightField,
					_candidate.LeftPath, _candidate.RightPath,
					_rule.GetType(),
					new TryConvertFactory<TLeftData, TRightData>(_rule.TryConvertFactory<TLeftData, TRightData>)
					);
			}
		}
	}
}
