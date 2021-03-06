﻿namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A mapping between two models.
	/// </summary>
	/// <typeparam name="TFromModel"></typeparam>
	/// <typeparam name="TFromField"></typeparam>
	/// <typeparam name="TToModel"></typeparam>
	/// <typeparam name="TToField"></typeparam>
	public interface IMapping<TFromModel, TFromField, TToModel, TToField>
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
		where TFromField : class, IField
		where TToField : class, IField
	{
		/// <summary>
		/// Gets the model the mapping maps from.
		/// </summary>
		TFromModel FromModel { get; }

		/// <summary>
		/// Gets the model the mapping maps to.
		/// </summary>
		TToModel ToModel { get; }

		/// <summary>
		/// Perform the mapping.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		void Map(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination);
	}
}
