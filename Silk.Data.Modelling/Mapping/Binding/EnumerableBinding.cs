using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class EnumerableBinding<TFrom, TTo, TFromElement, TToElement> : MappingBinding
		where TFrom : class, IEnumerable<TFromElement>
		where TTo : class, IEnumerable<TToElement>
	{
		public EnumerableBinding(IFieldReference from, IFieldReference to, MappingBinding elementBinding) : base(from, to)
		{
			ElementBinding = elementBinding;
		}

		public MappingBinding ElementBinding { get; }

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var source = from.ReadField<TFrom>(From);
			var result = new List<TToElement>();

			var sourceReader = new EnumerableModelReader<TFromElement>(source, TypeModel.GetModelOf(typeof(TFromElement)));
			var resultWriter = new EnumerableModelWriter<TToElement>(result, TypeModel.GetModelOf(typeof(TToElement)));

			while(sourceReader.MoveNext())
			{
				ElementBinding.CopyBindingValue(sourceReader, resultWriter);
			}

			if (typeof(TTo).IsArray)
				to.WriteField<TTo>(To, result.ToArray() as TTo);
			else
				to.WriteField<TTo>(To, result as TTo);
		}
	}

	public class EnumerableBindingFactory : IMappingBindingFactory<IMappingBindingFactory>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, IMappingBindingFactory bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory.CreateBinding))
				.MakeGenericMethod(fromField.Field.ElementType, toField.Field.ElementType)
				.Invoke(bindingOption, new object[] { fromField, toField })
				as MappingBinding;

			return Activator.CreateInstance(typeof(EnumerableBinding<,,,>)
				.MakeGenericType(typeof(TFrom), typeof(TTo), fromField.Field.ElementType, toField.Field.ElementType),
				new object[] { new string[0], new string[0], elementBinding }) as MappingBinding;
		}
	}

	public class EnumerableBindingFactory<T> : IMappingBindingFactory<(IMappingBindingFactory<T> factory, T option)>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(IFieldReference fromField, IFieldReference toField, (IMappingBindingFactory<T> factory, T option) bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory<T>).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory<T>.CreateBinding))
				.MakeGenericMethod(fromField.Field.ElementType, toField.Field.ElementType)
				.Invoke(bindingOption.factory, new object[] { fromField, toField, bindingOption.option })
				as MappingBinding;

			return Activator.CreateInstance(typeof(EnumerableBinding<,,,>)
				.MakeGenericType(typeof(TFrom), typeof(TTo), fromField.Field.ElementType, toField.Field.ElementType),
				new object[] { new string[0], new string[0], elementBinding }) as MappingBinding;
		}
	}

	internal class EnumerableModelWriter<TToElement> : IModelReadWriter
	{
		public IModel Model { get; }
		public List<TToElement> List { get; }

		public IFieldResolver FieldResolver => throw new NotImplementedException();

		private ObjectReadWriter _objectReadWriter;

		public EnumerableModelWriter(List<TToElement> list, IModel model)
		{
			Model = model;
			List = list;
		}

		public T ReadField<T>(Span<string> path)
		{
			//  todo: how to get a potential source object
			return default(T);
		}

		public void WriteField<T>(Span<string> path, T value)
		{
			if (path.Length > 1 && path[1] == ".")
			{
				//  path taken for object mapping
				List.Add((TToElement)(object)value);
				_objectReadWriter = new ObjectReadWriter((TToElement)(object)value, Model, typeof(TToElement));
			}
			else
			{
				if (_objectReadWriter != null)
					_objectReadWriter.WriteField<T>(path.Slice(1), value);
				else
					//  path taken for straight value binding
					List.Add((TToElement)(object)value);
			}
		}

		public T ReadField<T>(ModelNode modelNode)
		{
			throw new NotImplementedException();
		}

		public void WriteField<T>(ModelNode modelNode, T value)
		{
			throw new NotImplementedException();
		}

		public T ReadField<T>(IFieldReference field)
		{
			throw new NotImplementedException();
		}

		public void WriteField<T>(IFieldReference field, T value)
		{
			throw new NotImplementedException();
		}
	}

	internal class EnumerableModelReader<TFromElement> : IModelReadWriter
	{
		public IModel Model { get; }
		public IEnumerable Source { get; }

		public IFieldResolver FieldResolver => throw new NotImplementedException();

		private IEnumerator _enumerator;
		private ObjectReadWriter _objectReadWriter;

		public EnumerableModelReader(IEnumerable source, IModel model)
		{
			Model = model;
			Source = source;
			_enumerator = Source.GetEnumerator();
		}

		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				_objectReadWriter = new ObjectReadWriter(_enumerator.Current, Model, typeof(TFromElement));
				return true;
			}

			_objectReadWriter = null;
			return false;
		}

		public T ReadField<T>(Span<string> path)
		{
			if (path.Length == 1)
				return _objectReadWriter.ReadField<T>(
					new Span<string>(
						path.Slice(1).ToArray().Concat(new[] { "." }).ToArray()
					));
			return _objectReadWriter.ReadField<T>(path.Slice(1));
		}

		public void WriteField<T>(Span<string> path, T value)
		{
			throw new NotSupportedException();
		}

		public T ReadField<T>(ModelNode modelNode)
		{
			throw new NotImplementedException();
		}

		public void WriteField<T>(ModelNode modelNode, T value)
		{
			throw new NotImplementedException();
		}

		public T ReadField<T>(IFieldReference field)
		{
			throw new NotImplementedException();
		}

		public void WriteField<T>(IFieldReference field, T value)
		{
			throw new NotImplementedException();
		}
	}
}
