using Silk.Data.Modelling.Mapping.Binding;
using System;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class CopyExplicitCast : IMappingConvention
	{
		public static CopyExplicitCast Instance { get; } = new CopyExplicitCast();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var (fromField, toField) in builder.BindingCandidatesDelegate(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);
				var castMethod = GetExplicitCast(fromType, toType);
				if (castMethod == null)
					continue;

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<ExplicitCastBinding, MethodInfo>(castMethod);
			}
		}

		private MethodInfo GetExplicitCast(Type fromType, Type toType)
		{
			return fromType.GetTypeInfo()
				.DeclaredMethods.FirstOrDefault(q => q.IsStatic && q.Name == "op_Explicit" && q.ReturnType == toType);
		}
	}
}
