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
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q) &&
				(_numericTypes.Contains(q.FieldType) || q.FieldType.GetTypeInfo().IsEnum)))
			{
				var fromField = fromModel.Fields.FirstOrDefault(q => q.CanRead &&
					q.FieldName == toField.FieldName &&
					(_numericTypes.Contains(q.FieldType) || q.FieldType.GetTypeInfo().IsEnum));

				if (fromField == null)
					continue;

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CastExpressionBinding>();
			}
		}
	}
}
