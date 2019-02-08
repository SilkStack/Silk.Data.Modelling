using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using System.Linq;

namespace Silk.Data.Modelling.Tests.Mapping
{
	[TestClass]
	public class ObjectGraphReaderWriterTests
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
		public void Read_Field_Returns_Correct_Value()
		{
			var graph = new TestModel { Property = "Test Value" };
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var testValue = readerWriter.Read<string>(_propertyFieldPath);
			Assert.AreEqual(graph.Property, testValue);
		}

		[TestMethod]
		public void Write_Field_Sets_Correct_Value()
		{
			var graph = new TestModel();
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var testValue = "Test Value";
			readerWriter.Write(_propertyFieldPath, testValue);
			Assert.AreEqual(testValue, graph.Property);
		}

		[TestMethod]
		public void CheckContainer_Returns_False_When_Null()
		{
			var graph = new TestModel();
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var containerCheck = readerWriter.CheckContainer(_containerFieldPath);
			Assert.IsFalse(containerCheck);
		}

		[TestMethod]
		public void CheckContainer_Returns_True_When_Not_Null()
		{
			var graph = new TestModel { Container = new SubModel() };
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var containerCheck = readerWriter.CheckContainer(_containerFieldPath);
			Assert.IsTrue(containerCheck);
		}

		[TestMethod]
		public void CreateContainer_Creates_Instance()
		{
			var graph = new TestModel();
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			readerWriter.CreateContainer(_containerFieldPath);
			Assert.IsNotNull(graph.Container);
		}

		[TestMethod]
		public void CheckPath_Returns_False_When_Null()
		{
			var graph = new TestModel();
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var pathCheck = readerWriter.CheckPath(_subPropertyFieldPath);
			Assert.IsFalse(pathCheck);
		}

		[TestMethod]
		public void CheckPath_Returns_True_When_Not_Null()
		{
			var graph = new TestModel { Container = new SubModel() };
			var readerWriter = new ObjectGraphReaderWriter<TestModel>(graph);
			var pathCheck = readerWriter.CheckPath(_subPropertyFieldPath);
			Assert.IsTrue(pathCheck);
		}

		private class TestModel
		{
			public string Property { get; set; }

			public SubModel Container { get; set; }
		}

		private class SubModel
		{
			public string SubProperty { get; set; }
		}
	}
}
