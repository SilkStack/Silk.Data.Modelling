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
			new CopyReferencesConvention(),
			new MapReferenceTypesConvention()
		};
		private static EnumerableConversionsConvention _bindEnumerableConversions = new EnumerableConversionsConvention();

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

		public TypedModelField GetField(string[] path)
		{
			var currentModel = this;
			TypedModelField result = null;
			foreach (var pathSegment in path)
			{
				result = currentModel.Fields.FirstOrDefault(q => q.Name == pathSegment);
				if (result == null)
					return null;
				currentModel = result.DataTypeModel;
			}
			return result;
		}

		protected virtual ViewDefinition CreateViewDefinition(ViewConvention[] viewConventions, Model targetModel = null)
		{
			if (targetModel == null)
				targetModel = this;

			if (viewConventions == null || viewConventions.Length == 0)
				viewConventions = _defaultConventions;
			var viewDefinition = new ViewDefinition(this, targetModel, viewConventions);
			foreach (var field in targetModel.Fields)
			{
				foreach (var viewConvention in viewConventions)
				{
					viewConvention.MakeModelFields(this, field, viewDefinition);
				}
			}
			_bindEnumerableConversions.FinalizeModel(viewDefinition);
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
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView(Type viewType, params ViewConvention[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, TypeModeller.GetModelOf(viewType));
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView<TView>(params ViewConvention[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, TypeModeller.GetModelOf<TView>());
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public T CreateView<T>(Func<ViewDefinition, T> viewBuilder, params ViewConvention[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions);
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T>(Func<ViewDefinition, T> viewBuilder, Type viewType, params ViewConvention[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions, TypeModeller.GetModelOf(viewType));
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
