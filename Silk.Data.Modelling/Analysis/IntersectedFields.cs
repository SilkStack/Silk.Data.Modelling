using System;

namespace Silk.Data.Modelling.Analysis
{
	public abstract class IntersectedFields
	{
		public IField LeftField { get; }
		public IFieldPath LeftPath { get; }
		public IField RightField { get; }
		public IFieldPath RightPath { get; }

		protected IntersectedFields(IField leftField, IField rightField,
			IFieldPath leftPath, IFieldPath rightPath)
		{
			LeftField = leftField;
			RightField = rightField;
			LeftPath = leftPath;
			RightPath = rightPath;
		}

		/// <summary>
		/// Execute a generic entry point with the generic type parameters provided by the IntersectedFields implementation.
		/// </summary>
		/// <param name="genericEntryPoint"></param>
		public abstract void ExecuteGenericEntryPoint(ILeftRightIntersectionGenericExecutor genericEntryPoint);
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
		public new FieldPath<TLeftModel, TLeftField> LeftPath { get; }

		public new TRightField RightField { get; }
		public new FieldPath<TRightModel, TRightField> RightPath { get; }

		protected IntersectedFields(TLeftField leftField, TRightField rightField,
			FieldPath<TLeftModel, TLeftField> leftPath, FieldPath<TRightModel, TRightField> rightPath)
			: base(leftField, rightField, leftPath, rightPath)
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
		public static IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> Create(TLeftField leftField, TRightField rightField,
			FieldPath<TLeftModel, TLeftField> leftPath, FieldPath<TRightModel, TRightField> rightPath)
		{
			return Activator.CreateInstance(
				typeof(IntersectedFields<,,,,,>)
					.MakeGenericType(
						typeof(TLeftModel), typeof(TLeftField),
						typeof(TRightModel), typeof(TRightField),
						leftField.FieldDataType, rightField.FieldDataType
						),
				leftField, rightField, leftPath, rightPath
				) as IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>;
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
			FieldPath<TLeftModel, TLeftField> leftPath, FieldPath<TRightModel, TRightField> rightPath) :
			base(leftField, rightField, leftPath, rightPath)
		{
		}

		public override void ExecuteGenericEntryPoint(ILeftRightIntersectionGenericExecutor genericEntryPoint)
			=> genericEntryPoint.Execute(this);
	}
}
