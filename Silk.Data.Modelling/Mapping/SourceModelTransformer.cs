using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class SourceModelTransformer : IModelTransformer
	{
		private IModel _fromModel;
		private readonly List<ISourceField> _fields = new List<ISourceField>();
		private readonly string[] _rootPath;
		private readonly IModel _rootModel;

		public SourceModelTransformer(string[] rootPath, IModel rootModel)
		{
			_rootPath = rootPath;
			_rootModel = rootModel;
		}

		public void VisitModel<TField>(IModel<TField> model) where TField : IField
		{
			_fromModel = model;
		}

		public void VisitField<T>(IField<T> field)
		{
			if (_rootPath == null || _rootPath.Length == 0)
				_fields.Add(new SourceField<T>(field.FieldName, field.CanRead, field.CanWrite, field.IsEnumerable,
					field.ElementType, new[] { field.FieldName }, _rootModel ?? _fromModel));
			else
				_fields.Add(new SourceField<T>(field.FieldName, field.CanRead, field.CanWrite, field.IsEnumerable,
					field.ElementType, _rootPath.Concat(new[] { field.FieldName }).ToArray(), _rootModel ?? _fromModel));
		}

		public SourceModel BuildSourceModel()
		{
			string[] selfPath;
			if (_rootPath == null || _rootPath.Length == 0)
				selfPath = new[] { "." };
			else
				selfPath = _rootPath.Concat(new[] { "." }).ToArray();
			return new SourceModel(_fromModel, _fields.ToArray(), selfPath, _rootModel);
		}
	}
}
