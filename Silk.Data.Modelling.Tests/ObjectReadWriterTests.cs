using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ObjectReadWriterTests
	{
		private IModel _objectModel = TypeModel.GetModelOf<PocoClass>();

		[TestMethod]
		public void ReadProperty()
		{
			var readWriter = new ObjectReadWriter(new PocoClass { Property = 1 }, _objectModel, typeof(PocoClass));
			Assert.AreEqual(1, readWriter.ReadField<int>(new[] { nameof(PocoClass.Property) }, 0));
		}

		[TestMethod]
		public void WriteProperty()
		{
			var instance = new PocoClass { Property = 1 };
			var readWriter = new ObjectReadWriter(instance, _objectModel, typeof(PocoClass));
			readWriter.WriteField(new[] { nameof(PocoClass.Property) }, 0, 2);
			Assert.AreEqual(2, instance.Property);
		}

		[TestMethod]
		public void ReadDeepProperty()
		{
			var readWriter = new ObjectReadWriter(new PocoClass { Sub = new SubPocoClass { Property = 1 } }, _objectModel, typeof(PocoClass));
			Assert.AreEqual(1, readWriter.ReadField<int>(new[] { nameof(PocoClass.Sub), nameof(SubPocoClass.Property) }, 0));
		}

		[TestMethod]
		public void WriteDeepProperty()
		{
			var instance = new PocoClass { Sub = new SubPocoClass { Property = 1 } };
			var readWriter = new ObjectReadWriter(instance, _objectModel, typeof(PocoClass));
			readWriter.WriteField(new[] { nameof(PocoClass.Sub), nameof(SubPocoClass.Property) }, 0, 2);
			Assert.AreEqual(2, instance.Sub.Property);
		}

		[TestMethod]
		public void ReadDefaultWhenPropertyNull()
		{
			var readWriter = new ObjectReadWriter(null, _objectModel, typeof(PocoClass));
			Assert.AreEqual(0, readWriter.ReadField<int>(new[] { nameof(PocoClass.Property) }, 0));
		}

		[TestMethod]
		public void IgnoreWriteWhenPropertyNull()
		{
			var readWriter = new ObjectReadWriter(null, _objectModel, typeof(PocoClass));
			readWriter.WriteField(new[] { nameof(PocoClass.Sub), nameof(SubPocoClass.Property) }, 0, 2);
		}

		private class PocoClass
		{
			public int Property { get; set; }
			public SubPocoClass Sub { get; set; }
		}

		private class SubPocoClass
		{
			public int Property { get; set; }
		}
	}
}
