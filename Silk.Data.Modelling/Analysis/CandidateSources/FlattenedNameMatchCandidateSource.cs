using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	/// <summary>
	/// Finds intersect candidates with a matching flattened path.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class FlattenedNameMatchCandidateSource<TLeftModel, TLeftField, TRightModel, TRightField> :
		CandidateSourceBase<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Gets or sets a maximum depth to search for candidates.
		/// Defaults to 10.
		/// </summary>
		public int MaxDepth { get; set; } = 10;

		public override IEnumerable<IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField>> GetIntersectCandidates(TLeftModel leftModel, TRightModel rightModel)
		{
			var flatLeft = Flatten<TLeftModel, TLeftField>(leftModel).ToArray();
			var flatRight = Flatten<TRightModel, TRightField>(rightModel).ToArray();

			foreach (var leftField in flatLeft)
			{
				foreach (var rightField in flatRight)
				{
					if (leftField.FlatFieldPath == rightField.FlatFieldPath)
					{
						yield return BuildIntersectCandidate(leftField.FieldPath, rightField.FieldPath);
					}
				}
			}
		}

		private IEnumerable<FlatField<TModel, TField>> Flatten<TModel, TField>(TModel model)
			where TModel : IModel<TField>
			where TField : class, IField
		{
			foreach (var field in FlattenPath(model.Fields, new FieldPath<TModel, TField>(model, null, new TField[0]), 0))
				yield return field;

			IEnumerable<FlatField<TModel, TField>> FlattenPath(
				IEnumerable<TField> fields,
				FieldPath<TModel, TField> path,
				int depth
				)
			{
				if (depth == MaxDepth)
					yield break;

				foreach (var field in fields)
				{
					var fieldPath = path.Child(field);

					yield return new FlatField<TModel, TField>(
						string.Join("", fieldPath.Fields.Select(q => q.FieldName)),
						fieldPath,
						model,
						field
						);

					foreach (var subField in FlattenPath(model.GetPathFields(fieldPath), fieldPath, depth + 1))
						yield return subField;
				}
			}
		}

		private class FlatField<TModel, TField>
			where TModel : IModel<TField>
			where TField : class, IField
		{
			public string FlatFieldPath { get; }
			public FieldPath<TModel, TField> FieldPath { get; }
			public TModel Model { get; }
			public TField Field { get; }

			public FlatField(string flatFieldPath, FieldPath<TModel, TField> fieldPath,
				TModel model, TField field)
			{
				FlatFieldPath = flatFieldPath;
				FieldPath = fieldPath;
				Model = model;
				Field = field;
			}
		}
	}
}
