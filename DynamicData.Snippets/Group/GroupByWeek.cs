using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using DynamicData.Kernel;

namespace DynamicData.Snippets.Group
{
    public class MyCalendarEntry
    {
        public int Id { get; }
        public DateTime DateTime { get; }
        public string Entry { get; }

        public MyCalendarEntry(int id, DateTime dateTime, string entry)
        {
            Id = id;
            DateTime = dateTime;
            Entry = entry;
        }
    }

    public class MyYearAndWeekAggregations: IDisposable
    {
        private readonly IDisposable _cleanUp;
        public ReadOnlyObservableCollection<AnnualGrouping> Years { get; }

        public MyYearAndWeekAggregations(IObservableCache<MyCalendarEntry, int> source)
        {
            _cleanUp = source.Connect()
                .Group(x => x.DateTime.Year)
                .Transform(group => new AnnualGrouping(group))
                .DisposeMany()
                .Bind(out var years)
                .Subscribe();

            Years = years;
        }
        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class AnnualGrouping: IDisposable
    {
        private readonly IDisposable _cleanUp;

        public int Year { get; }
        public ReadOnlyObservableCollection<WeeklyGrouping> Weeks { get; }

        public AnnualGrouping(IGroup<MyCalendarEntry, int, int> annualGroup)
        {
            _cleanUp = annualGroup.Cache.Connect()
                .GroupWithImmutableState(x => x.DateTime.GetIso8601WeekOfYear())
                .Transform(g => new WeeklyGrouping(annualGroup.Key, g.Key, g.Items))
                .Bind(out var weeks)
                .Subscribe();

            Year = annualGroup.Key;
            Weeks = weeks;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }

    public class WeeklyGrouping
    {
        public int Year { get; }
        public int Week { get; }
        private List<MyCalendarEntry> Entries { get; }

        public WeeklyGrouping(int year, int week, IEnumerable<MyCalendarEntry> entries)
        {
            Year = year;
            Week = week;
            Entries = new List<MyCalendarEntry>(entries);
        }
    }

    public static class MySuperCoolExtensions
    {
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

}
