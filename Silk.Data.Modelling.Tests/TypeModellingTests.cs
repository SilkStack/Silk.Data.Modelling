using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class TypeModellingTests
	{
		[TestMethod]
		public void CanCreateTypeModel()
		{
			var model = TypeModel.GetModelOf<EmptyClass>();
			Assert.IsNotNull(model);
		}

		[TestMethod]
		public void ModelBasicProperties()
		{
			var model = TypeModel.GetModelOf<ClassWithProperties>();

			Assert.AreEqual(3, model.Fields.Length);
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.ReadWriteProperty)
				&& field.CanRead && field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.ReadOnlyProperty)
				&& field.CanRead && !field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.SetOnlyProperty)
				&& !field.CanRead && field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
		}

		[TestMethod]
		public void ModelInheritedProperties()
		{
			var model = TypeModel.GetModelOf<ClassWithInheritedProperties>();

			Assert.AreEqual(3, model.Fields.Length);
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.ReadWriteProperty)
				&& field.CanRead && field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.ReadOnlyProperty)
				&& field.CanRead && !field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithProperties.SetOnlyProperty)
				&& !field.CanRead && field.CanWrite && !field.IsEnumerable && field.FieldType == typeof(int)));
		}

		[TestMethod]
		public void ModelPrivateSetterPropertiesAsWritable()
		{
			var model = TypeModel.GetModelOf<ClassWithPrivateSetters>();

			Assert.AreEqual(1, model.Fields.Length);
			Assert.IsTrue(model.Fields[0].CanRead);
			Assert.IsTrue(model.Fields[0].CanWrite);
		}

		[TestMethod]
		public void ModelProtectedSetterPropertiesAsWritable()
		{
			var model = TypeModel.GetModelOf<ClassWithProtectedSetters>();

			Assert.AreEqual(1, model.Fields.Length);
			Assert.IsTrue(model.Fields[0].CanRead);
			Assert.IsTrue(model.Fields[0].CanWrite);
		}

		[TestMethod]
		public void ModelEnumerableProperties()
		{
			var model = TypeModel.GetModelOf<ClassWithEnumerables>();

			Assert.AreEqual(4, model.Fields.Length);
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithEnumerables.Array)
				&& field.CanRead && !field.CanWrite && field.IsEnumerable && field.ElementType == typeof(int) && field.FieldType == typeof(int[])));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithEnumerables.List)
				&& field.CanRead && !field.CanWrite && field.IsEnumerable && field.ElementType == typeof(int) && field.FieldType == typeof(List<int>)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithEnumerables.Collection)
				&& field.CanRead && !field.CanWrite && field.IsEnumerable && field.ElementType == typeof(int) && field.FieldType == typeof(ICollection<int>)));
			Assert.IsTrue(model.Fields.Any(field => field.FieldName == nameof(ClassWithEnumerables.Enumerable)
				&& field.CanRead && !field.CanWrite && field.IsEnumerable && field.ElementType == typeof(int) && field.FieldType == typeof(IEnumerable<int>)));
		}

		[TestMethod]
		public void ModelRecursiveClasses()
		{
			var model = TypeModel.GetModelOf<RecursiveClass>();

			Assert.AreEqual(1, model.Fields.Length);
			Assert.ReferenceEquals(model, model.Fields[0].FieldTypeModel);
		}

		private class EmptyClass { }

		private class ClassWithProperties
		{
			private int _uselessBackingField;

			public int ReadWriteProperty { get; set; }
			public int ReadOnlyProperty { get; }
			public int SetOnlyProperty { set => _uselessBackingField = value; }
		}

		private class ClassWithInheritedProperties : ClassWithProperties
		{
		}

		private class ClassWithPrivateSetters
		{
			public int ReadWriteProperty { get; private set; }
		}

		private class ClassWithProtectedSetters
		{
			public int ReadWriteProperty { get; protected set; }
		}

		private class ClassWithEnumerables
		{
			public int[] Array { get; }
			public List<int> List { get; }
			public ICollection<int> Collection { get; }
			public IEnumerable<int> Enumerable { get; }
		}

		private class RecursiveClass
		{
			public RecursiveClass Member { get; }
		}
	}
}
