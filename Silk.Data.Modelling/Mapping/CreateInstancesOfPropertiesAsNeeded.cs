using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	public class CreateInstancesOfPropertiesAsNeeded : IMappingConvention
	{
		public static CreateInstancesOfPropertiesAsNeeded Instance { get; }
			= new CreateInstancesOfPropertiesAsNeeded();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !q.IsEnumerable && MapReferenceTypes.IsReferenceType(q.FieldType) && !builder.IsBound(q)))
			{
				var ctor = toField.FieldType.GetTypeInfo().DeclaredConstructors
					.FirstOrDefault(q => q.GetParameters().Length == 0);
				if (ctor == null)
				{
					continue;
				}

				builder
					.Bind(toField)
					.AssignUsing<CreateInstanceIfNull, ConstructorInfo>(ctor);
			}
		}
	}
}
