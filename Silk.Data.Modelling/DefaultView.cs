using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class DefaultView : IView<ViewField>
	{
		public string Name { get; }
		public ViewField[] Fields { get; }
		public Model Model { get; }
		IViewField[] IView.Fields => Fields;
		public IResourceLoader[] ResourceLoaders { get; }

		public DefaultView(string name, IEnumerable<ViewField> fields,
			Model model, IEnumerable<IResourceLoader> resourceLoaders)
		{
			Name = name;
			Fields = fields.ToArray();
			Model = model;
			ResourceLoaders = resourceLoaders.ToArray();
		}
	}

	public class TypedDefaultView<TSource> : DefaultView, IView<ViewField, TSource>
	{
		public new TypedModel<TSource> Model { get; }

		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			TypedModel<TSource> model, IEnumerable<IResourceLoader> resourceLoaders)
			: base(name, fields, model, resourceLoaders)
		{
			Model = model;
		}
	}

	public class TypedDefaultView<TSource, TView> : TypedDefaultView<TSource>, IView<ViewField, TSource, TView>
	{
		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			TypedModel<TSource> model, IEnumerable<IResourceLoader> resourceLoaders)
			: base(name, fields, model, resourceLoaders)
		{
		}
	}
}
