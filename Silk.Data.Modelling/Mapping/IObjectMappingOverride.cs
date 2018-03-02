using System;

namespace Silk.Data.Modelling.Mapping
{
	public interface IObjectMappingOverride
	{
	}

	public interface IObjectMappingOverride<TFrom, TTo> : IObjectMappingOverride
	{
		void CreateBindings(ObjectMappingBuilder<TFrom, TTo> builder);
	}

	public class ObjectMappingBuilder<TFrom, TTo>
	{
		private readonly MappingBuilder _mappingBuilder;
		private readonly SourceModel _sourceModel;
		private readonly TargetModel _targetModel;

		public ObjectMappingBuilder(MappingBuilder mappingBuilder, SourceModel sourceModel,
			TargetModel targetModel)
		{
			_mappingBuilder = mappingBuilder;
			_sourceModel = sourceModel;
			_targetModel = targetModel;
		}

		public void ConstructWithFactory(Func<TFrom, TTo> factory)
		{
			_mappingBuilder
				.Bind(_targetModel.GetSelf())
				.AssignUsing<UseFactoryBinding<TFrom,TTo>, Func<TFrom, TTo>>(factory);
		}
	}
}
