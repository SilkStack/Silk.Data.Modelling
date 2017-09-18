using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Represents the data structure of a CLR type.
	/// </summary>
	public abstract class TypedModel : Model
	{
		public Type DataType { get; }

		public new TypedModelField[] Fields { get; }

		public TypedModel(Type dataType, string name, IEnumerable<TypedModelField> fields, IEnumerable<object> metadata) 
			: base(name, fields, metadata)
		{
			DataType = dataType;
			Fields = base.Fields.OfType<TypedModelField>().ToArray();
		}
	}

	/// <summary>
	/// Represents the data structure of a CLR type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TypedModel<T> : TypedModel
	{
		public TypedModel(string name, IEnumerable<TypedModelField> fields, IEnumerable<object> metadata)
			: base(typeof(T), name, fields, metadata)
		{
		}

		public Modeller<TView> GetModeller<TView>()
		{
			return new Modeller<TView>(this);
		}

		/// <summary>
		/// Doesn't actually do anything. Is an extension point for extension methods.
		/// </summary>
		/// <typeparam name="TView"></typeparam>
		public class Modeller<TView>
		{
			public TypedModel<T> Model { get; }

			public Modeller(TypedModel<T> model)
			{
				Model = model;
			}
		}
	}
}
