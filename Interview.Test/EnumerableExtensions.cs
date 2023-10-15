using Interview.Test.Reports;

namespace Interview.Test;

public static class EnumerableExtensions
{
    public static IEnumerable<ReportSessionMaxActivityPerDays.Request> ToSessionMaxActivityRequests(this IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var sublines = line.Split(';');

            // Предположим, что не кидаем ошибок, а если строка невалидна, то продолжаем..
            if (sublines.Length < 2)
            {
                continue;
            }

            if (!DateTime.TryParse(sublines[0], out var startedAt))
            {
                continue;
            }

            if (!DateTime.TryParse(sublines[1], out var endedAt))
            {
                continue;
            }

            yield return new ReportSessionMaxActivityPerDays.Request(startedAt, endedAt);
        }
    }
    
    public static IEnumerable<ReportOperatorMinutesPerStates.Request> ToOperatorMinutesPerStateRequests(this IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            var sublines = line.Split(';');

            // Предположим, что не кидаем ошибок, а если строка невалидна, то продолжаем..
            if (sublines.Length < 6)
            {
                continue;
            }

            if (!int.TryParse(sublines[5], out var seconds))
            {
                continue;
            }

            yield return new ReportOperatorMinutesPerStates.Request(sublines[3], sublines[4], TimeSpan.FromSeconds(seconds));
        }
    }
}