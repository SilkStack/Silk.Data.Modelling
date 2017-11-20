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

		public ModelField GetField(string[] path)
		{
			var currentModel = this;
			ModelField result = null;
			foreach (var pathSegment in path)
			{
				result = currentModel.Fields.FirstOrDefault(q => q.Name == pathSegment);
				if (result == null)
					return null;
				currentModel = result.DataTypeModel;
			}
			return result;
		}

		protected virtual ViewDefinition CreateViewDefinition<TBuilder>(ViewConvention<TBuilder>[] viewConventions,
			Func<Model, Model, ViewConvention[], TBuilder> builderFactory, Model targetModel = null)
			where TBuilder : ViewBuilder
		{
			var viewBuilder = builderFactory(this, targetModel, viewConventions);
			var fields = targetModel?.Fields ?? Fields;

			if (viewConventions == null || viewConventions.Length == 0)
				viewConventions = _defaultConventions.OfType<ViewConvention<TBuilder>>().ToArray();

			foreach (var viewConvention in viewConventions)
			{
				if (!viewConvention.SupportedViewTypes.HasFlag(viewBuilder.Mode))
					continue;
				foreach (var field in fields)
				{
					if (viewConvention.SkipIfFieldDefined &&
						viewBuilder.ViewDefinition.FieldDefinitions.Any(q => q.Name == field.Name))
						continue;
					viewConvention.MakeModelField(viewBuilder, field);
				}
			}

			_bindEnumerableConversions.FinalizeModel(viewBuilder);

			foreach (var viewConvention in viewConventions)
			{
				if (viewConvention.SupportedViewTypes.HasFlag(viewBuilder.Mode))
				{
					viewConvention.FinalizeModel(viewBuilder);
				}
			}

			return viewBuilder.ViewDefinition;
		}

		public IView<ViewField> CreateView(params ViewConvention<ViewBuilder>[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create);
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView(Type viewType, params ViewConvention<ViewBuilder>[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create, TypeModeller.GetModelOf(viewType));
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView<TView>(params ViewConvention<ViewBuilder>[] viewConventions)
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create, TypeModeller.GetModelOf<TView>());
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public T CreateView<T>(Func<ViewDefinition, T> viewBuilder, params ViewConvention<ViewBuilder>[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create);
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T>(Func<ViewDefinition, T> viewBuilder, Type viewType, params ViewConvention<ViewBuilder>[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create, TypeModeller.GetModelOf(viewType));
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T, TView>(Func<ViewDefinition, T> viewBuilder, params ViewConvention<ViewBuilder>[] viewConventions)
			where T : IView
		{
			var viewDefinition = CreateViewDefinition(viewConventions, ViewBuilder.Create, TypeModeller.GetModelOf<TView>());
			return viewBuilder(viewDefinition);
		}

		public IView<ViewField> CreateView<TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory,
			params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory);
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView<TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory, 
			Type viewType, params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory, TypeModeller.GetModelOf(viewType));
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public IView<ViewField> CreateView<TView, TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory,
			params ViewConvention<TBuilder>[] viewConventions)
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory, TypeModeller.GetModelOf<TView>());
			return new DefaultView(viewDefinition.Name,
				ViewField.FromDefinitions(viewDefinition.FieldDefinitions),
				this, viewDefinition.ResourceLoaders);
		}

		public T CreateView<T, TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory,
			Func<ViewDefinition, T> viewBuilder, params ViewConvention<TBuilder>[] viewConventions)
			where T : IView
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory);
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T, TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory,
			Func<ViewDefinition, T> viewBuilder, Type viewType, params ViewConvention<TBuilder>[] viewConventions)
			where T : IView
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory, TypeModeller.GetModelOf(viewType));
			return viewBuilder(viewDefinition);
		}

		public T CreateView<T, TView, TBuilder>(Func<Model, Model, ViewConvention[], TBuilder> builderFactory,
			Func<ViewDefinition, T> viewBuilder, params ViewConvention<TBuilder>[] viewConventions)
			where T : IView
			where TBuilder : ViewBuilder
		{
			var viewDefinition = CreateViewDefinition(viewConventions, builderFactory, TypeModeller.GetModelOf<TView>());
			return viewBuilder(viewDefinition);
		}
	}
}
