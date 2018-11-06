using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class CreateInstanceAsNeededTests
	{
		[TestMethod]
		public void ThrowExceptionWithNoUsableCtor()
		{
			var exceptionCaught = false;
			try
			{
				CreateMapping<PocoWithComplexCtor>();
			}
			catch (MappingRequirementException)
			{
				exceptionCaught = true;
			}
			Assert.IsTrue(exceptionCaught);
		}

		[TestMethod]
		public void HonorExistingSelfBinding()
		{
			var options = MappingOptions.CreateObjectMappingOptions();
			options.Conventions.Insert(0, new AssignSelf());

			var mapping = CreateMapping<PocoWithComplexCtor>(options);
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsInstanceOfType(mapping.Bindings[0], typeof(AssignFactory.DefaultBinding<PocoWithComplexCtor>));
		}

		[TestMethod]
		public void CreateCtorBindingForPublicCtor()
		{
			var mapping = CreateMapping<PocoWithPublicCtor>();
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsInstanceOfType(mapping.Bindings[0], typeof(CreateInstanceIfNull<PocoWithPublicCtor>));

			var ctorBinding = (CreateInstanceIfNull<PocoWithPublicCtor>)mapping.Bindings[0];

			var instance = ctorBinding.CreateInstance();
			Assert.IsNotNull(instance);
		}

		[TestMethod]
		public void CreateCtorBindingForPrivateCtor()
		{
			var mapping = CreateMapping<PocoWithPrivateCtor>();
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsInstanceOfType(mapping.Bindings[0], typeof(CreateInstanceIfNull<PocoWithPrivateCtor>));

			var ctorBinding = (CreateInstanceIfNull<PocoWithPrivateCtor>)mapping.Bindings[0];

			var instance = ctorBinding.CreateInstance();
			Assert.IsNotNull(instance);
		}

		private Mapping.Mapping CreateMapping<T>(MappingOptions mappingOptions = null)
		{
			return CreateMapping<T, T>(mappingOptions);
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>(MappingOptions mappingOptions = null)
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var builder = new MappingBuilder(fromPocoModel, toPocoModel);
			if (mappingOptions != null)
			{
				foreach (var convention in mappingOptions.Conventions)
				{
					builder.AddConvention(convention);
				}
			}
			builder.AddConvention(CreateInstanceAsNeeded.Instance);
			return builder.BuildMapping();
		}

		private class PocoWithPublicCtor
		{
			public PocoWithPublicCtor() { }
		}

		private class PocoWithPrivateCtor
		{
			private PocoWithPrivateCtor() { }
		}

		private class PocoWithComplexCtor
		{
			public PocoWithComplexCtor(int value)
			{
			}
		}

		private class AssignSelf : IMappingConvention
		{
			public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
			{
				builder
					.Bind(toModel.GetSelf())
					.AssignUsing<AssignFactory>();
			}
		}

		private class AssignFactory : IAssignmentBindingFactory
		{
			public AssignmentBinding CreateBinding<TTo>(IFieldReference toField)
			{
				return new DefaultBinding<TTo>(toField);
			}

			public class DefaultBinding<T> : AssignmentBinding
			{
				public DefaultBinding(IFieldReference to) : base(to)
				{
				}

				public override void AssignBindingValue(IModelReadWriter from, IModelReadWriter to)
				{
					to.WriteField<T>(To, default(T));
				}
			}
		}
	}
}
