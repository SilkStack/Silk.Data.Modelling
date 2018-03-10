using System.Linq;
using Silk.Data.Modelling.Mapping.Binding;

namespace Silk.Data.Modelling.Mapping
{
	public class ConvertToStringWithToString : IMappingConvention
	{
		public static ConvertToStringWithToString Instance { get; } = new ConvertToStringWithToString();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach(var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q) && q.FieldType == typeof(string)))
			{
				var fromField = fromModel.Fields.FirstOrDefault(q => q.CanRead &&
					q.FieldName == toField.FieldName);
				if (fromField == null)
					continue;

				builder
					.Bind(toField)
					.From(fromField)
					.MapUsing<ToStringBinding>();
			}
		}
	}
}