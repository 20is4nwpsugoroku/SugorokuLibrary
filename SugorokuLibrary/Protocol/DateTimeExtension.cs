using System;

namespace SugorokuLibrary.Protocol
{
	public static class DateTimeExtension
	{
		public static long ToTimeStamp(this DateTime dateTime)
		{
			var ts = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long) ts.TotalSeconds;
		}

		public static DateTime ToDateTime(this long timeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();
		}
	}
}