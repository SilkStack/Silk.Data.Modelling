using System;
using System.Linq;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Reads and writes fields on an object tree.
	/// </summary>
	public class ObjectReadWriter : IModelReadWriter
	{
		public IModel Model { get; }
		private object _instance;
		private ObjectReadWriteHelpers _readWriteMethods;

		public ObjectReadWriter(object instance, IModel model, Type objectType)
		{
			_instance = instance;
			Model = model;
			_readWriteMethods = ObjectReadWriteHelpers.GetForType(objectType);
		}

		public T ReadField<T>(Span<string> path)
			=> ReadField<T>(Model.ResolveNode(path));

		public void WriteField<T>(Span<string> path, T value)
			=> WriteField<T>(Model.ResolveNode(path), value);


		public T ReadField<T>(ModelNode modelNode)
		{
			if (_instance == null)
				return default(T);

			var currentObj = _instance;
			var currentReadWriterHelpers = _readWriteMethods;
			foreach (var pathNode in modelNode.Path)
			{
				switch (pathNode.PathNodeType)
				{
					case ModelPathNodeType.Root:
						return (T)currentObj;
					case ModelPathNodeType.Field:
						return currentReadWriterHelpers.GetTypedValue<T>(currentObj, pathNode.PathNodeName);
					case ModelPathNodeType.Tree:
						currentObj = currentReadWriterHelpers.GetValue(currentObj, pathNode.PathNodeName);
						if (currentObj == null)
							return default(T);
						currentReadWriterHelpers = ObjectReadWriteHelpers.GetForType(pathNode.Field.FieldType);
						break;
				}
			}

			throw new ArgumentException("Path does not lead to a valid field.", nameof(modelNode));
		}

		public void WriteField<T>(ModelNode modelNode, T value)
		{
			var currentObj = _instance;
			var currentReadWriterHelpers = _readWriteMethods;
			var previousReadWriterHelpers = _readWriteMethods;
			var previousNode = default(ModelPathNode);
			var previousObj = default(object);
			foreach (var pathNode in modelNode.Path)
			{
				switch (pathNode.PathNodeType)
				{
					case ModelPathNodeType.Root:
						if (modelNode.Path.Count == 1)
							_instance = value;
						else
							previousReadWriterHelpers.SetTypedValue<T>(previousObj, previousNode.PathNodeName, value);
						break;
					case ModelPathNodeType.Field:
						if (currentObj == null)
							return;
						currentReadWriterHelpers.SetTypedValue<T>(currentObj, pathNode.PathNodeName, value);
						break;
					case ModelPathNodeType.Tree:
						previousObj = currentObj;
						if (currentObj == null)
							return;
						currentObj = currentReadWriterHelpers.GetValue(currentObj, pathNode.PathNodeName);
						previousReadWriterHelpers = currentReadWriterHelpers;
						currentReadWriterHelpers = ObjectReadWriteHelpers.GetForType(pathNode.Field.FieldType);
						break;
				}
				previousNode = pathNode;
			}
		}

		/// <summary>
		/// Create a new ObjectReadWriter for writing to instances of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static ObjectReadWriter Create<T>(T instance = default(T))
		{
			return new ObjectReadWriter(instance, TypeModel.GetModelOf<T>(), typeof(T));
		}

		/// <summary>
		/// Create a new ObjectReadWriter for writing to instances of the provided type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static ObjectReadWriter Create(Type type, object instance = null)
		{
			return new ObjectReadWriter(instance, TypeModel.GetModelOf(type), type);
		}
	}
}
