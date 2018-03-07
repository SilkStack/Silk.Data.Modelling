using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class MappingOverridesTests
	{
		[TestMethod]
		public void MappingOverrideUsesCustomCtor()
		{
			var options = new MappingOptions();
			options.Conventions.Add(new UseObjectMappingOverrides());
			options.AddMappingOverride(new ComplexConstructorOverride());

			var mapping = CreateMapping<ComplexConstructorPoco>(options);
			Assert.AreEqual(1, mapping.Bindings.Length);
			Assert.IsInstanceOfType(mapping.Bindings[0], typeof(UseFactory<ComplexConstructorPoco,ComplexConstructorPoco>));

			var source = new ComplexConstructorPoco(1);
			var sourceReadWriter = new ObjectReadWriter(source, mapping.FromModel, typeof(ComplexConstructorPoco));
			var targetReadWriter = new ObjectReadWriter(null, mapping.ToModel, typeof(ComplexConstructorPoco));
			mapping.PerformMapping(sourceReadWriter, targetReadWriter);

			var target = targetReadWriter.ReadField<ComplexConstructorPoco>(new[] { "." }, 0);
			Assert.IsNotNull(target);
			Assert.AreEqual(2, target.Value);
		}

		[TestMethod]
		public void MappingUsesOverrideBinding()
		{
			var options = new MappingOptions();
			options.Conventions.Add(new UseObjectMappingOverrides());
			options.AddMappingOverride(new SimplePocoMappingOverride());

			var mapping = CreateMapping<SimplePoco>(options);
			Assert.AreEqual(2, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<CopyBinding<int>>().Any(binding => binding.FromPath.SequenceEqual(new[] { "PropertyA" }) && binding.ToPath.SequenceEqual(new[] { "PropertyB" })));
			Assert.IsTrue(mapping.Bindings.OfType<CopyBinding<int>>().Any(binding => binding.FromPath.SequenceEqual(new[] { "PropertyB" }) && binding.ToPath.SequenceEqual(new[] { "PropertyA" })));
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
			return builder.BuildMapping();
		}

		private class ComplexConstructorPoco
		{
			public int Value { get; }

			public ComplexConstructorPoco(int value)
			{
				Value = value;
			}
		}

		private class ComplexConstructorOverride : IObjectMappingOverride<ComplexConstructorPoco, ComplexConstructorPoco>
		{
			public void CreateBindings(ObjectMappingBuilder<ComplexConstructorPoco, ComplexConstructorPoco> builder)
			{
				builder.ConstructWithFactory(from => new ComplexConstructorPoco(from.Value + 1));
			}
		}

		private class SimplePoco
		{
			public int PropertyA { get; set; }
			public int PropertyB { get; set; }
		}

		private class SimplePocoMappingOverride : IObjectMappingOverride<SimplePoco, SimplePoco>
		{
			public void CreateBindings(ObjectMappingBuilder<SimplePoco, SimplePoco> builder)
			{
				builder
					.Bind(q => q.PropertyA)
					.From(q => q.PropertyB);
				builder
					.Bind(q => q.PropertyB)
					.From(q => q.PropertyA);
			}
		}
	}
}
