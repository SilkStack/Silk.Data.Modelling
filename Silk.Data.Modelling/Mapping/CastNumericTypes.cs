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
			foreach (var (fromField, toField) in builder.BindingCandidatesDelegate(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);
				if (!_numericTypes.Contains(fromType) && !fromType.GetTypeInfo().IsEnum)
					continue;
				if (!_numericTypes.Contains(toType) && !toType.GetTypeInfo().IsEnum)
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<CastExpressionBinding>();
			}
		}
	}
}
