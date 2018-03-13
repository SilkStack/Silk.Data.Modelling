using Silk.Data.Modelling.Mapping.Binding;
using System;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class CopyTryParse : IMappingConvention
	{
		public static CopyTryParse Instance { get; } = new CopyTryParse();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			//foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q)))
			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder)
				.Where(q => q.sourceField.FieldType == typeof(string)))
			{
				var parseMethod = GetTryParseMethod(fromField, toField);
				if (parseMethod == null)
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<TryParseBinding, MethodInfo>(parseMethod);
			}
		}

		private MethodInfo GetTryParseMethod(ISourceField sourceField, ITargetField targetField)
		{
			var toType = targetField.FieldType.GetTypeInfo();
			if (toType.IsEnum)
				return typeof(Enum).GetTypeInfo().DeclaredMethods
					.First(q => q.Name == nameof(Enum.TryParse) && q.IsStatic && q.GetParameters().Length == 3 && q.IsGenericMethodDefinition)
					.MakeGenericMethod(targetField.FieldType);
			return toType.DeclaredMethods.FirstOrDefault(
				q => q.Name == "TryParse" && q.IsStatic && q.GetParameters().Length == 2
			);
		}
	}
}
