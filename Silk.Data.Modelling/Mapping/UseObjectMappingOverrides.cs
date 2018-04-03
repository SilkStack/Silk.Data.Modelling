using Silk.Data.Modelling.Mapping.Binding;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Silk.Data.Modelling.Mapping
{
	public class UseObjectMappingOverrides : IMappingConvention
	{
		private static readonly MethodInfo _createBindingsMethod
			= typeof(UseObjectMappingOverrides).GetTypeInfo().GetDeclaredMethod("CreateBindingsImpl");

		private readonly ReaderWriterLockSlim _lock
			= new ReaderWriterLockSlim();
		private readonly List<IObjectMappingOverride> _mappingOverrides
			= new List<IObjectMappingOverride>();

		public IObjectMappingOverride[] MappingOverrides
		{
			get
			{
				_lock.EnterReadLock();
				try
				{
					return _mappingOverrides.ToArray();
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
		}

		public void AddMappingOverride<TFrom, TTo>(IObjectMappingOverride<TFrom, TTo> objectMappingOverride)
		{
			_lock.EnterWriteLock();
			try
			{
				_mappingOverrides.Add(objectMappingOverride);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void RemoveMappingOverride<TFrom, TTo>(IObjectMappingOverride<TFrom, TTo> objectMappingOverride)
		{
			_lock.EnterWriteLock();
			try
			{
				_mappingOverrides.Remove(objectMappingOverride);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public void CreateBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			CreateTypeBindings(fromModel, toModel, builder);

			foreach (var (fromField, toField) in ConventionUtilities.GetBindCandidatePairs(fromModel, toModel, builder))
			{
				var (fromType, toType) = ConventionUtilities.GetCompareTypes(fromField, toField);
				var mappingOverrideType = typeof(IObjectMappingOverride<,>).MakeGenericType(fromType, toType);

				IObjectMappingOverride[] mappingOverrides;
				_lock.EnterReadLock();
				try
				{
					mappingOverrides = _mappingOverrides
						.Where(q => q.GetType().GetTypeInfo().ImplementedInterfaces.Contains(mappingOverrideType))
						.GroupBy(q => q.GetType())
						.Select(q => q.First())
						.ToArray();
				}
				finally
				{
					_lock.ExitReadLock();
				}

				if (mappingOverrides.Length > 0)
				{
					var fromTypeModel = TypeModel.GetModelOf(fromType);
					var toTypeModel = TypeModel.GetModelOf(toType);

					//  if a mapping already exists, use it, otherwise build it
					if (!builder.MappingStore.TryGetMapping(fromTypeModel, toTypeModel, out var subMapping) &&
						!builder.BuilderStack.IsBeingMapped(fromTypeModel, toTypeModel))
					{
						var subBuilder = new MappingBuilder(fromTypeModel, toTypeModel,
							builder.MappingStore, builder.BuilderStack);
						foreach (var convention in builder.Conventions)
						{
							subBuilder.AddConvention(convention);
						}
						subMapping = subBuilder.BuildMapping();
					}

					builder
						.Bind(toField)
						.From(fromField)
						.MapUsing<SubmappingBinding, MappingStore>(builder.MappingStore);
				}
			}
		}

		private void CreateTypeBindings(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			var toTypeModel = toModel.FromModel as TypeModel;
			if (toTypeModel == null)
				return;

			var fromTypeModel = fromModel.FromModel as TypeModel;
			if (fromTypeModel == null)
				return;

			_createBindingsMethod
				.MakeGenericMethod(fromTypeModel.Type, toTypeModel.Type)
				.Invoke(this, new object[] { fromModel, toModel, builder });
		}

		private void CreateBindingsImpl<TFrom, TTo>(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			IObjectMappingOverride<TFrom, TTo>[] mappingOverrides;
			_lock.EnterReadLock();
			try
			{
				mappingOverrides = _mappingOverrides
					.OfType<IObjectMappingOverride<TFrom, TTo>>()
					.GroupBy(q => q.GetType())
					.Select(q => q.First())
					.ToArray();
			}
			finally
			{
				_lock.ExitReadLock();
			}

			var objectMappingBuilder = new ObjectMappingBuilder<TFrom, TTo>(builder, fromModel, toModel);
			foreach (var mappingOverride in mappingOverrides)
				mappingOverride.CreateBindings(objectMappingBuilder);
		}
	}
}
