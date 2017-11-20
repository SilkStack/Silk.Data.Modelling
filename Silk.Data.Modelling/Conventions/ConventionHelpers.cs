using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silk.Data.Modelling.Conventions
{
	public static class ConventionHelpers
	{
		public static ModelField GetField(string[] path, Model model)
		{
			ModelField ret = null;
			foreach (var pathComponent in path)
			{
				ret = model.Fields.FirstOrDefault(q => q.Name == pathComponent);
				if (ret == null)
					break;
				model = ret.DataTypeModel;
			}
			return ret;
		}

		public static IEnumerable<string[]> GetPaths(string fieldName, string[] parentPath = null, int offset = 0)
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
	}
}
