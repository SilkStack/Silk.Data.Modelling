using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// Mapping convention for use in object mapping. Checks for usable ctors and creates instances as needed.
	/// </summary>
	public class CreateInstanceAsNeeded : IMappingConvention
	{
		public static CreateInstanceAsNeeded Instance { get; } = new CreateInstanceAsNeeded();

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			var typeModel = toModel.FromModel as TypeModel;
			if (typeModel == null)
				return;
			var ctor = typeModel.Type.GetTypeInfo().DeclaredConstructors
				.FirstOrDefault(q => q.GetParameters().Length == 0);
			//  todo: support providing a factory for the type or assuming the member will already be instantiated
			if (ctor == null)
			{
				throw new MappingRequirementException($"A constructor with 0 parameters is required on type {typeModel.Type}.");
			}

			builder
				.Bind(toModel.GetSelf())
				.AssignUsing<CreateInstanceIfNull, ConstructorInfo>(ctor);
		}
	}
}
