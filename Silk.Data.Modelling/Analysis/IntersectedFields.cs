using Silk.Data.Modelling.Analysis.CandidateSources;
using Silk.Data.Modelling.GenericDispatch;
using System;

namespace Silk.Data.Modelling.Analysis
{
	public delegate bool TryConvertDelegate<TFrom, TTo>(TFrom from, out TTo to);

	public abstract class IntersectedFields
	{
		public IField LeftField { get; }
		public IFieldPath LeftPath { get; }
		public IField RightField { get; }
		public IFieldPath RightPath { get; }

		/// <summary>
		/// Gets the Type of the intersection rule that created the intersected fields.
		/// </summary>
		public Type IntersectionRuleType { get; }

		/// <summary>
		/// Gets any metadata that was set by the intersection rule.
		/// </summary>
		public object IntersectionMetadata { get; }

		protected IntersectedFields(IField leftField, IField rightField,
			IFieldPath leftPath, IFieldPath rightPath, Type intersectionRuleType,
			object intersectionMetadata)
		{
			LeftField = leftField;
			RightField = rightField;
			LeftPath = leftPath;
			RightPath = rightPath;
			IntersectionRuleType = intersectionRuleType;
			IntersectionMetadata = intersectionMetadata;
		}

		/// <summary>
		/// Execute a generic entry point with the generic type parameters provided by the IntersectedFields implementation.
		/// </summary>
		/// <param name="genericEntryPoint"></param>
		public abstract void Dispatch(IIntersectedFieldsGenericExecutor genericEntryPoint);
	}

	/// <summary>
	/// Intersection of two fields.
	/// </summary>
	public abstract class IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> : IntersectedFields
		where TLeftField : class, IField
		where TRightField : class, IField
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
	{
		public new TLeftField LeftField { get; }
		public new IFieldPath<TLeftModel, TLeftField> LeftPath { get; }

		public new TRightField RightField { get; }
		public new IFieldPath<TRightModel, TRightField> RightPath { get; }

		protected IntersectedFields(TLeftField leftField, TRightField rightField,
			IFieldPath<TLeftModel, TLeftField> leftPath, IFieldPath<TRightModel, TRightField> rightPath,
			Type intersectionRuleType, object intersectionMetadata)
			: base(leftField, rightField, leftPath, rightPath, intersectionRuleType, intersectionMetadata)
		{
			LeftField = leftField;
			RightField = rightField;
			LeftPath = leftPath;
			RightPath = rightPath;
		}

		/// <summary>
		/// Creates an instance of IntersectedFields for the provided fields.
		/// </summary>
		/// <param name="leftField"></param>
		/// <param name="rightField"></param>
		/// <returns></returns>
		public static IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> Create(
			IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate, Type intersectionRuleType,
			object intersectionMetadata = null)
		{
			var builder = new FieldBuilder(candidate, intersectionRuleType, intersectionMetadata);
			candidate.Dispatch(builder);
			return builder.IntersectedFields;
		}

		private class FieldBuilder : IIntersectCandidateGenericExecutor
		{
			private readonly IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> _candidate;
			private readonly Type _ruleType;
			private readonly object _metadata;

			public IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> IntersectedFields { get; private set; }

			public FieldBuilder(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> candidate,
				Type intersectionRuleType, object intersectionMetadata)
			{
				_candidate = candidate;
				_ruleType = intersectionRuleType;
				_metadata = intersectionMetadata;
			}

			void IIntersectCandidateGenericExecutor.Execute<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData>(IntersectCandidate<TLeftModel1, TLeftField1, TRightModel1, TRightField1, TLeftData, TRightData> intersectCandidate)
			{
				IntersectedFields = new IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
					_candidate.LeftField, _candidate.RightField,
					_candidate.LeftPath, _candidate.RightPath,
					_ruleType, _metadata
					);
			}
		}
	}

	public class IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> :
		IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
	{
		public IntersectedFields(TLeftField leftField, TRightField rightField,
			IFieldPath<TLeftModel, TLeftField> leftPath, IFieldPath<TRightModel, TRightField> rightPath,
			Type intersectionRuleType, object intersectionMetadata) :
			base(leftField, rightField, leftPath, rightPath, intersectionRuleType, intersectionMetadata)
		{
		}

		public TryConvertDelegate<TLeftData, TRightData> GetConvertDelegate()
		{
			return null;
		}

		public override void Dispatch(IIntersectedFieldsGenericExecutor genericEntryPoint)
			=> genericEntryPoint.Execute(this);
	}
}
