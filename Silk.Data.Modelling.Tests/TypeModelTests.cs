using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class TypeModelTests
	{
		[TestMethod]
		public void GetModelOf_Can_Create_Models()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			Assert.IsNotNull(typeModel);
		}

		[TestMethod]
		public void GetModelOf_Returns_Cached_Models()
		{
			var firstTypeModel = TypeModel.GetModelOf<SuperType>();
			var secondTypeModel = TypeModel.GetModelOf<SuperType>();
			Assert.ReferenceEquals(firstTypeModel, secondTypeModel);
		}

		[TestMethod]
		public void GetModelOf_Excludes_Static_Properties()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == nameof(SuperType.StaticProperty));
			Assert.IsTrue(testForField == null, "Static field was found on type model.");
		}

		[TestMethod]
		public void GetModelOf_Includes_Private_Properties()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == "PrivateSuperTypeProperty");
			Assert.IsTrue(testForField != null, "Private field was not found on type model.");
		}

		[TestMethod]
		public void GetModelOf_Arrays_Are_Enumerable_Types()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == nameof(SuperType.ArraySuperTypeProperty));
			Assert.IsTrue(testForField.IsEnumerableType, "Array field isn't an enumerable type.");
			Assert.AreEqual(testForField.FieldElementType, typeof(string), "Array field element type isn't valid.");
		}

		[TestMethod]
		public void GetModelOf_Include_ReadOnly_Properties()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == nameof(SuperType.ReadOnlySuperTypeProperty));
			Assert.IsTrue(testForField != null, "Read only field was not found on type model.");
			Assert.IsTrue(testForField.CanRead, "Read only field can't be read.");
			Assert.IsFalse(testForField.CanWrite, "Read only field can be written.");
		}

		[TestMethod]
		public void GetModelOf_Include_WriteOnly_Properties()
		{
			var typeModel = TypeModel.GetModelOf<SuperType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == nameof(SuperType.WriteOnlySuperTypeProperty));
			Assert.IsTrue(testForField != null, "Write only field was not found on type model.");
			Assert.IsFalse(testForField.CanRead, "Write only field can be read.");
			Assert.IsTrue(testForField.CanWrite, "Write only field can't be written.");
		}

		[TestMethod]
		public void GetModelOf_Includes_Inherited_Properties()
		{
			var typeModel = TypeModel.GetModelOf<SubType>();
			var testForField = typeModel.Fields.FirstOrDefault(field => field.FieldName == nameof(SuperType.PublicSuperTypeProperty));
			Assert.IsTrue(testForField != null, "Inherited field was not found on type model.");
		}

		private class SuperType
		{
			public static string StaticProperty { get; set; }

			public string PublicSuperTypeProperty { get; set; }
			private string PrivateSuperTypeProperty { get; set; }

			public string[] ArraySuperTypeProperty { get; set; }

			public string ReadOnlySuperTypeProperty => null;

			public string WriteOnlySuperTypeProperty { set { } }
		}

		private class SubType : SuperType
		{
			public string PublicSubTypeProperty { get; set; }
		}
	}
}
