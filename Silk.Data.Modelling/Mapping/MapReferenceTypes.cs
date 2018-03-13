﻿using Silk.Data.Modelling.Mapping.Binding;
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
			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder)
				.Where(q => IsReferenceType(q.sourceField.FieldType) && IsReferenceType(q.targetField.FieldType)))
			{
				var fromElementType = fromField.ElementType;
				var toElementType = toField.ElementType;
				var fromTypeModel = fromField.FieldTypeModel;
				var toTypeModel = toField.FieldTypeModel;

				if (fromElementType != null && toElementType != null)
				{
					if (IsReferenceType(fromElementType) && IsReferenceType(toElementType))
					{
						fromTypeModel = TypeModel.GetModelOf(fromElementType);
						toTypeModel = TypeModel.GetModelOf(toElementType);
					}
					else
					{
						continue;
					}
				}

				//  if a mapping already exists, use it, otherwise build it
				if (!builder.MappingStore.TryGetMapping(fromTypeModel, toTypeModel, out var subMapping) &&
					!builder.BuilderStack.IsBeingMapped(fromTypeModel, toTypeModel))
				{
					var subBuilder = new MappingBuilder(fromTypeModel, toTypeModel,
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
		public static bool IsReferenceType(Type type)
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
