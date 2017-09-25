using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Conventions;
using System;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ConventionsTests
	{
		[TestMethod]
		public void CopySimpleTypesConvention()
		{
			var model = TypeModeller.GetModelOf<SimpleTypePoco>();
			var view = model.CreateView(new CopySimpleTypesConvention());
			Assert.IsNotNull(view);
			Assert.AreEqual(model.Name, view.Name);
			Assert.AreEqual(14, view.Fields.Length);
		}

		[TestMethod]
		public void CopyReferencesConvention()
		{
			var model = TypeModeller.GetModelOf<SimpleTypePoco>();
			var view = model.CreateView(new CopyReferencesConvention());
			Assert.IsNotNull(view);
			Assert.AreEqual(model.Name, view.Name);
			Assert.AreEqual(2, view.Fields.Length);
		}

		[TestMethod]
		public void DontFlattenWithoutTargetModel()
		{
			var model = TypeModeller.GetModelOf<ComplexTypePoco>();
			var view = model.CreateView(new FlattenSimpleTypesConvention());
			Assert.IsNotNull(view);
			Assert.AreEqual(model.Name, view.Name);
			Assert.AreEqual(0, view.Fields.Length);
		}

		[TestMethod]
		public void FlattenSimpleTypesConvention()
		{
			var model = TypeModeller.GetModelOf<ComplexTypePoco>();
			var view = model.CreateView<FlattenedPoco>(new FlattenSimpleTypesConvention());
			Assert.IsNotNull(view);
			Assert.AreEqual(nameof(FlattenedPoco), view.Name);
			Assert.AreEqual(28, view.Fields.Length);
		}

		private class SimpleTypePoco
		{
			public sbyte SByte { get; set; }
			public byte Byte { get; set; }
			public short Short { get; set; }
			public ushort UShort { get; set; }
			public int Integer { get; set; }
			public uint UInteger { get; set; }
			public long Long { get; set; }
			public ulong ULong { get; set; }
			public float Single { get; set; }
			public double Double { get; set; }
			public decimal Decimal { get; set; }
			public string String { get; set; }
			public Guid Guid { get; set; }
			public char Char { get; set; }

			public object DontMapObject { get; set; }
		}

		private class ComplexTypePoco
		{
			public SimpleTypePoco SimpleA { get; set; }
			public SimpleTypePoco SimpleB { get; set; }
		}

		private class FlattenedPoco
		{
			public sbyte SimpleASByte { get; set; }
			public byte SimpleAByte { get; set; }
			public short SimpleAShort { get; set; }
			public ushort SimpleAUShort { get; set; }
			public int SimpleAInteger { get; set; }
			public uint SimpleAUInteger { get; set; }
			public long SimpleALong { get; set; }
			public ulong SimpleAULong { get; set; }
			public float SimpleASingle { get; set; }
			public double SimpleADouble { get; set; }
			public decimal SimpleADecimal { get; set; }
			public string SimpleAString { get; set; }
			public Guid SimpleAGuid { get; set; }
			public char SimpleAChar { get; set; }
			public object SimpleADontMapObject { get; set; }

			public sbyte SimpleBSByte { get; set; }
			public byte SimpleBByte { get; set; }
			public short SimpleBShort { get; set; }
			public ushort SimpleBUShort { get; set; }
			public int SimpleBInteger { get; set; }
			public uint SimpleBUInteger { get; set; }
			public long SimpleBLong { get; set; }
			public ulong SimpleBULong { get; set; }
			public float SimpleBSingle { get; set; }
			public double SimpleBDouble { get; set; }
			public decimal SimpleBDecimal { get; set; }
			public string SimpleBString { get; set; }
			public Guid SimpleBGuid { get; set; }
			public char SimpleBChar { get; set; }
			public object SimpleBDontMapObject { get; set; }
		}
	}
}
