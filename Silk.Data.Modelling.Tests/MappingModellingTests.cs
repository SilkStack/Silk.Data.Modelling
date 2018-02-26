using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class MappingModellingTests
	{
		private static readonly IModel _model = TypeModel.GetModelOf<SimplePoco>();

		[TestMethod]
		public void CanTransformToSourceModel()
		{
			var sourceModel = _model.TransformToSourceModel();
			Assert.IsNotNull(sourceModel);
			Assert.AreEqual(2, sourceModel.Fields.Length);
			Assert.IsTrue(sourceModel.Fields.Any(q => q.FieldName == nameof(SimplePoco.Id) && q.FieldType == typeof(int) && q.FieldPath.SequenceEqual(new[] { "Id" })));
			Assert.IsTrue(sourceModel.Fields.Any(q => q.FieldName == nameof(SimplePoco.Sub) && q.FieldType == typeof(SubClass) && q.FieldPath.SequenceEqual(new[] { "Sub" })));

			var subClassIdField = sourceModel.GetField("Sub", "Id");
			Assert.IsNotNull(subClassIdField);
			Assert.IsTrue(subClassIdField.FieldPath.SequenceEqual(new[] { "Sub", "Id" }));
			Assert.AreEqual(0, subClassIdField.Fields.Length);
			Assert.AreEqual(typeof(int), subClassIdField.FieldType);
		}

		[TestMethod]
		public void CanTransformToTargetModel()
		{
			var targetModel = _model.TransformToTargetModel();
			Assert.IsNotNull(targetModel);
			Assert.AreEqual(2, targetModel.Fields.Length);
			Assert.IsTrue(targetModel.Fields.Any(q => q.FieldName == nameof(SimplePoco.Id) && q.FieldType == typeof(int) && q.FieldPath.SequenceEqual(new[] { "Id" })));
			Assert.IsTrue(targetModel.Fields.Any(q => q.FieldName == nameof(SimplePoco.Sub) && q.FieldType == typeof(SubClass) && q.FieldPath.SequenceEqual(new[] { "Sub" })));

			var subClassIdField = targetModel.GetField("Sub", "Id");
			Assert.IsNotNull(subClassIdField);
			Assert.IsTrue(subClassIdField.FieldPath.SequenceEqual(new[] { "Sub", "Id" }));
			Assert.AreEqual(0, subClassIdField.Fields.Length);
			Assert.AreEqual(typeof(int), subClassIdField.FieldType);
		}

		private class SimplePoco
		{
			public int Id { get; set; }
			public SubClass Sub { get; set; }
		}

		private class SubClass
		{
			public int Id { get; set; }
		}
	}
}
