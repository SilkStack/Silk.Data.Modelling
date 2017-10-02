using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class SubMappingTests
	{
		[TestMethod]
		public async Task MapSubModelToSubView()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstance = new Model
			{
				Item1 = new SubModel
				{
					Value = 5
				},
				Item2 = new SubModel
				{
					Value = 10
				}
			};
			var viewInstance = await view.MapToViewAsync(modelInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.Item1);
			Assert.IsNotNull(viewInstance.Item2);
			Assert.AreEqual(modelInstance.Item1.Value, viewInstance.Item1.Value);
			Assert.AreEqual(modelInstance.Item2.Value, viewInstance.Item2.Value);
		}

		[TestMethod]
		public async Task MapSubModelToSubViewArray()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var modelInstances = new Model[]
			{
				new Model
				{
					Item1 = new SubModel
					{
						Value = 5
					},
					Item2 = new SubModel
					{
						Value = 10
					}
				},
				new Model
				{
					Item1 = new SubModel
					{
						Value = 15
					},
					Item2 = new SubModel
					{
						Value = 110
					}
				}
			};
			var viewInstances = await view.MapToViewAsync(modelInstances)
				.ConfigureAwait(false);
			Assert.IsNotNull(viewInstances);
			Assert.AreEqual(modelInstances.Length, viewInstances.Length);
			for (var i = 0; i < modelInstances.Length; i++)
			{
				Assert.IsNotNull(viewInstances[i].Item1);
				Assert.IsNotNull(viewInstances[i].Item2);
				Assert.AreEqual(modelInstances[i].Item1.Value, viewInstances[i].Item1.Value);
				Assert.AreEqual(modelInstances[i].Item2.Value, viewInstances[i].Item2.Value);
			}
		}

		[TestMethod]
		public async Task MapSubViewToSubModel()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var viewInstance = new View
			{
				Item1 = new SubView
				{
					Value = 5
				},
				Item2 = new SubView
				{
					Value = 10
				}
			};
			var modelInstance = await view.MapToModelAsync(viewInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(modelInstance);
			Assert.IsNotNull(modelInstance.Item1);
			Assert.IsNotNull(modelInstance.Item2);
			Assert.AreEqual(viewInstance.Item1.Value, modelInstance.Item1.Value);
			Assert.AreEqual(viewInstance.Item2.Value, modelInstance.Item2.Value);
		}

		[TestMethod]
		public async Task MapSubViewToSubModelArray()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView();
			var viewInstances = new View[]
			{
				new View
				{
					Item1 = new SubView
					{
						Value = 5
					},
					Item2 = new SubView
					{
						Value = 10
					}
				},
				new View
				{
					Item1 = new SubView
					{
						Value = 15
					},
					Item2 = new SubView
					{
						Value = 110
					}
				}
			};
			var modelInstances = await view.MapToModelAsync(viewInstances)
				.ConfigureAwait(false);
			Assert.IsNotNull(modelInstances);
			Assert.AreEqual(viewInstances.Length, modelInstances.Length);
			for (var i = 0; i < viewInstances.Length; i++)
			{
				Assert.IsNotNull(modelInstances[i]);
				Assert.IsNotNull(modelInstances[i].Item1);
				Assert.IsNotNull(modelInstances[i].Item2);
				Assert.AreEqual(viewInstances[i].Item1.Value, modelInstances[i].Item1.Value);
				Assert.AreEqual(viewInstances[i].Item2.Value, modelInstances[i].Item2.Value);
			}
		}

		[TestMethod]
		public async Task MapModelWithArrayOfSubModels()
		{
			var model = TypeModeller.GetModelOf<ModelWithArray>();
			var view = model.GetModeller<ViewWithArray>().CreateTypedView();

			var modelInstance = new ModelWithArray
			{
				Items = new[]
				{
					new SubModel
					{
						Value = 1
					},
					new SubModel
					{
						Value = 2
					},
					new SubModel
					{
						Value = 3
					},
					new SubModel
					{
						Value = 4
					},
					new SubModel
					{
						Value = 5
					}
				}
			};
			var viewInstance = await view.MapToViewAsync(modelInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.Items);
			Assert.AreEqual(modelInstance.Items.Length, viewInstance.Items.Length);
			for(var i = 0; i < modelInstance.Items.Length; i++)
			{
				Assert.AreEqual(modelInstance.Items[i].Value, viewInstance.Items[i].Value);
			}
		}

		[TestMethod]
		public async Task MapViewWithArrayOfSubViews()
		{
			var model = TypeModeller.GetModelOf<ModelWithArray>();
			var view = model.GetModeller<ViewWithArray>().CreateTypedView();

			var viewInstance = new ViewWithArray
			{
				Items = new[]
				{
					new SubView
					{
						Value = 1
					},
					new SubView
					{
						Value = 2
					},
					new SubView
					{
						Value = 3
					},
					new SubView
					{
						Value = 4
					},
					new SubView
					{
						Value = 5
					}
				}
			};
			var modelInstance = await view.MapToModelAsync(viewInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(modelInstance);
			Assert.IsNotNull(modelInstance.Items);
			Assert.AreEqual(viewInstance.Items.Length, modelInstance.Items.Length);
			for (var i = 0; i < viewInstance.Items.Length; i++)
			{
				Assert.AreEqual(viewInstance.Items[i].Value, modelInstance.Items[i].Value);
			}
		}

		[TestMethod]
		public async Task SupportMultipleSubMappings()
		{
			var model = TypeModeller.GetModelOf<ModelWith2SubModels>();
			var view = model.GetModeller<ViewWith2SubViews>().CreateTypedView();

			var modelInstance = new ModelWith2SubModels
			{
				Item1 = new SubModel { Value = 1 },
				Item2 = new SubModel { Value = 2 }
			};
			var viewInstance = await view.MapToViewAsync(modelInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.Item1);
			Assert.IsNotNull(viewInstance.Item2);
			Assert.AreEqual(modelInstance.Item1.Value, viewInstance.Item1.Value);
			Assert.AreEqual(modelInstance.Item2.Value, viewInstance.Item2.Value);
		}

		private class Model
		{
			public SubModel Item1 { get; set; }
			public SubModel Item2 { get; set; }
		}

		private class ModelWithArray
		{
			public SubModel[] Items { get; set; }
		}

		private class ModelWith2SubModels
		{
			public SubModel Item1 { get; set; }
			public SubModel Item2 { get; set; }
		}

		private class SubModel
		{
			public int Value { get; set; }
		}

		private class View
		{
			public SubView Item1 { get; set; }
			public SubView Item2 { get; set; }
		}

		private class ViewWithArray
		{
			public SubView[] Items { get; set; }
		}

		private class ViewWith2SubViews
		{
			public SubView Item1 { get; set; }
			public SubView2 Item2 { get; set; }
		}

		private class SubView
		{
			public int Value { get; set; }
		}

		private class SubView2
		{
			public int Value { get; set; }
		}
	}
}
