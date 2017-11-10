using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class EnumerableTests
	{
		[TestMethod]
		public void EnumerableTypesAreModelled()
		{
			var model = TypeModeller.GetModelOf<Model>();

			foreach (var field in model.Fields)
			{
				Assert.IsTrue(field.IsEnumerable);
				Assert.AreEqual(typeof(int), field.DataType);
				switch (field.Name)
				{
					case nameof(Model.List):
						Assert.AreEqual(typeof(List<int>), field.EnumerableType);
						break;
					case nameof(Model.ROCollection):
						Assert.AreEqual(typeof(IReadOnlyCollection<int>), field.EnumerableType);
						break;
					case nameof(Model.Array):
						Assert.AreEqual(typeof(int[]), field.EnumerableType);
						break;
				}
			}
		}

		[TestMethod]
		public async Task EnumerablesAreMappedToViews()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstance = new Model
			{
				List = new List<int>()
				{
					1, 2, 3, 4, 5
				},
				ROCollection = new List<int>()
				{
					6, 7, 8, 9, 10
				},
				Array = new []
				{
					11, 12, 13, 14, 15
				}
			};
			var viewInstance = await view.MapToViewAsync(modelInstance);

			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.List);
			Assert.IsNotNull(viewInstance.ROCollection);
			Assert.IsNotNull(viewInstance.Array);
			Assert.IsTrue(modelInstance.List.SequenceEqual(viewInstance.List));
			Assert.IsTrue(modelInstance.ROCollection.SequenceEqual(viewInstance.ROCollection));
			Assert.IsTrue(modelInstance.Array.SequenceEqual(viewInstance.Array));
		}

		[TestMethod]
		public async Task EnumerablesAreMappedToModels()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var viewInstance = new View
			{
				List = new int[]
				{
					1, 2, 3, 4, 5
				},
				ROCollection = new List<int>()
				{
					6, 7, 8, 9, 10
				},
				Array = new List<int>
				{
					11, 12, 13, 14, 15
				}
			};
			var modelInstance = await view.MapToModelAsync(viewInstance);

			Assert.IsNotNull(modelInstance);
			Assert.IsNotNull(modelInstance.List);
			Assert.IsNotNull(modelInstance.ROCollection);
			Assert.IsNotNull(modelInstance.Array);
			Assert.IsTrue(viewInstance.List.SequenceEqual(modelInstance.List));
			Assert.IsTrue(viewInstance.ROCollection.SequenceEqual(modelInstance.ROCollection));
			Assert.IsTrue(viewInstance.Array.SequenceEqual(modelInstance.Array));
		}

		[TestMethod]
		public async Task EnumerablesOfSingleElements()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstance = new Model
			{
				List = new List<int>()
				{
					1
				},
				ROCollection = new List<int>()
				{
					6
				},
				Array = new[]
				{
					11
				}
			};
			var viewInstance = await view.MapToViewAsync(modelInstance);

			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.List);
			Assert.IsNotNull(viewInstance.ROCollection);
			Assert.IsNotNull(viewInstance.Array);
			Assert.IsTrue(modelInstance.List.SequenceEqual(viewInstance.List));
			Assert.IsTrue(modelInstance.ROCollection.SequenceEqual(viewInstance.ROCollection));
			Assert.IsTrue(modelInstance.Array.SequenceEqual(viewInstance.Array));
		}

		[TestMethod]
		public async Task EmptyEnumerablesAreEmpty()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstance = new Model
			{
				List = new List<int>(),
				ROCollection = new List<int>(),
				Array = new int[0]
			};
			var viewInstance = await view.MapToViewAsync(modelInstance);

			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.List);
			Assert.IsNotNull(viewInstance.ROCollection);
			Assert.IsNotNull(viewInstance.Array);
			Assert.IsTrue(modelInstance.List.SequenceEqual(viewInstance.List));
			Assert.IsTrue(modelInstance.ROCollection.SequenceEqual(viewInstance.ROCollection));
			Assert.IsTrue(modelInstance.Array.SequenceEqual(viewInstance.Array));
		}

		[TestMethod]
		public async Task NullEnumerablesResultInNull()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstance = new Model
			{
				List = null,
				ROCollection = null,
				Array = null
			};
			var viewInstance = await view.MapToViewAsync(modelInstance);

			Assert.IsNotNull(viewInstance);
			Assert.IsNull(viewInstance.List);
			Assert.IsNull(viewInstance.ROCollection);
			Assert.IsNull(viewInstance.Array);
		}

		private class Model
		{
			public List<int> List { get; set; } = new List<int>();
			public IReadOnlyCollection<int> ROCollection { get; set; }
			public int[] Array { get; set; }
		}

		private class View
		{
			public int[] List { get; set; }
			public IEnumerable<int> ROCollection { get; set; }
			public IList<int> Array { get; set; }
		}
	}
}
