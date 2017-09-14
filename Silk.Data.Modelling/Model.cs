using Silk.Data.Modelling.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Represents a data structure.
	/// </summary>
	public class Model
	{
		private static ViewConvention[] _defaultConventions = new ViewConvention[]
		{
			new CopySimpleTypesConvention(),
			new FlattenSimpleTypesConvention()
		};

		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the fields present on the model.
		/// </summary>
		public TypedModelField[] Fields { get; }

		/// <summary>
		/// Gets metadata present on the model.
		/// </summary>
		public object[] Metadata { get; }

		public Model(string name, IEnumerable<TypedModelField> fields, IEnumerable<object> metadata)
		{
			Name = name;
			Fields = fields.ToArray();
			Metadata = metadata.ToArray();
			foreach (var field in Fields)
				field.ParentModel = this;
		}

		public IView<ViewField> CreateView(params ViewConvention[] viewConventions)
		{
			if (viewConventions == null || viewConventions.Length == 0)
				viewConventions = _defaultConventions;
			var viewDefinition = new ViewDefinition(this);
			foreach (var field in Fields)
			{
				foreach (var viewConvention in viewConventions)
				{
					viewConvention.MakeModelFields(this, field, viewDefinition);
				}
			}
			foreach (var viewConvention in viewConventions)
			{
				viewConvention.FinalizeModel(viewDefinition);
			}
			return new DefaultView(viewDefinition.Name, GetDefaultViewFields(viewDefinition.FieldDefinitions));
		}

		public IView<ViewField> CreateView<TView>(params ViewConvention[] viewConventions)
		{
			var targetModel = TypeModeller.GetModelOf<TView>();
			if (viewConventions == null || viewConventions.Length == 0)
				viewConventions = _defaultConventions;
			var viewDefinition = new ViewDefinition(this);
			foreach (var field in targetModel.Fields)
			{
				foreach (var viewConvention in viewConventions)
				{
					viewConvention.MakeModelFields(this, field, viewDefinition);
				}
			}
			foreach (var viewConvention in viewConventions)
			{
				viewConvention.FinalizeModel(viewDefinition);
			}
			return new DefaultView(viewDefinition.Name, GetDefaultViewFields(viewDefinition.FieldDefinitions));
		}

		public TView CreateView<TView>(Func<ViewDefinition, TView> viewBuilder, params ViewConvention[] viewConventions)
			where TView : IView
		{
			return default(TView);
		}

		private IEnumerable<ViewField> GetDefaultViewFields(IEnumerable<ViewFieldDefinition> fieldDefinitions)
		{
			foreach (var viewDefinition in fieldDefinitions)
			{
				yield return new ViewField(viewDefinition.Name, viewDefinition.DataType,
					viewDefinition.Metadata);
			}
		}
	}
}
