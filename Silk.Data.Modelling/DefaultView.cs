using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class DefaultView : IView<ViewField>
	{
		public string Name { get; }
		public ViewField[] Fields { get; }
		public Model Model { get; }

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
		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			Model model)
			: base(name, fields, model)
		{
		}
	}

	public class TypedDefaultView<TSource, TView> : DefaultView, IView<ViewField, TSource, TView>
	{
		public TypedDefaultView(string name, IEnumerable<ViewField> fields,
			Model model)
			: base(name, fields, model)
		{
		}
	}
}
