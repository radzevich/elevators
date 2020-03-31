using System.Collections.Generic;
using Elevators.FakeExmo.Services;
using Elevators.Providers.Exmo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Elevators.FakeExmo.Controllers
{
    [ApiController]
    public class ExmoApi : Controller
    {
        private readonly DataGenerator _dataGenerator;

        public ExmoApi(DataGenerator dataGenerator)
        {
            _dataGenerator = dataGenerator;
        }

        [HttpGet("/ticker")]
        public Dictionary<string, ExmoExchangeInfo> Ticker()
        {
            return _dataGenerator.GetNext();
        }
    }
}