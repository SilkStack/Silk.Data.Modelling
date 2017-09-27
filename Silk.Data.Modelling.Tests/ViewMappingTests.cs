using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ViewMappingTests
	{
		private TypedModel<Model> _genericModel = TypeModeller.GetModelOf<Model>();
		private TypedModel _nonGenericModel => _genericModel;

		[TestMethod]
		public async Task MapDefaultView()
		{
			var view = _nonGenericModel.CreateView();
			var instance = new Model
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
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.Integer)));
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.String)));
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.Object)));
			Assert.AreEqual(instance.Integer, data[nameof(Model.Integer)]);
			Assert.AreEqual(instance.String, data[nameof(Model.String)]);
			Assert.ReferenceEquals(instance.Object, data[nameof(Model.Object)]);
		}

		[TestMethod]
		public async Task MapTypedView()
		{
			var view = _genericModel.CreateTypedView();
			var instance = new Model
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var container = new MemoryContainer(_genericModel, view);
			await view.MapToViewAsync(instance, container)
				.ConfigureAwait(false);
			var data = container.Data;
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.Integer)));
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.String)));
			Assert.AreEqual(true, data.ContainsKey(nameof(Model.Object)));
			Assert.AreEqual(instance.Integer, data[nameof(Model.Integer)]);
			Assert.AreEqual(instance.String, data[nameof(Model.String)]);
			Assert.ReferenceEquals(instance.Object, data[nameof(Model.Object)]);
		}

		[TestMethod]
		public async Task MapTypedViewToObjectInstance()
		{
			var view = _genericModel
				.GetModeller<View>()
				.CreateTypedView();
			var instance = new Model
			{
				Integer = 5,
				String = "Hello World",
				Object = new object(),
				Sub = new SubModel(10)
			};
			var container = new View();
			await view.MapToViewAsync(instance, container)
				.ConfigureAwait(false);
			Assert.AreEqual(instance.Integer, container.Integer);
			Assert.AreEqual(instance.String, container.String);
			Assert.ReferenceEquals(instance.Object, container.Object);
			Assert.AreEqual(instance.Sub.Integer, container.SubInteger);
		}

		[TestMethod]
		public async Task MapTypedViewToObjectInstanceWithNullSubModel()
		{
			var view = _genericModel
				.GetModeller<View>()
				.CreateTypedView();
			var instance = new Model
			{
				Integer = 5,
				String = "Hello World",
				Object = new object()
			};
			var container = new View();
			await view.MapToViewAsync(instance, container)
				.ConfigureAwait(false);
			Assert.AreEqual(instance.Integer, container.Integer);
			Assert.AreEqual(instance.String, container.String);
			Assert.ReferenceEquals(instance.Object, container.Object);
		}

		[TestMethod]
		public async Task MapTypedViewToObject()
		{
			var view = _genericModel
				.GetModeller<View>()
				.CreateTypedView();
			var instance = new Model
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
		public async Task MapTypedViewToObjectArray()
		{
			var view = _genericModel
				.GetModeller<View>()
				.CreateTypedView();
			var instances = new Model[]
			{
				new Model
				{
					Integer = 5,
					String = "Hello World",
					Object = new object()
				},
				new Model
				{
					Integer = 15,
					String = "Foo",
					Object = new object()
				},
				new Model
				{
					Integer = 25,
					String = "Bar",
					Object = new object()
				}
			};
			var views = await view.MapToViewAsync(instances)
				.ConfigureAwait(false);
			Assert.AreEqual(instances.Length, views.Length);
			for (var i = 0; i < instances.Length; i++)
			{
				Assert.AreEqual(instances[i].Integer, views[i].Integer);
				Assert.AreEqual(instances[i].String, views[i].String);
				Assert.ReferenceEquals(instances[i].Object, views[i].Object);
			}
		}

		private class Model
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
			public SubModel Sub { get; set; }
		}

		public class SubModel
		{
			public int Integer { get; }

			public SubModel(int i)
			{
				Integer = i;
			}
		}

		private class View
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
			public int SubInteger { get; set; }
		}
	}
}
