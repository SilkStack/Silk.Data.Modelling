using Silk.Data.Modelling.GenericDispatch;
using System;

namespace Silk.Data.Modelling.Analysis
{
	public delegate bool TryConvertDelegate<TFrom, TTo>(TFrom from, out TTo to);

	public delegate TryConvertDelegate<TFrom, TTo> TryConvertFactory<TFrom, TTo>();

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

		protected IntersectedFields(IField leftField, IField rightField,
			IFieldPath leftPath, IFieldPath rightPath, Type intersectionRuleType)
		{
			LeftField = leftField;
			RightField = rightField;
			LeftPath = leftPath;
			RightPath = rightPath;
			IntersectionRuleType = intersectionRuleType;
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
			Type intersectionRuleType)
			: base(leftField, rightField, leftPath, rightPath, intersectionRuleType)
		{
			LeftField = leftField;
			RightField = rightField;
			LeftPath = leftPath;
			RightPath = rightPath;
		}
	}

	public class IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> :
		IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
	{
		private readonly TryConvertFactory<TLeftData, TRightData> _convertDelegateFactory;
		private TryConvertDelegate<TLeftData, TRightData> _convertDelegate;

		public IntersectedFields(TLeftField leftField, TRightField rightField,
			IFieldPath<TLeftModel, TLeftField> leftPath, IFieldPath<TRightModel, TRightField> rightPath,
			Type intersectionRuleType,
			TryConvertFactory<TLeftData, TRightData> convertDelegateFactory) :
			base(leftField, rightField, leftPath, rightPath, intersectionRuleType)
		{
			_convertDelegateFactory = convertDelegateFactory;
		}

		public TryConvertDelegate<TLeftData, TRightData> GetConvertDelegate()
		{
			if (_convertDelegate == null)
				_convertDelegate = _convertDelegateFactory();
			return _convertDelegate;
		}

		public override void Dispatch(IIntersectedFieldsGenericExecutor genericEntryPoint)
			=> genericEntryPoint.Execute(this);
	}
}
