namespace Silk.Data.Modelling.Conventions
{
	public abstract class ViewConvention
	{
		/// <summary>
		/// View types the view convention should be executed for.
		/// </summary>
		public abstract ViewType SupportedViewTypes { get; }
		/// <summary>
		/// When true the convention needs to be executed until no extra fields are added to the defined view.
		/// </summary>
		public abstract bool PerformMultiplePasses { get; }
		/// <summary>
		/// When true and PerformMultiplePasses is false skip this convention if a field has been defined for a field name.
		/// </summary>
		public abstract bool SkipIfFieldDefined { get; }
	}

	/// <summary>
	/// Defines a convention used for creating views.
	/// </summary>
	public abstract class ViewConvention<TBuilder> : ViewConvention
		where TBuilder : ViewBuilder
	{
		public virtual void MakeModelField(TBuilder viewBuilder, ModelField field)
		{
		}

		/// <summary>
		/// Defines view fields from the given model field.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="field"></param>
		/// <param name="viewDefinition"></param>
		public virtual void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
		}

		public virtual void FinalizeModel(TBuilder viewBuilder)
		{
		}

		/// <summary>
		/// Makes any finalizing changes to a view definition before a view is created.
		/// </summary>
		/// <param name="viewDefinition"></param>
		public virtual void FinalizeModel(ViewDefinition viewDefinition)
		{
		}
	}
}
