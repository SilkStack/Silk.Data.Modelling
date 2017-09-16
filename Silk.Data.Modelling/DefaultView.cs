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

		public DefaultView(string name, IEnumerable<ViewField> fields,
			Model model)
		{
			Name = name;
			Fields = fields.ToArray();
			Model = model;
		}
	}

	public class TypedDefaultView<TSource> : DefaultView, IView<ViewField, TSource>
	{
		public new TypedModel<TSource> Model { get; }

		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			TypedModel<TSource> model)
			: base(name, fields, model)
		{
			Model = model;
		}
	}

	public class TypedDefaultView<TSource, TView> : TypedDefaultView<TSource>, IView<ViewField, TSource, TView>
	{
		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			TypedModel<TSource> model)
			: base(name, fields, model)
		{
		}
	}
}
