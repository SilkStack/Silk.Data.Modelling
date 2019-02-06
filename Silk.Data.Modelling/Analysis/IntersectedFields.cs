using System;

namespace Silk.Data.Modelling.Analysis
{
	public abstract class IntersectedFields
	{
		public IField LeftField { get; }
		public IField RightField { get; }

		protected IntersectedFields(IField leftField, IField rightField)
		{
			LeftField = leftField;
			RightField = rightField;
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
	public abstract class IntersectedFields<TLeftField, TRightField> : IntersectedFields
		where TLeftField : IField
		where TRightField : IField
	{
		public new TLeftField LeftField { get; }
		public new TRightField RightField { get; }

		protected IntersectedFields(TLeftField leftField, TRightField rightField)
			: base(leftField, rightField)
		{
			LeftField = leftField;
			RightField = rightField;
		}

		/// <summary>
		/// Creates an instance of IntersectedFields for the provided fields.
		/// </summary>
		/// <param name="leftField"></param>
		/// <param name="rightField"></param>
		/// <returns></returns>
		public static IntersectedFields<TLeftField, TRightField> Create(TLeftField leftField, TRightField rightField)
		{
			return Activator.CreateInstance(
				typeof(IntersectedFields<,,,>)
					.MakeGenericType(typeof(TLeftField), typeof(TRightField), leftField.FieldDataType, rightField.FieldDataType),
				leftField, rightField
				) as IntersectedFields<TLeftField, TRightField>;
		}
	}

	public class IntersectedFields<TLeftField, TRightField, TLeftData, TRightData> : IntersectedFields<TLeftField, TRightField>
		where TLeftField : IField
		where TRightField : IField
	{
		public IntersectedFields(TLeftField leftField, TRightField rightField) : base(leftField, rightField)
		{
		}

		public override void ExecuteGenericEntryPoint(ILeftRightIntersectionGenericExecutor genericEntryPoint)
			=> genericEntryPoint.Execute(this);
	}
}
