﻿using System;
using System.Linq;

namespace Silk.Data.Modelling.Bindings
{
	/// <summary>
	/// Copies values/references from models to views and vice-versa.
	/// </summary>
	public class BidirectionalAssignmentBinding : IModelBinding
	{
		public BindingDirection Direction => BindingDirection.Bidirectional;

		public string[] FieldPath { get; }

		public BidirectionalAssignmentBinding(params string[] fieldPath)
		{
			FieldPath = fieldPath;
		}

		public object ReadFromModel(IModelReadWriter modelReadWriter)
		{
			foreach (var pathComponent in FieldPath)
			{
				var field = modelReadWriter.Model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (field == null)
					throw new InvalidOperationException("Invalid field path.");
				modelReadWriter = modelReadWriter.GetField(field);
				if (modelReadWriter == null)
					throw new InvalidOperationException($"Couldn't get field \"{field.Name}\".");
			}
			return modelReadWriter.Value;
		}

		public void WriteToModel(IModelReadWriter modelReadWriter, object value, MappingContext mappingContext)
		{
			foreach (var pathComponent in FieldPath)
			{
				var field = modelReadWriter.Model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (field == null)
					throw new InvalidOperationException("Invalid field path.");
				modelReadWriter = modelReadWriter.GetField(field);
				if (modelReadWriter == null)
					throw new InvalidOperationException($"Couldn't get field \"{field.Name}\".");
			}
			modelReadWriter.Value = value;
		}
	}
}
