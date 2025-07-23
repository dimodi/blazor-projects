using Microsoft.AspNetCore.Mvc;
using Telerik.DataSource;
using T1694183.Data;
using T1694183.Services;
using T1694183.Shared.Data;

namespace T1694183.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IProductService ControllerProductService;

    public ProductController(IProductService productService)
    {
        ControllerProductService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Read(DataSourceRequest request)
    {
        DataSourceResult result = await ControllerProductService.GetProductsAsync(request);

        DataEnvelope<Product> dataEnvelope = new()
        {
            Data = result.Data.Cast<Product>()?.ToList() ?? [],
            Total = result.Total
        };

        return new JsonResult(result);
    }
}
