using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.Conventions;
using Silk.Data.Modelling.ResourceLoaders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ResourceLoaderTests
	{
		[TestMethod]
		public async Task CustomResourceLoaderWorks()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView(new SubObjectSupport());

			var viewInstance = new View
			{
				Object1Value = 10,
				Object2Value = 50
			};
			var modelInstance = await view.MapToModelAsync(viewInstance)
				.ConfigureAwait(false);

			Assert.IsNotNull(modelInstance);
			Assert.IsNotNull(modelInstance.Object1);
			Assert.IsNotNull(modelInstance.Object2);
			Assert.AreEqual(viewInstance.Object1Value, modelInstance.Object1.Value);
			Assert.AreEqual(viewInstance.Object2Value, modelInstance.Object2.Value);
		}

		[TestMethod]
		public async Task ResourceLoaderSupportsEnumerable()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView(new SubObjectSupport());

			var viewInstances = new View[]
			{
				new View
				{
					Object1Value = 10,
					Object2Value = 50
				},
				new View
				{
					Object1Value = 10,
					Object2Value = 50
				},
				new View
				{
					Object1Value = 15,
					Object2Value = 25
				}
			};
			SubObjectResourceLoader.RunCount = 0;
			var modelInstances = await view.MapToModelAsync(viewInstances);
			Assert.AreEqual(1, SubObjectResourceLoader.RunCount);
			for (var i = 0; i < modelInstances.Length; i++)
			{
				Assert.IsNotNull(modelInstances[i].Object1);
				Assert.IsNotNull(modelInstances[i].Object2);
				Assert.AreEqual(viewInstances[i].Object1Value, modelInstances[i].Object1.Value);
				Assert.AreEqual(viewInstances[i].Object2Value, modelInstances[i].Object2.Value);
			}
		}

		private class SubObject
		{
			public int Value { get; }

			public SubObject(int value)
			{
				Value = value;
			}
		}

		private class Model
		{
			public SubObject Object1 { get; set; }
			public SubObject Object2 { get; set; }
		}

		private class View
		{
			public int Object1Value { get; set; }
			public int Object2Value { get; set; }
		}

		private class SubObjectSupport : ViewConvention
		{
			public override void MakeModelFields(Modelling.Model model, TypedModelField field, ViewDefinition viewDefinition)
			{
				var subObjectLoader = viewDefinition.ResourceLoaders.OfType<SubObjectResourceLoader>().FirstOrDefault();
				if (subObjectLoader == null)
				{
					subObjectLoader = new SubObjectResourceLoader();
					viewDefinition.ResourceLoaders.Add(subObjectLoader);
				}

				var fieldName = field.Name.Replace("Value", "");
				var bindField = model.Fields.First(q => q.Name == fieldName);

				viewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name,
					new SubObjectBinding(new[] { bindField.Name }, new[] { field.Name }))
				{
					DataType = field.DataType
				});
				subObjectLoader.AddField(field.Name);
			}
		}

		private class SubObjectBinding : ModelBinding
		{
			public override BindingDirection Direction => BindingDirection.ViewToModel;

			public SubObjectBinding(string[] modelFieldPath, string[] viewFieldPath)
				: base(modelFieldPath, viewFieldPath)
			{
			}

			public override void WriteToModel(IModelReadWriter modelReadWriter, object value, MappingContext mappingContext)
			{
				base.WriteToModel(modelReadWriter,
					mappingContext.Resources.Retrieve($"subObject:{value}"),
					mappingContext);
			}
		}

		private class SubObjectResourceLoader : IResourceLoader
		{
			private readonly List<string> _fieldNames = new List<string>();

			public static int RunCount { get; set; }

			public void AddField(string fieldName)
			{
				_fieldNames.Add(fieldName);
			}

			public Task LoadResourcesAsync(IEnumerable<IContainer> containers, MappingContext mappingContext)
			{
				RunCount++;
				var builtObjects = new List<int>();
				foreach (var container in containers)
				{
					foreach (var field in container.View.Fields.Where(q => _fieldNames.Contains(q.Name)))
					{
						var value = (int)container.GetValue(new string[] { field.Name });
						if (!builtObjects.Contains(value))
						{
							mappingContext.Resources.Store($"subObject:{value}", new SubObject(value));
							builtObjects.Add(value);
						}
					}
				}
				return Task.CompletedTask;
			}

			public Task LoadResourcesAsync(IEnumerable<IModelReadWriter> modelReadWriters, MappingContext mappingContext)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
