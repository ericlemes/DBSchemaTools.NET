using System;

namespace DBInfo.Core.Model
{
	public class IndexColumn
	{
		public enum EnumOrder {
			Ascending = 0, 
			Descending = 1
		};

		public Column Column;
		public EnumOrder Order;
		
	}
}
