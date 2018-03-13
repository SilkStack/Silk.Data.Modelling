using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silk.Data.Modelling.Mapping
{
	public static class ConventionUtilities
	{
		public static IEnumerable<(ISourceField sourceField, ITargetField targetField)> GetBindCandidatePairs(SourceModel fromModel, TargetModel toModel, MappingBuilder builder)
		{
			//  find fields with matching names that aren't already bound
			foreach (var fromField in fromModel.Fields.Where(q => q.CanRead && !builder.IsBound(q)))
			{
				var toField = toModel.Fields.FirstOrDefault(q => q.CanWrite && q.FieldName == fromField.FieldName);
				if (toField == null)
					continue;

				yield return (fromField, toField);
			}

			//  flattening candidates from the target model
			foreach (var toField in toModel.Fields.Where(q => q.CanWrite && !builder.IsBound(q)))
			{
				var potentialPaths = GetPaths(toField.FieldName).ToArray();
				if (potentialPaths.Length == 1)
					continue;

				ISourceField fromField = null;
				foreach (var sourcePath in potentialPaths)
				{
					var testField = fromModel.GetField(sourcePath);
					if (testField == null || !testField.CanRead)
						continue;
					fromField = testField;
					break;
				}
				if (fromField == null)
					continue;

				yield return (fromField, toField);
			}

			//  inflation candidates from the source model
			foreach (var fromField in fromModel.Fields.Where(q => q.CanRead))
			{
				var potentialPaths = GetPaths(fromField.FieldName).ToArray();
				if (potentialPaths.Length == 1)
					continue;

				ITargetField toField = null;

				foreach (var targetPath in potentialPaths)
				{
					var testField = toModel.GetField(targetPath);
					if (testField == null || !testField.CanWrite || testField.FieldType != fromField.FieldType ||
						builder.IsBound(testField))
						continue;
					toField = testField;
					break;
				}
				if (toField == null)
					continue;

				yield return (fromField, toField);
			}
		}

		public static (Type sourceType, Type targetType) GetCompareTypes(ISourceField sourceField, ITargetField targetField)
		{
			if (sourceField.IsEnumerable && targetField.IsEnumerable)
				return (sourceType: sourceField.ElementType, targetType: targetField.ElementType);
			return (sourceType: sourceField.FieldType, targetType: targetField.FieldType);
		}

		public static IEnumerable<string[]> GetPaths(string fieldName, string[] parentPath = null, int offset = 0)
		{
			var nextWord = ReadWord(fieldName, offset);
			if (nextWord != null)
			{
				offset += nextWord.Length;

				//  travel down a path using the current work concatenated with the parentpath's last element
				if (parentPath != null)
				{
					var concatPath = new string[parentPath.Length];
					Array.Copy(parentPath, concatPath, parentPath.Length);
					concatPath[parentPath.Length - 1] += nextWord;

					foreach (var subPath in GetPaths(fieldName, concatPath, offset))
						yield return subPath;
				}

				string[] nextPath;
				if (parentPath == null)
				{
					nextPath = new[] { nextWord };
				}
				else
				{
					nextPath = new string[parentPath.Length + 1];
					Array.Copy(parentPath, nextPath, parentPath.Length);
					nextPath[parentPath.Length] = nextWord;
				}
				//  travel down the path using the current word as a new path element
				foreach (var subPath in GetPaths(fieldName, nextPath, offset))
					yield return subPath;
			}
			else
			{
				yield return parentPath;
			}
		}

		private static string ReadWord(string subject, int offset)
		{
			var word = new StringBuilder();
			if (offset >= subject.Length)
				return null;
			for (var i = offset; i < subject.Length; i++)
			{
				var character = subject[i];
				if (i != offset && char.IsUpper(character))
					break;
				word.Append(character);
			}
			return word.ToString();
		}
	}
}
