using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class MappingTests
	{
		private TypedModel _nonGenericModel = TypeModeller.GetModelOf<SimpleClassWithPublicProperties>();

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
			var readWriter = new ObjectReadWriter(_nonGenericModel)
			{
				Instance = instance
			};
			var container = new MemoryContainer(_nonGenericModel, view);
			await view.MapToViewAsync(readWriter, container)
				.ConfigureAwait(false);
			var data = container.Data;
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Integer)));
			Assert.AreEqual(true, data.ContainsKey(nameof(SimpleClassWithPublicProperties.String)));
			Assert.AreEqual(false, data.ContainsKey(nameof(SimpleClassWithPublicProperties.Object)));
			Assert.AreEqual(instance.Integer, data[nameof(SimpleClassWithPublicProperties.Integer)]);
			Assert.AreEqual(instance.String, data[nameof(SimpleClassWithPublicProperties.String)]);
		}

		private class SimpleClassWithPublicProperties
		{
			public int Integer { get; set; }
			public string String { get; set; }
			public object Object { get; set; }
		}
	}
}
