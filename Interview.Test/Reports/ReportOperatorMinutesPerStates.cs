namespace Interview.Test.Reports;

public class ReportOperatorMinutesPerStates: IReport<ReportOperatorMinutesPerStates.Request, ReportOperatorMinutesPerStates.Response>
{
    public record Request(string Operator, string State, TimeSpan TimeInState);
    public record Response(string Operator, IEnumerable<TimeSpan> TimePerStates);
    private record struct OperatorInState(string Operator, string State);

    public IEnumerable<Response> Create(IEnumerable<Request> value)
    {
        var states = new HashSet<string>();
        var operators = new HashSet<string>();
        var stateTimesPerOperators = new Dictionary<OperatorInState, TimeSpan>();

        foreach (var request in value)
        {
            states.Add(request.State);
            operators.Add(request.Operator);
            
            var operatorInState = new OperatorInState(request.Operator, request.State);

            if (stateTimesPerOperators.TryGetValue(operatorInState, out var timeInState))
            {
                stateTimesPerOperators[operatorInState] = timeInState.Add(request.TimeInState);
                
                continue;
            }

            stateTimesPerOperators[operatorInState] = request.TimeInState;
        }

        foreach (var @operator in operators)
        {
            var parts = states
                .Select(state => new OperatorInState(@operator, state))
                .Select(key => stateTimesPerOperators.TryGetValue(key, out var perOperator) 
                    ? perOperator 
                    : TimeSpan.Zero);

            yield return new Response(@operator, parts);
        }
    }
}