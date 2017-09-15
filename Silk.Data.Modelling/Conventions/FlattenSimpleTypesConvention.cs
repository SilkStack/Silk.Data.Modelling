using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silk.Data.Modelling.Conventions
{
	public class FlattenSimpleTypesConvention : ViewConvention
	{
		public override void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
			if (!IsSimpleType(field.DataType) || viewDefinition.FieldDefinitions.Any(q => q.Name == field.Name))
				return;
			var checkPaths = GetPaths(field.Name);
			foreach (var path in checkPaths)
			{
				var sourceField = GetField(path, model);
				if (sourceField != null && sourceField.DataType == field.DataType)
				{
					viewDefinition.FieldDefinitions.Add(
						new ViewFieldDefinition(field.Name, null)
						{
							DataType = field.DataType
						});
					break;
				}
			}
		}

		private TypedModelField GetField(string[] path, Model model)
		{
			TypedModelField ret = null;
			foreach (var pathComponent in path)
			{
				ret = model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (ret == null)
					break;
				model = ret.DataTypeModel;
			}
			return ret;
		}

		private static IEnumerable<string[]> GetPaths(string fieldName, string[] parentPath = null, int offset = 0)
		{
			var nextWord = ReadWord(fieldName, offset);
			if (nextWord != null)
			{
				offset += nextWord.Length;
				if (parentPath == null)
					parentPath = new string[0];

				//  create a new path array with the nextWord appened to the last element of the existing path
				if (parentPath.Length == 0)
				{
					var newPath = new string[] { nextWord };
					yield return newPath;
					foreach (var childPath in GetPaths(fieldName, newPath, offset))
						yield return childPath;
				}
				else
				{
					var newPath = new string[parentPath.Length];
					Array.Copy(parentPath, newPath, parentPath.Length);
					newPath[parentPath.Length - 1] += nextWord;
					yield return newPath;
					foreach (var childPath in GetPaths(fieldName, newPath, offset))
						yield return childPath;
				}
				//  create a new path array with the nextWord appended as a new element to the existing path
				var extendedPath = new string[parentPath.Length + 1];
				if (parentPath.Length > 0)
					Array.Copy(parentPath, extendedPath, parentPath.Length);
				extendedPath[parentPath.Length] = nextWord;
				yield return extendedPath;
				foreach (var childPath in GetPaths(fieldName, extendedPath, offset))
					yield return childPath;
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

		private static bool IsSimpleType(Type type)
		{
			return
				type == typeof(sbyte) ||
				type == typeof(byte) ||
				type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(long) ||
				type == typeof(ulong) ||
				type == typeof(Single) ||
				type == typeof(Double) ||
				type == typeof(Decimal) ||
				type == typeof(string) ||
				type == typeof(Guid) ||
				type == typeof(char)
				;
		}
	}
}
