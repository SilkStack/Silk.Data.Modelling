using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ObjectMappingTests
	{
		[TestMethod]
		public void ThrowExceptionWithNoUsableCtor()
		{
			throw new System.NotImplementedException();
		}

		[TestMethod]
		public void InjectSameTypes()
		{
			var mapper = new ObjectMapper();
			var output = new SimplePoco();
			var input = new SimplePoco
			{
				Property = 1
			};
			mapper.Inject(input, output);
			Assert.AreEqual(input.Property, output.Property);
		}

		private class SimplePoco
		{
			public int Property { get; set; }
		}
	}
}
