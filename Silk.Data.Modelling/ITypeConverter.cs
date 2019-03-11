using System;

namespace Silk.Data.Modelling
{
	public interface ITypeConverter
	{
		Type FromType { get; }
		Type ToType { get; }
	}

	public interface ITypeConverter<TFrom, TTo> : ITypeConverter
	{
		bool TryConvert(TFrom from, out TTo to);
	}

	public abstract class TypeConverter<TFrom, TTo> : ITypeConverter<TFrom, TTo>
	{
		public Type FromType => typeof(TFrom);

		public Type ToType => typeof(TTo);

		public abstract bool TryConvert(TFrom from, out TTo to);
	}
}
