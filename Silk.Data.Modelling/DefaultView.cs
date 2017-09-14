using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	public class DefaultView : IView<ViewField>
	{
		public string Name { get; }
		public ViewField[] Fields { get; }

		public DefaultView(string name, IEnumerable<ViewField> fields)
		{
			Name = name;
			Fields = fields.ToArray();
		}
	}

	public class TypedDefaultView<TSource> : DefaultView, IView<ViewField, TSource>
	{
		public TypedDefaultView(string name, IEnumerable<ViewField> fields)
			: base(name, fields)
		{
		}
	}

	public class TypedDefaultView<TSource, TView> : DefaultView, IView<ViewField, TSource, TView>
	{
		public TypedDefaultView(string name, IEnumerable<ViewField> fields)
			: base(name, fields)
		{
		}
	}
}
