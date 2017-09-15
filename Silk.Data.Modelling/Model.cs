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
			new FlattenSimpleTypesConvention(),
			new CopyReferencesConvention()
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

		protected virtual ViewDefinition CreateViewDefinition(ViewConvention[] viewConventions, Model targetModel = null)
		{
			if (targetModel == null)
				targetModel = this;

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

			return viewDefinition;
		}

		public IView<ViewField> CreateView(params ViewConvention[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions);
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this);
		}

		public IView<ViewField> CreateView<TView>(params ViewConvention[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, TypeModeller.GetModelOf<TView>());
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this);
		}

		public T CreateView<T>(Func<ViewDefinition, T> viewBuilder, params ViewConvention[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions);
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T, TView>(Func<ViewDefinition, T> viewBuilder, params ViewConvention[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions, TypeModeller.GetModelOf<TView>());
			return viewBuilder(viewDefinition);
		}
	}
}
