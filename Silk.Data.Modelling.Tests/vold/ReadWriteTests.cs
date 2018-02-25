using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ReadWriteTests
	{
		[TestMethod]
		public async Task ReadModelWriteView()
		{
			var model = TypeModeller.GetModelOf<ReadablePoco>();
			var view = model.GetModeller<WriteablePoco>().CreateTypedView();

			var modelIntegerField = model.Fields.FirstOrDefault(q => q.Name == nameof(ReadablePoco.Integer));
			Assert.IsNotNull(modelIntegerField);
			Assert.IsTrue(modelIntegerField.CanRead);
			Assert.IsFalse(modelIntegerField.CanWrite);

			var viewIntegerField = view.Fields.FirstOrDefault(q => q.Name == nameof(WriteablePoco.Integer));
			Assert.IsNotNull(viewIntegerField);
			Assert.AreEqual(Bindings.BindingDirection.ModelToView, viewIntegerField.ModelBinding.Direction);

			var modelInstance = new ReadablePoco(5);
			var viewInstance = await view.MapToViewAsync(modelInstance)
				.ConfigureAwait(false);

			Assert.AreEqual(modelInstance.Integer, viewInstance.BackingValue);
		}

		[TestMethod]
		public async Task ReadViewWriteModel()
		{
			var model = TypeModeller.GetModelOf<WriteablePoco>();
			var view = model.GetModeller<ReadablePoco>().CreateTypedView();

			var modelIntegerField = model.Fields.FirstOrDefault(q => q.Name == nameof(WriteablePoco.Integer));
			Assert.IsNotNull(modelIntegerField);
			Assert.IsTrue(modelIntegerField.CanWrite);
			Assert.IsFalse(modelIntegerField.CanRead);

			var viewIntegerField = view.Fields.FirstOrDefault(q => q.Name == nameof(ReadablePoco.Integer));
			Assert.IsNotNull(viewIntegerField);
			Assert.AreEqual(Bindings.BindingDirection.ViewToModel, viewIntegerField.ModelBinding.Direction);

			var viewInstance = new ReadablePoco(5);
			var modelInstance = await view.MapToModelAsync(viewInstance)
				.ConfigureAwait(false);

			Assert.AreEqual(viewInstance.Integer, modelInstance.BackingValue);
		}

		private class ReadablePoco
		{
			public int Integer { get; }

			public ReadablePoco(int value)
			{
				Integer = value;
			}
		}

		private class WriteablePoco
		{
			public int BackingValue { get; private set; }
			public int Integer
			{
				set
				{
					BackingValue = value;
				}
			}
		}
	}
}
