namespace Interview.Test.Reports;

public interface IReport<in TRequest, out TResponse>
{
    IEnumerable<TResponse> Create(IEnumerable<TRequest> value);
}