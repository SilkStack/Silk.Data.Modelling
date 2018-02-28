using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	/// <summary>
	/// A store of mappings.
	/// </summary>
	public class MappingStore
	{
		private readonly List<MappingDetails> _store =
			new List<MappingDetails>();

		public void AddMapping(IModel fromModel, IModel toModel, Mapping mapping)
		{
			_store.Add(new MappingDetails(fromModel, toModel, mapping));
		}

		public bool TryGetMapping(IModel fromModel, IModel toModel, out Mapping mapping)
		{
			mapping = _store.FirstOrDefault(q => q.FromModel.Equals(fromModel) && q.ToModel == toModel)?.Mapping;
			return mapping != null;
		}

		private class MappingDetails
		{
			public IModel FromModel { get; }
			public IModel ToModel { get; }
			public Mapping Mapping { get; }

			public MappingDetails(IModel fromModel, IModel toModel, Mapping mapping)
			{
				FromModel = fromModel;
				ToModel = toModel;
				Mapping = mapping;
			}
		}
	}
}
