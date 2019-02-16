using Silk.Data.Modelling.GenericDispatch;
using System;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	public abstract class CandidateSourceBase<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public abstract IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(
			TLeftModel leftModel, TRightModel rightModel
			);

		protected IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> BuildIntersectCandidate(
			FieldPath<TLeftModel, TLeftField> leftPath, FieldPath<TRightModel, TRightField> rightPath
			)
		{
			var builder = new CandidateBuilder(leftPath, rightPath, GetType());
			leftPath.FinalField.Dispatch(builder);
			return builder.Candidate;
		}

		private class CandidateBuilder : IFieldGenericExecutor
		{
			private readonly FieldPath<TLeftModel, TLeftField> _leftPath;
			private readonly FieldPath<TRightModel, TRightField> _rightPath;
			private readonly Type _candidateSourceType;

			public IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> Candidate { get; private set; }

			public CandidateBuilder(FieldPath<TLeftModel, TLeftField> leftPath,
				FieldPath<TRightModel, TRightField> rightPath,
				Type candidateSourceType)
			{
				_leftPath = leftPath;
				_rightPath = rightPath;
				_candidateSourceType = candidateSourceType;
			}

			void IFieldGenericExecutor.Execute<TField, TData>(IField field)
			{
				var builder = new CandidateBuilder<TData>(_leftPath, _rightPath, _candidateSourceType);
				_rightPath.FinalField.Dispatch(builder);
				Candidate = builder.Candidate;
			}
		}

		private class CandidateBuilder<TLeft> : IFieldGenericExecutor
		{
			private readonly FieldPath<TLeftModel, TLeftField> _leftPath;
			private readonly FieldPath<TRightModel, TRightField> _rightPath;
			private readonly Type _candidateSourceType;

			public IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> Candidate { get; private set; }

			public CandidateBuilder(FieldPath<TLeftModel, TLeftField> leftPath,
				FieldPath<TRightModel, TRightField> rightPath,
				Type candidateSourceType)
			{
				_leftPath = leftPath;
				_rightPath = rightPath;
				_candidateSourceType = candidateSourceType;
			}

			void IFieldGenericExecutor.Execute<TField, TData>(IField field)
			{
				Candidate = new IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField, TLeft, TData>(
					_leftPath, _rightPath, _candidateSourceType
					);
			}
		}
	}
}
