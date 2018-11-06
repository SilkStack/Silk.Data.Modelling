using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Mapping
{
	public class TargetModelTransformer : IModelTransformer
	{
		private IModel _fromModel;
		private readonly List<ITargetField> _fields = new List<ITargetField>();
		private readonly string[] _rootPath;
		private readonly IModel _rootModel;

		public TargetModelTransformer(string[] rootPath, IModel rootModel)
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
				_fields.Add(new TargetField<T>(field.FieldName, field.CanRead, field.CanWrite, field.IsEnumerable,
					field.ElementType, new[] { field.FieldName }, _rootModel ?? _fromModel));
			else
				_fields.Add(new TargetField<T>(field.FieldName, field.CanRead, field.CanWrite, field.IsEnumerable,
					field.ElementType, _rootPath.Concat(new[] { field.FieldName }).ToArray(), _rootModel ?? _fromModel));
		}

		public TargetModel BuildTargetModel()
		{
			string[] selfPath;
			if (_rootPath == null || _rootPath.Length == 0)
				selfPath = new[] { "." };
			else
				selfPath = _rootPath.Concat(new[] { "." }).ToArray();
			return new TargetModel(_fromModel, _fields.ToArray(), selfPath, _rootModel);
		}
	}
}
