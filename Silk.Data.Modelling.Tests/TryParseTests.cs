using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using Silk.Data.Modelling.Mapping.Binding;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class TryParseTests
	{
		[TestMethod]
		public void MapParsedProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			Assert.AreEqual(2, mapping.Bindings.Length);
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is TryParseBinding<string, int>
				));
			Assert.IsTrue(mapping.Bindings.OfType<MappingBinding>().Any(q => 
				q is TryParseBinding<string, ParseEnum>
				));
		}

		[TestMethod]
		public void MappingParsesProperties()
		{
			var mapping = CreateMapping<SourcePoco, TargetPoco>();
			var source = new SourcePoco
			{
				Source = "1",
				Option = nameof(ParseEnum.Option2)
			};
			var result = new TargetPoco();
			var sourceReader = new ObjectReadWriter(source, TypeModel.GetModelOf<SourcePoco>(), typeof(SourcePoco));
			var targetWriter = new ObjectReadWriter(result, TypeModel.GetModelOf<TargetPoco>(), typeof(TargetPoco));
			mapping.PerformMapping(sourceReader, targetWriter);
			Assert.AreEqual(source.Source, result.Source.ToString());
			Assert.AreEqual(source.Option, result.Option.ToString());
		}

		private Mapping.Mapping CreateMapping<T>()
		{
			return CreateMapping<T, T>();
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>()
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var options = new MappingOptions();
			options.Conventions.Add(CopyTryParse.Instance);
			var builder = new MappingBuilder(fromPocoModel, toPocoModel, options);
			return builder.BuildMapping();
		}

		private class SourcePoco
		{
			public string Source { get; set; }
			public string Option { get;set; }
		}

		private class TargetPoco
		{
			public int Source { get; set; }
			public ParseEnum Option { get; set; }
		}

		private enum ParseEnum
		{
			Option1,
			Option2
		}
	}
}
