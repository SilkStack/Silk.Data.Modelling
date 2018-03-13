using Silk.Data.Modelling.Mapping.Binding;
using System;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class CastNumericTypes : IMappingConvention
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

		public static CastNumericTypes Instance { get; } = new CastNumericTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder)
				.Where(q => (_numericTypes.Contains(q.sourceField.FieldType) || q.sourceField.FieldType.GetTypeInfo().IsEnum) &&
					(_numericTypes.Contains(q.targetField.FieldType) || q.targetField.FieldType.GetTypeInfo().IsEnum)))
			{
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CastExpressionBinding>();
			}
		}
	}
}
