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

		[TestMethod]
		public void MappingSameComplexTypeWorks()
		{
			var mapper = new ObjectMapper();
			var source = new DuplicateComplexMappingSource();
			var target = mapper.Map<DuplicateComplexMappingSource, DuplicateComplexMappingTarget>(source);

			Assert.IsNotNull(target);
			Assert.AreEqual(source.SourceOne.Id.ToString(), target.SourceOne.Id);
			Assert.AreEqual(source.SourceTwo.Id.ToString(), target.SourceTwo.Id);
			Assert.AreEqual(source.SourceThree.Id.ToString(), target.SourceThree.Id);
		}

		private class SourceModel
		{
			public Guid Id { get; } = Guid.NewGuid();
		}

		private class TargetModel
		{
			public string Id { get; set; }
		}

		private class DuplicateComplexMappingSource
		{
			public SourceModel SourceOne { get; } = new SourceModel();
			public SourceModel SourceTwo { get; } = new SourceModel();
			public SourceModel SourceThree { get; } = new SourceModel();
		}

		private class DuplicateComplexMappingTarget
		{
			public TargetModel SourceOne { get; set; }
			public TargetModel SourceTwo { get; set; }
			public TargetModel SourceThree { get; set; }
		}
	}
}
