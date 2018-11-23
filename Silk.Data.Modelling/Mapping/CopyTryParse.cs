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
			foreach (var (fromField, toField) in builder.BindingCandidatesDelegate(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);
				if (fromType != typeof(string))
					continue;

				var parseMethod = GetTryParseMethod(fromType, toType);
				if (parseMethod == null)
					continue;
				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<TryParseBinding, MethodInfo>(parseMethod);
			}
		}

		private MethodInfo GetTryParseMethod(Type sourceType, Type toType)
		{
			var toTypeInfo = toType.GetTypeInfo();
			if (toTypeInfo.IsEnum)
				return typeof(Enum).GetTypeInfo().DeclaredMethods
					.First(q => q.Name == nameof(Enum.TryParse) && q.IsStatic && q.GetParameters().Length == 3 && q.IsGenericMethodDefinition)
					.MakeGenericMethod(toType);
			return toTypeInfo.DeclaredMethods.FirstOrDefault(
				q => q.Name == "TryParse" && q.IsStatic && q.GetParameters().Length == 2
			);
		}
	}
}
