using ClassLibrary;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ODataProject.Controllers
{
    public class SampleModelsController : ODataController
    {
        private static List<SampleModel> Data { get; set; } = new();

        [EnableCors]
        [EnableQuery]
        public ActionResult<IEnumerable<SampleModel>> Get()
        {
            return Ok(Data);
        }

        [EnableCors]
        [EnableQuery]
        public ActionResult<SampleModel> Get([FromRoute] int key)
        {
            var item = Data.SingleOrDefault(d => d.Id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        public SampleModelsController()
        {
            if (Data.Count > 0)
            {
                return;
            }

            Random rnd = Random.Shared;

            Data = new();

            for (int i = 1; i <= 24; i++)
            {
                Data.Add(new SampleModel()
                {
                    Id = i,
                    BoolProperty = i % 2 == 0,
                    CharProperty = (char)rnd.Next(97, 123),
                    DateOnlyProperty = new DateOnly(rnd.Next(2000, 2030), rnd.Next(1, 13), rnd.Next(1, 29)),
                    DateTimeProperty = new DateTime(rnd.Next(2000, 2030), rnd.Next(1, 13), rnd.Next(1, 29), rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(1, 60)),
                    DecimalProperty = rnd.Next(0, 10000) * 1.23m,
                    EnumProperty = (SampleEnum)rnd.Next(1, 4),
                    IntProperty = rnd.Next(0, 10000),
                    StringProperty = $"String {(char)rnd.Next(97, 123)}{(char)rnd.Next(97, 123)}{(char)rnd.Next(97, 123)} {i}",
                    TimeOnlyProperty = new TimeOnly(rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60))
                });
            }
        }
    }
}
