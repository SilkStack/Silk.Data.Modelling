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

			var target = targetReadWriter.ReadField<ComplexConstructorPoco>(TypeModel.GetModelOf<ComplexConstructorPoco>().Root);
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
		}

		[TestMethod]
		public void MapToValueOverride()
		{
			var options = new MappingOptions();
			options.Conventions.Add(new UseObjectMappingOverrides());
			options.AddMappingOverride(new SimplePocoValueOverride());

			var mapper = new ObjectMapper(options);
			var result = new SimplePoco();
			mapper.Inject<SimplePoco, SimplePoco>(null, result);
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.PropertyA);
			Assert.AreEqual(2, result.PropertyB);
		}

		[TestMethod]
		public void MapToDelegateOverride()
		{
			var options = new MappingOptions();
			options.Conventions.Add(new UseObjectMappingOverrides());
			options.AddMappingOverride(new SimplePocoDelegateOverride());

			var mapper = new ObjectMapper(options);
			var result = new SimplePoco();
			mapper.Inject<SimplePoco, SimplePoco>(new SimplePoco { PropertyA = 1, PropertyB = 2 }, result);
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.PropertyA);
			Assert.AreEqual(4, result.PropertyB);
		}

		[TestMethod]
		public void MapPropertiesUsingMappingOverride()
		{
			var options = new MappingOptions();
			options.Conventions.Add(new UseObjectMappingOverrides());
			options.Conventions.Add(MapOverriddenTypes.Instance);
			options.AddMappingOverride<string, StrObj>(new StrToObjOverride());
			options.AddMappingOverride<StrObj, string>(new StrToObjOverride());

			var mapper = new ObjectMapper(options);
			var result = new PocoWithObj();
			mapper.Inject(new PocoWithStr { Property = "Hello World" }, result);
			Assert.IsNotNull(result.Property);
			Assert.AreEqual("Hello World", result.Property.Value);

			var backwardsResult = new PocoWithStr();
			mapper.Inject(result, backwardsResult);
			Assert.AreEqual("Hello World", backwardsResult.Property);
		}

		private Mapping.Mapping CreateMapping<T>(MappingOptions mappingOptions = null)
		{
			return CreateMapping<T, T>(mappingOptions);
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>(MappingOptions mappingOptions = null)
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			if (mappingOptions == null)
				mappingOptions = new MappingOptions();

			var builder = new MappingBuilder(fromPocoModel, toPocoModel, mappingOptions);
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

		private class PocoWithStr
		{
			public string Property { get; set; }
		}

		private class PocoWithObj
		{
			public StrObj Property { get; set; }
		}

		private class StrObj
		{
			public string Value { get; set; }
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

		private class SimplePocoValueOverride : IObjectMappingOverride<SimplePoco, SimplePoco>
		{
			public void CreateBindings(ObjectMappingBuilder<SimplePoco, SimplePoco> builder)
			{
				builder
					.Bind(q => q.PropertyA)
					.ToValue(1);
				builder
					.Bind(q => q.PropertyB)
					.ToValue(2);
			}
		}

		private class SimplePocoDelegateOverride : IObjectMappingOverride<SimplePoco, SimplePoco>
		{
			public void CreateBindings(ObjectMappingBuilder<SimplePoco, SimplePoco> builder)
			{
				builder
					.Bind(q => q.PropertyA)
					.From(q => q.PropertyA, value => value + 1);
				builder
					.Bind(q => q.PropertyB)
					.From(q => q.PropertyB, value => value + 2);
			}
		}

		private class StrToObjOverride : IObjectMappingOverride<string, StrObj>, IObjectMappingOverride<StrObj, string>
		{
			public void CreateBindings(ObjectMappingBuilder<string, StrObj> builder)
			{
				builder.ConstructWithFactory(from => new StrObj { Value = from });
			}

			public void CreateBindings(ObjectMappingBuilder<StrObj, string> builder)
			{
				builder.ConstructWithFactory(from => from.Value);
			}
		}
	}
}
