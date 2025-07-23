using Telerik.DataSource;

namespace T1694183.Services;

public interface IProductService
{
    public Task<DataSourceResult> GetProductsAsync(DataSourceRequest request);
}
