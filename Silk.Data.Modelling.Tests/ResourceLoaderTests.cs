using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.Conventions;
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

				viewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name, new SubObjectBinding(bindField))
				{
					DataType = field.DataType
				});
				subObjectLoader.AddField(field.Name);
			}
		}

		private class SubObjectBinding : IModelBinding
		{
			private readonly TypedModelField _bindField;

			public BindingDirection Direction => BindingDirection.ViewToModel;

			public SubObjectBinding(TypedModelField bindField)
			{
				_bindField = bindField;
			}

			public object ReadFromModel(IModelReadWriter modelReadWriter)
			{
				throw new System.NotSupportedException();
			}

			public void WriteToModel(IModelReadWriter modelReadWriter, object value, MappingContext mappingContext)
			{
				modelReadWriter = modelReadWriter.GetField(_bindField);
				modelReadWriter.Value = mappingContext.Resources.Retrieve($"subObject:{value}");
			}
		}

		private class SubObjectResourceLoader : IResourceLoader
		{
			private readonly List<string> _fieldNames = new List<string>();

			public void AddField(string fieldName)
			{
				_fieldNames.Add(fieldName);
			}

			public Task LoadResourcesAsync(IEnumerable<IContainer> containers, MappingContext mappingContext)
			{
				foreach (var container in containers)
				{
					foreach (var field in container.View.Fields.Where(q => _fieldNames.Contains(q.Name)))
					{
						var value = (int)container.GetValue(field);
						mappingContext.Resources.Store($"subObject:{value}", new SubObject(value));
					}
				}
				return Task.CompletedTask;
			}
		}
	}
}
