using System;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Reads and writes model fields to a store instance.
	/// </summary>
	public interface IModelReadWriter
	{
		/// <summary>
		/// Gets the model.
		/// </summary>
		IModel Model { get; }

		/// <summary>
		/// Reads a given field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="modelNode"></param>
		/// <returns></returns>
		T ReadField<T>(ModelNode modelNode);

		/// <summary>
		/// Writes to a given field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="modelNode"></param>
		/// <param name="value"></param>
		void WriteField<T>(ModelNode modelNode, T value);

		/// <summary>
		/// Reads a given field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		[Obsolete]
		T ReadField<T>(Span<string> path);

		/// <summary>
		/// Writes to a given field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <param name="value"></param>
		[Obsolete]
		void WriteField<T>(Span<string> path, T value);
	}
}
