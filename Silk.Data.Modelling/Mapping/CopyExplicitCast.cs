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
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q)))
			{
				var fromField = fromModel.Fields.FirstOrDefault(field => field.CanRead &&
					field.FieldName == toField.FieldName);
				if (fromField == null)
					continue;
				var castMethod = GetExplicitCast(fromField.FieldType, toField.FieldType);
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
