using Silk.Data.Modelling.Mapping.Binding;
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

			if (typeModel.Type.GetEnumerableElementType() != null)
				return;

			if (!MapReferenceTypes.IsReferenceType(typeModel.Type))
				return;

			var selfField = toModel.GetSelf();
			if (builder.IsBound(selfField))
				return;

			var ctor = typeModel.Type.GetTypeInfo().DeclaredConstructors
				.FirstOrDefault(q => q.GetParameters().Length == 0);
			if (ctor == null)
			{
				throw new MappingRequirementException($"A constructor with 0 parameters is required on type {typeModel.Type}.");
			}

			builder
				.Bind(selfField)
				.AssignUsing<CreateInstanceIfNull, ConstructorInfo>(ctor);
		}
	}
}
