using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.Conventions;
using System;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class ViewBuilder
	{
		public Model SourceModel { get; }
		public Model TargetModel { get; }
		public ViewType Mode { get; }
		public ViewDefinition ViewDefinition { get; }

		protected ViewBuilder(Model sourceModel, Model targetModel, ViewConvention[] viewConventions)
		{
			SourceModel = sourceModel;
			TargetModel = targetModel;
			Mode = TargetModel != null ? ViewType.ModelDriven : ViewType.ConventionDerived;
			ViewDefinition = new ViewDefinition(sourceModel, targetModel ?? sourceModel, viewConventions);
		}

		public virtual void DefineField(string viewFieldName, ModelBinding binding, Type fieldDataType,
			params object[] metadata)
		{
			var fieldDefinition = new ViewFieldDefinition(viewFieldName, binding)
			{
				DataType = fieldDataType
			};
			fieldDefinition.Metadata.AddRange(metadata);
			ViewDefinition.FieldDefinitions.Add(fieldDefinition);
		}

		public virtual bool IsFieldDefined(string viewFieldName)
		{
			return ViewDefinition.FieldDefinitions.Any(q => q.Name == viewFieldName);
		}

		public virtual FieldInfo FindSourceField(ModelField modelField, string name, bool caseSenitive = true, Type dataType = null)
		{
			return FindSourceField(SourceModel, modelField, name, caseSenitive, dataType);
		}

		protected virtual FieldInfo FindSourceField(Model model, ModelField modelField, string name, bool caseSenitive = true, Type dataType = null)
		{
			var field = model.Fields
				.FirstOrDefault(q => FieldSelector(q, name, caseSenitive, dataType));
			if (field == null)
				return null;

			return new FieldInfo(field, GetBindingDirection(modelField, field));
		}

		public virtual FieldInfo FindSourceField(ModelField modelField, string[] path, bool caseSenitive = true, Type dataType = null)
		{
			return FindSourceField(SourceModel, modelField, path, caseSenitive, dataType);
		}

		protected virtual FieldInfo FindSourceField(Model model, ModelField modelField, string[] path, bool caseSenitive = true, Type dataType = null)
		{
			ModelField field = null;
			foreach (var pathComponent in path)
			{
				field = model.Fields.FirstOrDefault(q => FieldSelector(q, pathComponent, caseSenitive, null));
				if (field == null)
					break;
				model = field.DataTypeModel;
			}

			if (field == null)
				return null;

			if (dataType != null && field.DataType != dataType)
				return null;

			return new FieldInfo(field, GetBindingDirection(modelField, field));
		}

		private BindingDirection GetBindingDirection(ModelField modelField, ModelField viewFieldCandidate)
		{
			var bindingDirection = BindingDirection.None;

			if (modelField.CanRead && viewFieldCandidate.CanWrite)
				bindingDirection |= BindingDirection.ViewToModel;
			if (viewFieldCandidate.CanRead && modelField.CanWrite)
				bindingDirection |= BindingDirection.ModelToView;

			return bindingDirection;
		}

		private bool FieldSelector(ModelField field, string name, bool caseSensitive, Type dataType)
		{
			if (dataType != null && field.DataType != dataType)
				return false;

			if (caseSensitive)
				return name == field.Name;

			return name.ToLowerInvariant() == field.Name.ToLowerInvariant();
		}

		public static ViewBuilder Create(Model sourceModel, Model targetModel,
			ViewConvention[] viewConventions)
		{
			return new ViewBuilder(sourceModel, targetModel, viewConventions);
		}

		public class FieldInfo
		{
			public ModelField Field { get; }
			public BindingDirection BindingDirection { get; }

			public FieldInfo(ModelField field, BindingDirection bindingDirection)
			{
				Field = field;
				BindingDirection = bindingDirection;
			}
		}
	}
}
