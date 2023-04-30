using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Common.Helpers
{
    /// <summary>
    /// Static class containing Fluent <see cref="DateTime"/> extension methods.
    /// </summary>
    public static class DateTimeExtensions
	{
		private static IList<DayOfWeek> Weekend = new List<DayOfWeek>() { DayOfWeek.Sunday, DayOfWeek.Saturday };

		/// <summary>
		/// Gets the number of days in the given month.
		/// </summary>
		/// <param name="value">The <see cref="DateTime"/> month to get the number of days for.</param>
		/// <returns>The number of days in the given month.</returns>
		public static int DaysInMonth(this DateTime value)
		{
			return DateTime.DaysInMonth(value.Year, value.Month);
		}

		/// <summary>
		/// Sets the day of the <see cref="DateTime"/> to the first day in that month.
		/// </summary>
		/// <param name="value">The current <see cref="DateTime"/> to be changed.</param>
		/// <returns>given <see cref="DateTime"/> with the day part set to the first day in that month.</returns>
		public static DateTime FirstDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		/// <summary>
		/// Sets the day of the <see cref="DateTime"/> to the last day in that month.
		/// </summary>
		/// <param name="value">The current DateTime to be changed.</param>
		/// <returns>given <see cref="DateTime"/> with the day part set to the last day in that month.</returns>
		public static DateTime LastDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.DaysInMonth());
		}

		public static DateTime GetNextBusinessDay(this DateTime value)
		{
			var nextBusinessDay = value.AddDays(1);
			if (!nextBusinessDay.IsBusinessDay())
				nextBusinessDay = nextBusinessDay.GetNextBusinessDay();
			return nextBusinessDay;
		}

		public static bool IsBusinessDay(this DateTime value)
		{
			return !Weekend.Contains(value.DayOfWeek);
		}

		/// <summary>
		/// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
		/// </summary>
		public static DateTime BeginningOfDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
		}

		/// <summary>
		/// Returns the timezone-adjusted Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
		/// </summary>
		public static DateTime BeginningOfDay(this DateTime date, int timezoneOffset)
		{
			return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind).AddHours(timezoneOffset);
		}

		/// <summary>
		/// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
		/// </summary>
		public static DateTime EndOfDay(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);
		}

		/// <summary>
		/// Returns the timezone-adjusted very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
		/// </summary>
		public static DateTime EndOfDay(this DateTime date, int timeZoneOffset)
		{
			return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind)
				.AddHours(timeZoneOffset);
		}

		/// <summary>
		/// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the next calendar year. 
		/// If that day does not exist in next year in same month, number of missing days is added to the last day in same month next year.
		/// </summary>
		public static DateTime NextYear(this DateTime start)
		{
			var nextYear = start.Year + 1;
			var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth(nextYear, start.Month);

			if (numberOfDaysInSameMonthNextYear < start.Day)
			{
				var differenceInDays = start.Day - numberOfDaysInSameMonthNextYear;
				var dateTime = new DateTime(nextYear, start.Month, numberOfDaysInSameMonthNextYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
				return dateTime + TimeSpan.FromDays(differenceInDays);
			}
			return new DateTime(nextYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
		}

		/// <summary>
		/// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the previous calendar year.
		/// If that day does not exist in previous year in same month, number of missing days is added to the last day in same month previous year.
		/// </summary>
		public static DateTime PreviousYear(this DateTime start)
		{
			var previousYear = start.Year - 1;
			var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth(previousYear, start.Month);

			if (numberOfDaysInSameMonthPreviousYear < start.Day)
			{
				var differenceInDays = start.Day - numberOfDaysInSameMonthPreviousYear;
				var dateTime = new DateTime(previousYear, start.Month, numberOfDaysInSameMonthPreviousYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
				return dateTime + TimeSpan.FromDays(differenceInDays);
			}
			return new DateTime(previousYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
		}

		/// <summary>
		/// Returns the original <see cref="DateTime"/> with Hour part changed to supplied hour parameter.
		/// </summary>
		public static DateTime SetTime(this DateTime originalDate, int hour)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns the original <see cref="DateTime"/> with Hour and Minute parts changed to supplied hour and minute parameters.
		/// </summary>
		public static DateTime SetTime(this DateTime originalDate, int hour, int minute)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns the original <see cref="DateTime"/> with Hour, Minute and Second parts changed to supplied hour, minute and second parameters.
		/// </summary>
		public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns the original <see cref="DateTime"/> with Hour, Minute, Second and Millisecond parts changed to supplied hour, minute, second and millisecond parameters.
		/// </summary>
		public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second, int millisecond)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Hour part.
		/// </summary>
		public static DateTime SetHour(this DateTime originalDate, int hour)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Minute part.
		/// </summary>
		public static DateTime SetMinute(this DateTime originalDate, int minute)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Second part.
		/// </summary>
		public static DateTime SetSecond(this DateTime originalDate, int second)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Millisecond part.
		/// </summary>
		public static DateTime SetMillisecond(this DateTime originalDate, int millisecond)
		{
			return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Year part.
		/// </summary>
		public static DateTime SetDate(this DateTime value, int year)
		{
			return new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Year and Month part.
		/// </summary>
		public static DateTime SetDate(this DateTime value, int year, int month)
		{
			return new DateTime(year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Year, Month and Day part.
		/// </summary>
		public static DateTime SetDate(this DateTime value, int year, int month, int day)
		{
			return new DateTime(year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Year part.
		/// </summary>
		public static DateTime SetYear(this DateTime value, int year)
		{
			return new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Month part.
		/// </summary>
		public static DateTime SetMonth(this DateTime value, int month)
		{
			return new DateTime(value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns <see cref="DateTime"/> with changed Day part.
		/// </summary>
		public static DateTime SetDay(this DateTime value, int day)
		{
			return new DateTime(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
		}

		/// <summary>
		/// Returns original <see cref="DateTime"/> value with time part set to midnight (00:00:00.000).
		/// </summary>
		/// <param name="value">The <see cref="DateTime"/> to find midnight for.</param>
		/// <returns>A <see cref="DateTime"/> value with time part set to midnight (12:00:00.000).</returns>
		public static DateTime Midnight(this DateTime value)
		{
			return value.SetTime(0, 0, 0, 0);
		}

		/// <summary>
		/// Returns original <see cref="DateTime"/> value with time part set to noon (12:00:00.000).
		/// </summary>
		/// <param name="value">The <see cref="DateTime"/> to find noon for.</param>
		/// <returns>A <see cref="DateTime"/> value with time part set to noon (12:00:00.000).</returns>
		public static DateTime Noon(this DateTime value)
		{
			return value.SetTime(12, 0, 0, 0);
		}

		/// <summary>
		/// Sets the day of the <see cref="DateTime"/> to the first day in that calendar quarter.
		/// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
		/// </summary>
		/// <param name="current"></param>
		/// <returns>given <see cref="DateTime"/> with the day part set to the first day in the quarter.</returns>
		public static DateTime FirstDayOfQuarter(this DateTime current)
		{
			var currentQuarter = (current.Month - 1) / 3 + 1;
			var firstDay = new DateTime(current.Year, 3 * currentQuarter - 2, 1);

			return current.SetDate(firstDay.Year, firstDay.Month, firstDay.Day);
		}

		/// <summary>
		/// Sets the day of the <see cref="DateTime"/> to the last day in that calendar quarter.
		/// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
		/// </summary>
		/// <param name="current"></param>
		/// <returns>given <see cref="DateTime"/> with the day part set to the last day in the quarter.</returns>
		public static DateTime LastDayOfQuarter(this DateTime current)
		{
			var currentQuarter = (current.Month - 1) / 3 + 1;
			var firstDay = current.SetDate(current.Year, 3 * currentQuarter - 2, 1);
			return firstDay.SetMonth(firstDay.Month + 2).LastDayOfMonth();
		}

		/// <summary>
		/// Returns a DateTime adjusted to the beginning of the week.
		/// </summary>
		/// <param name="dateTime">The DateTime to adjust</param>
		/// <returns>A DateTime instance adjusted to the beginning of the current week</returns>
		/// <remarks>the beginning of the week is controlled by the current Culture</remarks>
		public static DateTime FirstDayOfWeek(this DateTime dateTime)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
			var offset = dateTime.DayOfWeek - firstDayOfWeek < 0 ? 7 : 0;
			var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDayOfWeek;

			return dateTime.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
		}

		/// <summary>
		/// Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 => 2011-01-01T06:40:20.005
		/// </summary>
		/// <param name="current">The DateTime to adjust</param>
		/// <returns></returns>
		public static DateTime FirstDayOfYear(this DateTime current)
		{
			return current.SetDate(current.Year, 1, 1);
		}

		/// <summary>
		/// Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T06:40:20.005
		/// </summary>
		/// <param name="current">The DateTime to adjust</param>
		/// <returns></returns>
		public static DateTime LastDayOfWeek(this DateTime current)
		{
			return current.FirstDayOfWeek().AddDays(6);
		}

		/// <summary>
		/// Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T06:40:20.005
		/// </summary>
		/// <param name="current">The DateTime to adjust</param>
		/// <returns></returns>
		public static DateTime LastDayOfYear(this DateTime current)
		{
			return current.SetDate(current.Year, 12, 31);
		}

		/// <summary>
		/// Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 => 2009-12-20T06:40:20.005
		/// If the previous month doesn't have that many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005 => 2009-02-28T06:40:20.005
		/// </summary>
		/// <param name="current">The DateTime to adjust</param>
		/// <returns></returns>
		public static DateTime PreviousMonth(this DateTime current)
		{
			var year = current.Month == 1 ? current.Year - 1 : current.Year;

			var month = current.Month == 1 ? 12 : current.Month - 1;

			var firstDayOfPreviousMonth = current.SetDate(year, month, 1);

			var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

			var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

			return firstDayOfPreviousMonth.SetDay(day);
		}

		/// <summary>
		/// Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 => 2013-01-05T06:40:20.005
		/// If the next month doesn't have that many days the last day of the next month is used. Eg, 2013-01-31T06:40:20.005 => 2013-02-28T06:40:20.005
		/// </summary>
		/// <param name="current">The DateTime to adjust</param>
		/// <returns></returns>
		public static DateTime NextMonth(this DateTime current)
		{

			var year = current.Month == 12 ? current.Year + 1 : current.Year;

			var month = current.Month == 12 ? 1 : current.Month + 1;

			var firstDayOfNextMonth = current.SetDate(year, month, 1);

			var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

			var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

			return firstDayOfNextMonth.SetDay(day);
		}

		/// <summary>
		/// Adds the given number of business days to the <see cref="DateTime"/>.
		/// </summary>
		/// <param name="current">The date to be changed.</param>
		/// <param name="days">Number of business days to be added.</param>
		/// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
		public static DateTime AddBusinessDays(this DateTime current, int days)
		{
			var sign = Math.Sign(days);
			var unsignedDays = Math.Abs(days);
			for (var i = 0; i < unsignedDays; i++)
			{
				do
				{
					current = current.AddDays(sign);
				}
				while (current.DayOfWeek == DayOfWeek.Saturday ||
					   current.DayOfWeek == DayOfWeek.Sunday);
			}
			return current;
		}

		/// <summary>
		/// Subtracts the given number of business days to the <see cref="DateTime"/>.
		/// </summary>
		/// <param name="current">The date to be changed.</param>
		/// <param name="days">Number of business days to be subtracted.</param>
		/// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
		public static DateTime SubtractBusinessDays(this DateTime current, int days)
		{
			return AddBusinessDays(current, -days);
		}
	}
}
