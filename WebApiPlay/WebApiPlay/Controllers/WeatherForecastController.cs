using Microsoft.AspNetCore.Mvc;
using WebApiPlay.Models;

namespace WebApiPlay.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    private static List<WeatherForecast> _data { get; set; } = new();

    private static int _lastId { get; set; }

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        if (_data.Count == 0)
        {
            _data = Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Id = ++_lastId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToList();
        }
    }

    [HttpPut]
    public IActionResult Create(WeatherForecast newItem)
    {
        newItem.Id = ++_lastId;
        _data.Add(newItem);

        return new CreatedResult();
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Read()
    {
        return _data;
    }

    [HttpPost]
    public IActionResult Update(WeatherForecast updatedItem)
    {
        var originalIndex = _data.FindIndex(x => x.Id == updatedItem.Id);

        if (originalIndex >= 0)
        {
            _data[originalIndex] = updatedItem;

            return Ok();
        }
        else
        {
            return new UnprocessableEntityResult();
        }
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var originalIndex = _data.FindIndex(x => x.Id == id);

        if (originalIndex >= 0)
        {
            _data.RemoveAt(originalIndex);

            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}
