using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public abstract class BindingScope<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public IReadOnlyCollection<IBinding<TFromModel, TFromField, TToModel, TToField>> Bindings { get; }
		public IReadOnlyCollection<BindingScope<TFromModel, TFromField, TToModel, TToField>> Scopes { get; }

		protected BindingScope(
			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> bindings,
			IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> scopes
			)
		{
			Bindings = bindings.ToArray();
			Scopes = scopes.ToArray();
		}

		public virtual void RunScope(
			IGraphReader<TFromModel, TFromField> source,
			IGraphWriter<TToModel, TToField> destination
			)
		{
			foreach (var binding in Bindings)
				binding.Run(source, destination);

			foreach (var scope in Scopes)
				scope.RunScope(source, destination);
		}
	}

	/// <summary>
	/// A scope for binding a branch of the model graph.
	/// </summary>
	/// <typeparam name="TFromModel"></typeparam>
	/// <typeparam name="TFromField"></typeparam>
	/// <typeparam name="TToModel"></typeparam>
	/// <typeparam name="TToField"></typeparam>
	public class BranchBindingScope<TFromModel, TFromField, TToModel, TToField> :
		BindingScope<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public BranchBindingScope(
			IEnumerable<IBinding<TFromModel, TFromField, TToModel, TToField>> bindings,
			IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> scopes) :
			base(bindings, scopes)
		{
		}
	}

	/// <summary>
	/// A scope for binding an enumerable of the model graph.
	/// </summary>
	/// <typeparam name="TFromModel"></typeparam>
	/// <typeparam name="TFromField"></typeparam>
	/// <typeparam name="TToModel"></typeparam>
	/// <typeparam name="TToField"></typeparam>
	public class EnumerableBindingScope<TFromModel, TFromField, TToModel, TToField, TFromData, TToData> :
		BindingScope<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public EnumerableBindingScope(
			IBinding<TFromModel, TFromField, TToModel, TToField> binding,
			IEnumerable<BindingScope<TFromModel, TFromField, TToModel, TToField>> scopes) :
			base(new[] { binding }, scopes)
		{
		}

		public override void RunScope(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			var binding = Bindings.First();
			using (var readerEnumerator = source.GetEnumerator<TFromData>(binding.EnumerablePath))
			using (var writerStream = destination.CreateEnumerableStream<TToData>(binding.ToPath))
			while (readerEnumerator.MoveNext())
			{
				var scopeSource = readerEnumerator.Current;
				var scopeDestination = writerStream.CreateNew();

				base.RunScope(scopeSource, scopeDestination);
			}
		}
	}
}
