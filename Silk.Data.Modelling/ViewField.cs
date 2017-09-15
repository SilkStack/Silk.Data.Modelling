using Silk.Data.Modelling.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Silk.Data.Modelling
{
	[DebuggerDisplay("{Name} {DataType}")]
	public class ViewField : IViewField
	{
		public string Name { get; }

		public Type DataType { get; }

		public object[] Metadata { get; }

		public IModelBinding ModelBinding { get; }

		public ViewField(string name, Type dataType, IEnumerable<object> metadata,
			IModelBinding binding)
		{
			Name = name;
			DataType = dataType;
			Metadata = metadata.ToArray();
			ModelBinding = binding;
		}

		public static ViewField FromDefinition(ViewFieldDefinition definition)
		{
			return new ViewField(definition.Name, definition.DataType, definition.Metadata,
				definition.ModelBinding);
		}

		public static IEnumerable<ViewField> FromDefinitions(IEnumerable<ViewFieldDefinition> definitions)
		{
			return definitions.Select(q => FromDefinition(q));
		}
	}
}
