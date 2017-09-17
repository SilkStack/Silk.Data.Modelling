using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ViewMappingTests
	{
		private TypedModel<SimpleClassWithPublicProperties> _genericModel = TypeModeller.GetModelOf<SimpleClassWithPublicProperties>();
		private TypedModel _nonGenericModel => _genericModel;

		[TestMethod]
		public async Task MapDefaultView()
		{
			var view = _nonGenericModel.CreateView();
			var instance = new SimpleClassWithPublicProperties
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var readWriter = new ObjectReadWriter(_nonGenericModel, instance);
			var container = new MemoryContainer(_nonGenericModel, view);
			await view.MapToViewAsync(readWriter, container)
				.ConfigureAwait(false);
			var data = container.Data;
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Integer)));
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.String)));
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Object)));
			Assert.AreEqual(instance.Integer, data[nameof(SimpleClassWithPublicProperties.Integer)]);
			Assert.AreEqual(instance.String, data[nameof(SimpleClassWithPublicProperties.String)]);
			Assert.ReferenceEquals(instance.Object, data[nameof(SimpleClassWithPublicProperties.Object)]);
		}

		[TestMethod]
		public async Task MapTypedView()
		{
			var view = _genericModel.CreateTypedView();
			var instance = new SimpleClassWithPublicProperties
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var container = new MemoryContainer(_genericModel, view);
			await view.MapToViewAsync(instance, container)
				.ConfigureAwait(false);
			var data = container.Data;
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Integer)));
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.String)));
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Object)));
			Assert.AreEqual(instance.Integer, data[nameof(SimpleClassWithPublicProperties.Integer)]);
			Assert.AreEqual(instance.String, data[nameof(SimpleClassWithPublicProperties.String)]);
			Assert.ReferenceEquals(instance.Object, data[nameof(SimpleClassWithPublicProperties.Object)]);
		}

		[TestMethod]
		public async Task MapTypedViewToObject()
		{
			var view = _genericModel
				.GetModeller<SimpleMappedClass>()
				.CreateTypedView();
			var instance = new SimpleClassWithPublicProperties
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var container = await view.MapToViewAsync(instance)
				.ConfigureAwait(false);
			Assert.AreEqual(instance.Integer, container.Integer);
			Assert.AreEqual(instance.String, container.String);
			Assert.ReferenceEquals(instance.Object, container.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToObjectInstance()
		{
			var view = _genericModel
				.GetModeller<SimpleMappedClass>()
				.CreateTypedView();
			var instance = new SimpleClassWithPublicProperties
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var container = new SimpleMappedClass();
			await view.MapToViewAsync(instance, container)
				.ConfigureAwait(false);
			Assert.AreEqual(instance.Integer, container.Integer);
			Assert.AreEqual(instance.String, container.String);
			Assert.ReferenceEquals(instance.Object, container.Object);
		}

		private class SimpleClassWithPublicProperties
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
		}

		private class SimpleMappedClass
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
		}
	}
}
