using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ViewModellingTests
	{
		private TypedModel<SourceModel> _model = TypeModeller.GetModelOf<SourceModel>();

		[TestMethod]
		public void CreateUntypedView()
		{
			var view = _model.CreateView();
			Assert.IsNotNull(view);
			Assert.AreEqual(_model.Name, view.Name);
			Assert.AreEqual(1, view.Fields.Length);

			var idField = view.Fields[0];
			Assert.AreEqual(nameof(SourceModel.Id), idField.Name);
			Assert.AreEqual(typeof(int), idField.DataType);
		}

		[TestMethod]
		public void CreateTypedView()
		{
			var view = _model.CreateView(viewDefinition => new TypedDefaultView<SourceModel>(viewDefinition.Name, null));
		}

		[TestMethod]
		public void CreateTypedViewWithViewType()
		{
			var view = _model.CreateView(viewDefinition => new TypedDefaultView<SourceModel, ViewModel>(viewDefinition.Name, null));
		}

		private class SourceModel
		{

			public int Id { get; set; }
		}

		private class ViewModel
		{
			public int Id { get; set; }
		}
	}
}
