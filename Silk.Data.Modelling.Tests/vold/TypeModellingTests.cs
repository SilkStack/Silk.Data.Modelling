using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class TypeModellingTests
	{
		[TestMethod]
		public void PublicGetterSetterModelled()
		{
			var model = TypeModeller.GetModelOf<SimpleClassWithPublicProperties>();
			Assert.IsNotNull(model);
			Assert.AreEqual(nameof(SimpleClassWithPublicProperties), model.Name);
			Assert.IsNotNull(model.Fields);
			Assert.AreEqual(3, model.Fields.Length);
			foreach (var field in model.Fields)
			{
				Assert.AreEqual(true, field.CanRead);
				Assert.AreEqual(true, field.CanWrite);
				Assert.ReferenceEquals(model, field.ParentModel);
				switch (field.Name)
				{
					case nameof(SimpleClassWithPublicProperties.Integer):
						Assert.AreEqual(typeof(int), field.DataType);
						break;
					case nameof(SimpleClassWithPublicProperties.String):
						Assert.AreEqual(typeof(string), field.DataType);
						break;
					case nameof(SimpleClassWithPublicProperties.Object):
						Assert.AreEqual(typeof(object), field.DataType);
						break;
				}
			}
		}

		private class SimpleClassWithPublicProperties
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
		}
	}
}
