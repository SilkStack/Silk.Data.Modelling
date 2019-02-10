using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Mapping.Binding
{
	[TestClass]
	public class CopySameValueTypesFactoryTests
	{
		private readonly static TypeModel<TestModel> _testModel = TypeModel.GetModelOf<TestModel>();

		private readonly static PropertyInfoField _propertyField = _testModel.Fields.First(q => q.FieldName == nameof(TestModel.Property));
		private readonly static FieldPath<TypeModel, PropertyInfoField> _propertyFieldPath =
			new FieldPath<TypeModel, PropertyInfoField>(_testModel, _propertyField, new[] { _propertyField });

		private readonly static PropertyInfoField _containerField = _testModel.Fields.First(q => q.FieldName == nameof(TestModel.Container));
		private readonly static FieldPath<TypeModel, PropertyInfoField> _containerFieldPath =
			new FieldPath<TypeModel, PropertyInfoField>(_testModel, _containerField, new[] { _containerField });

		private readonly static PropertyInfoField _subPropertyField = _testModel.GetPathFields(_containerFieldPath)
			.First(q => q.FieldName == nameof(SubModel.SubProperty));
		private readonly static FieldPath<TypeModel, PropertyInfoField> _subPropertyFieldPath = _containerFieldPath.Child(_subPropertyField);

		[TestMethod]
		public void CreateBinding_Ignores_Mismatched_Types()
		{
			var factory = new CopySameValueTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var factoryContext = new MappingFactoryContext<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				null, null, null
				);
			factory.CreateBinding(
				factoryContext,
				IntersectedFields<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>.Create(
					_propertyField, _subPropertyField, _propertyFieldPath, _subPropertyFieldPath, null
					)
				);
			Assert.AreEqual(0, factoryContext.Bindings.Count);
		}

		[TestMethod]
		public void CreateBinding_Binds_Matched_Types()
		{
			var factory = new CopySameValueTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var factoryContext = new MappingFactoryContext<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				null, null, null
				);
			factory.CreateBinding(
				factoryContext,
				IntersectedFields<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>.Create(
					_propertyField, _propertyField, _propertyFieldPath, _propertyFieldPath, null
					)
				);
			Assert.AreEqual(1, factoryContext.Bindings.Count);
		}

		[TestMethod]
		public void CreateBinding_Binds_Ignores_Reference_Types()
		{
			var factory = new CopySameValueTypesFactory<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>();
			var factoryContext = new MappingFactoryContext<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>(
				null, null, null
				);
			factory.CreateBinding(
				factoryContext,
				IntersectedFields<TypeModel, PropertyInfoField, TypeModel, PropertyInfoField>.Create(
					_containerField, _containerField, _containerFieldPath, _containerFieldPath, null
					)
				);
			Assert.AreEqual(0, factoryContext.Bindings.Count);
		}

		private class TestModel
		{
			public string Property { get; set; }

			public SubModel Container { get; set; }
		}

		private class SubModel
		{
			public int SubProperty { get; set; }
		}
	}
}
