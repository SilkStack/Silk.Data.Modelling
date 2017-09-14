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
		/// <summary>
		/// Gets the name of the model.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the fields present on the model.
		/// </summary>
		public ModelField[] Fields { get; }

		/// <summary>
		/// Gets metadata present on the model.
		/// </summary>
		public object[] Metadata { get; }

		public Model(string name, IEnumerable<ModelField> fields, IEnumerable<object> metadata)
		{
			Name = name;
			Fields = fields.ToArray();
			Metadata = metadata.ToArray();
			foreach (var field in Fields)
				field.ParentModel = this;
		}

		public IView<ViewField> CreateView(params ViewConvention[] viewConventions)
		{
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
