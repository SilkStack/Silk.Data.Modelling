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

		public IViewFieldBinding Binding { get; }

		public ViewField(string name, Type dataType, IEnumerable<object> metadata,
			IViewFieldBinding binding)
		{
			Name = name;
			DataType = dataType;
			Metadata = metadata.ToArray();
			Binding = binding;
		}
	}
}
