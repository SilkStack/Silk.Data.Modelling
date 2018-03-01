using System;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class MapReferenceTypes : IMappingConvention
	{
		public static MapReferenceTypes Instance { get; } = new MapReferenceTypes();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q) && IsReferenceType(q.FieldType)))
			{
				var fromField = fromModel.Fields.FirstOrDefault(field => field.CanRead &&
					field.FieldName == toField.FieldName &&
					IsReferenceType(field.FieldType));
				if (fromField == null)
					continue;

				//  if a mapping already exists, use it, otherwise build it
				if (!builder.MappingStore.TryGetMapping(fromField.FieldTypeModel, toField.FieldTypeModel, out var subMapping) &&
					!builder.BuilderStack.IsBeingMapped(fromField.FieldTypeModel, toField.FieldTypeModel))
				{
					var subBuilder = new MappingBuilder(fromField.FieldTypeModel, toField.FieldTypeModel,
						builder.MappingStore, builder.BuilderStack);
					foreach (var convention in builder.Conventions)
					{
						subBuilder.AddConvention(convention);
					}
					subMapping = subBuilder.BuildMapping();
				}

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<SubmappingBinding, MappingStore>(builder.MappingStore);
			}
		}

		/// <summary>
		/// Checks if a type is a reference type. Nullable<T> is not considered a reference type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsReferenceType(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			if (typeInfo.IsValueType)
				return false;
			if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
				return false;
			return true;
		}
	}
}
