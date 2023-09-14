using System.Collections.Generic;

namespace Musimoji
{
	public static class ListExtensions
	{
		public static List<int> BuildIncrementedIntList(this List<int> list, int length, int initialValue = 0, int increment = 1)
		{
			for (var i = 0; i < length; i++)
			{
				list.Add(initialValue);
				initialValue += increment;
			}
			return list;
		}
	}
}