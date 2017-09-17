using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ModelMappingTests
	{
		private TypedModel<SimpleClassWithPublicProperties> _genericModel = TypeModeller.GetModelOf<SimpleClassWithPublicProperties>();
		private TypedModel _nonGenericModel => _genericModel;

		[TestMethod]
		public async Task MapDefaultViewToModel()
		{
			var view = _nonGenericModel.CreateView();
			var container = new MemoryContainer(_nonGenericModel, view);
			container.Data[nameof(SimpleClassWithPublicProperties.Integer)] = 5;
			container.Data[nameof(SimpleClassWithPublicProperties.String)] = "Hello World";
			container.Data[nameof(SimpleClassWithPublicProperties.Object)] = new object();

			var instance = new SimpleClassWithPublicProperties();
			var readWriter = new ObjectReadWriter(_nonGenericModel, instance);

			await view.MapToModelAsync(readWriter, container)
				.ConfigureAwait(false);

			Assert.AreEqual(container.Data[nameof(SimpleClassWithPublicProperties.Integer)], instance.Integer);
			Assert.AreEqual(container.Data[nameof(SimpleClassWithPublicProperties.String)], instance.String);
			Assert.ReferenceEquals(container.Data[nameof(SimpleClassWithPublicProperties.Object)], instance.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToModel()
		{
			var view = _genericModel.CreateTypedView();
			var container = new MemoryContainer(_nonGenericModel, view);
			container.Data[nameof(SimpleClassWithPublicProperties.Integer)] = 5;
			container.Data[nameof(SimpleClassWithPublicProperties.String)] = "Hello World";
			container.Data[nameof(SimpleClassWithPublicProperties.Object)] = new object();
			var instance = new SimpleClassWithPublicProperties();

			await view.MapToModelAsync(instance, container)
				.ConfigureAwait(false);
			Assert.AreEqual(container.Data[nameof(SimpleClassWithPublicProperties.Integer)], instance.Integer);
			Assert.AreEqual(container.Data[nameof(SimpleClassWithPublicProperties.String)], instance.String);
			Assert.ReferenceEquals(container.Data[nameof(SimpleClassWithPublicProperties.Object)], instance.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToModelObjectInstance()
		{
			var view = _genericModel
				.GetModeller<SimpleMappedClass>()
				.CreateTypedView();
			var container = new SimpleMappedClass
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var instance = new SimpleClassWithPublicProperties();
			await view.MapToModelAsync(instance, container)
				.ConfigureAwait(false);
			Assert.AreEqual(container.Integer, instance.Integer);
			Assert.AreEqual(container.String, instance.String);
			Assert.ReferenceEquals(container.Object, instance.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToModelObject()
		{
			var view = _genericModel
				.GetModeller<SimpleMappedClass>()
				.CreateTypedView();
			var container = new SimpleMappedClass
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var instance = await view.MapToModelAsync(container)
				.ConfigureAwait(false);
			Assert.AreEqual(container.Integer, instance.Integer);
			Assert.AreEqual(container.String, instance.String);
			Assert.ReferenceEquals(container.Object, instance.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToModelObjectArray()
		{
			var view = _genericModel
				.GetModeller<SimpleMappedClass>()
				.CreateTypedView();
			var containers = new SimpleMappedClass[]
			{
				new SimpleMappedClass
				{
					Integer = 5,
					String = "Hello World",
					Object = new object()
				},
				new SimpleMappedClass
				{
					Integer = 15,
					String = "Foo",
					Object = new object()
				},
				new SimpleMappedClass
				{
					Integer = 25,
					String = "Bar",
					Object = new object()
				}
			};
			var instances = await view.MapToModelAsync(containers)
				.ConfigureAwait(false);
			Assert.AreEqual(containers.Length, instances.Length);
			for (var i = 0; i < containers.Length; i++)
			{
				Assert.AreEqual(containers[i].Integer, instances[i].Integer);
				Assert.AreEqual(containers[i].String, instances[i].String);
				Assert.ReferenceEquals(containers[i].Object, instances[i].Object);
			}
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
