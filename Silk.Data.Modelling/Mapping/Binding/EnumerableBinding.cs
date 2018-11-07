using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class EnumerableBinding<TFrom, TTo, TFromElement, TToElement> : MappingBinding
		where TFrom : class, IEnumerable<TFromElement>
		where TTo : class, IEnumerable<TToElement>
	{
		private readonly TypeModel<EnumerableResult> _toModel = TypeModel.GetModelOf<EnumerableResult>();
		private readonly TypeModel<TFromElement> _fromModel = TypeModel.GetModelOf<TFromElement>();
		private readonly IFieldReference _fromSelfFieldReference;
		private readonly EnumerableReaderMutator _readerMutator
			= new EnumerableReaderMutator();
		private readonly EnumerableWriterMutator _writerMutator
			= new EnumerableWriterMutator();

		public EnumerableBinding(IFieldReference from, IFieldReference to, MappingBinding elementBinding) : base(from, to)
		{
			ElementBinding = elementBinding;

			_fromSelfFieldReference = _fromModel.GetFieldReference(_fromModel.TransformToSourceModel().GetSelf());
		}

		public MappingBinding ElementBinding { get; }

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var source = from.ReadField<TFrom>(From);
			var result = new EnumerableResult();

			var resultWriter = new ObjectReadWriter(result, _toModel, typeof(EnumerableResult));
			var itemReader = new ObjectReadWriter(null, _fromModel, typeof(TFromElement));
			foreach (var item in source)
			{
				itemReader.WriteField<TFromElement>(_fromSelfFieldReference, item);

				itemReader.FieldResolver.AddMutator(_readerMutator);
				resultWriter.FieldResolver.AddMutator(_writerMutator);

				ElementBinding.CopyBindingValue(itemReader, resultWriter);

				itemReader.FieldResolver.RemoveMutator(_readerMutator);
				resultWriter.FieldResolver.RemoveMutator(_writerMutator);

				result.Reset();
			}

			if (typeof(TTo).IsArray)
				to.WriteField<TTo>(To, result.ToArray() as TTo);
			else
				to.WriteField<TTo>(To, result as TTo);
		}

		private class EnumerableResult : List<TToElement>
		{
			private TToElement _last;

			public void Reset()
			{
				_last = default(TToElement);
			}

			public TToElement Element
			{
				get => _last;
				set
				{
					_last = value;
					Add(value);
				}
			}
		}

		private class EnumerableReaderMutator : IFieldReferenceMutator
		{
			public void MutatePath(List<ModelPathNode> pathNodes)
			{
				pathNodes.RemoveAt(0);
				if (pathNodes.Count == 0)
					pathNodes.Add(RootPathNode.Instance);
			}
		}

		private class EnumerableWriterMutator : IFieldReferenceMutator
		{
			private static readonly FieldPathNode _fieldPathNode = new FieldPathNode("Element", null);
			private static readonly TreePathNode _treePathNode = new TreePathNode("Element", 
				TypeModel.GetModelOf<EnumerableResult>().Fields.First(q => q.FieldName == "Element"));

			public void MutatePath(List<ModelPathNode> pathNodes)
			{
				pathNodes.RemoveAt(0);
				if (pathNodes.Count == 0)
					pathNodes.Add(_fieldPathNode);
				else
					pathNodes.Insert(0, _treePathNode);
			}
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
				new object[] { fromField, toField, elementBinding }) as MappingBinding;
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
				new object[] { fromField, toField, elementBinding }) as MappingBinding;
		}
	}
}
