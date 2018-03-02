using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class CreateInstanceAsNeededTests
	{
		[TestMethod]
		public void ThrowExceptionWithNoUsableCtor()
		{
			var exceptionCaught = false;
			try
			{
				CreateMapping<PocoWithComplexCtor>();
			}
			catch (MappingRequirementException)
			{
				exceptionCaught = true;
			}
			Assert.IsTrue(exceptionCaught);
		}

		private Mapping.Mapping CreateMapping<T>()
		{
			return CreateMapping<T, T>();
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>()
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var builder = new MappingBuilder(fromPocoModel, toPocoModel);
			builder.AddConvention(CreateInstanceAsNeeded.Instance);
			return builder.BuildMapping();
		}

		private class PocoWithComplexCtor
		{
			public PocoWithComplexCtor(int value)
			{
			}
		}
	}
}
