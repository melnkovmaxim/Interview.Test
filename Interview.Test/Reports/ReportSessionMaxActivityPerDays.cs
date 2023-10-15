namespace Interview.Test.Reports;

public class ReportSessionMaxActivityPerDays: IReport<ReportSessionMaxActivityPerDays.Request, ReportSessionMaxActivityPerDays.Response>
{
    public record Request(DateTime StartedAt, DateTime EndedAt);
    public record Response(DateOnly Day, int SessionsCount);
    private record DateTimeRangePart(DateTime DateTime, bool IsStart);
    
    public IEnumerable<Response> Create(IEnumerable<Request> value)
    {
        var periodPartsPerDates = new Dictionary<DateOnly, LinkedList<DateTimeRangePart>>();
        var startDatesPerDay = new Dictionary<DateOnly, int>();
        var lastDate = DateOnly.MinValue;
        
        foreach (var request in value)
        {
            var startedAtAsDateOnly = DateOnly.FromDateTime(request.StartedAt);
            var endedAtAsDateOnly = DateOnly.FromDateTime(request.EndedAt);

            if (lastDate == DateOnly.MinValue)
            {
                lastDate = startedAtAsDateOnly;
                periodPartsPerDates[startedAtAsDateOnly] = new LinkedList<DateTimeRangePart>();
                periodPartsPerDates[endedAtAsDateOnly] = new LinkedList<DateTimeRangePart>();
            }

            if (startedAtAsDateOnly > lastDate)
            {
                var activity = GetMaxOverlapPerPeriod(startDatesPerDay.GetValueOrDefault(startedAtAsDateOnly), SortDateTimeParts(periodPartsPerDates[lastDate]));

                yield return new Response(startedAtAsDateOnly, activity);

                periodPartsPerDates.Remove(lastDate);
                periodPartsPerDates[startedAtAsDateOnly] = new LinkedList<DateTimeRangePart>();
                
                lastDate = startedAtAsDateOnly;
            }
            
            periodPartsPerDates[startedAtAsDateOnly].AddLast(new DateTimeRangePart(request.StartedAt, true));

            if (!periodPartsPerDates.ContainsKey(endedAtAsDateOnly))
            {
                periodPartsPerDates[endedAtAsDateOnly] = new LinkedList<DateTimeRangePart>();
            }
            
            periodPartsPerDates[endedAtAsDateOnly].AddLast(new DateTimeRangePart(request.EndedAt, false));

            while (startedAtAsDateOnly < endedAtAsDateOnly)
            {
                startedAtAsDateOnly = startedAtAsDateOnly.AddDays(1);

                startDatesPerDay[startedAtAsDateOnly] = startDatesPerDay.GetValueOrDefault(startedAtAsDateOnly) + 1;
            }
        }

        foreach (var key in periodPartsPerDates.Keys)
        {
            var activity = GetMaxOverlapPerPeriod(startDatesPerDay.GetValueOrDefault(key), SortDateTimeParts(periodPartsPerDates[key]));

            yield return new Response(key, activity);
        }
    }

    private int GetMaxOverlapPerPeriod(int startFrom, IReadOnlyCollection<DateTimeRangePart> sortedOverlapPartials)
    {
        var current = startFrom;
        var max = startFrom;
    
        foreach (var part in sortedOverlapPartials)
        {
            if (part.IsStart)
            {
                current++;
            }
            else
            {
                current--;
            }

            if (current > max)
            {
                max = current;
            }
        }

        return max;
    }
    
    private IReadOnlyCollection<DateTimeRangePart> SortDateTimeParts(IEnumerable<DateTimeRangePart> collection)
    {
        var list = collection.ToList();
        
        list.Sort((curr, next) =>
        {
            if (curr.DateTime > next.DateTime) return 1;
            if (curr.DateTime < next.DateTime) return -1;

            return 0;
        });

        return list;
    }
}