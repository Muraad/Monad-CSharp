/*
 *  Copyright (C) 2014  Muraad Nofal

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;

namespace Monads
{
    public static class DateTimeExtension
    {
        public static readonly long MS_PER_SECOND = 1000;
        public static readonly long MS_PER_MINUTE = 60 * MS_PER_SECOND;
        public static readonly long MS_PER_HOUR = 60 * MS_PER_MINUTE;
        public static readonly long MS_PER_DAY = 60 * MS_PER_HOUR;
        public static readonly long MS_PER_MONTH = 30 * MS_PER_DAY;
        public static readonly long MS_PER_YEAR = (7 * 31 * MS_PER_DAY) + (4 * 30 * MS_PER_DAY) + (28 * MS_PER_DAY);

        /*public static long Diff(this DateTime dateTime, DateTime other)
        {

            long years = (dateTime.Year - other.Year) * MS_PER_YEAR;
            long months = (dateTime.Month - other.Month) * MS_PER_MONTH;
            long days = (dateTime.Day - other.Day) * MS_PER_DAY;
            long hours = (dateTime.Hour - other.Hour) * MS_PER_HOUR;
            long minutes = (dateTime.Minute - other.Minute) * MS_PER_MINUTE;
            long seconds = (dateTime.Second - other.Second) * MS_PER_SECOND;
            long milliseconds = dateTime.Millisecond - other.Millisecond;

            return years + months + days + hours + seconds + milliseconds;
        }*/

        public static long Diff(this DateTime dateTime, DateTime other)
        {

            long years = (dateTime.Year - other.Year) * MS_PER_YEAR;
            long months = dateTime.Month - other.Month;
            long days = dateTime.Day - other.Day;
            long hours = dateTime.Hour - other.Hour;
            long minutes = dateTime.Minute - other.Minute;
            long seconds = dateTime.Second - other.Second;
            long milliseconds = dateTime.Millisecond - other.Millisecond;

            long diff = milliseconds;

            if (Math.Abs(years) > 0)
                diff = years * MS_PER_YEAR;
            else if (Math.Abs(months) > 0)
                diff = months * MS_PER_MONTH;
            else if (Math.Abs(days) > 0)
                diff = days * MS_PER_DAY;
            else if (Math.Abs(hours) > 0)
                diff = hours * MS_PER_HOUR;
            else if (Math.Abs(minutes) > 0)
                diff = minutes * MS_PER_MINUTE;
            else if (Math.Abs(seconds) > 0)
                diff = seconds * MS_PER_SECOND;

            return diff;
        }

    }
}
