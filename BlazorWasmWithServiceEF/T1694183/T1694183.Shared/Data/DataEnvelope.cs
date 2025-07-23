using Telerik.DataSource;

namespace T1694183.Shared.Data;

public class DataEnvelope<T>
{
    public List<T> Data { get; set; } = new();

    public List<AggregateFunctionsGroup> GroupedData { get; set; } = new();

    public int Total { get; set; }

    public IEnumerable<AggregateResult>? AggregateResults { get; set; }
}
