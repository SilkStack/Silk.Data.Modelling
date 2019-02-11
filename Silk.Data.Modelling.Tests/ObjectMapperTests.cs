using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ObjectMapperTests
	{
		[TestMethod]
		public void MapperWorksWithDefaults()
		{
			var mapper = new ObjectMapper();
			var source = new SourceModel();
			var target = mapper.Map<SourceModel, TargetModel>(source);
			Assert.IsNotNull(target);
			Assert.AreEqual(source.Id.ToString(), target.Id);
		}

		private class SourceModel
		{
			public Guid Id { get; } = Guid.NewGuid();
		}

		private class TargetModel
		{
			public string Id { get; set; }
		}
	}
}
