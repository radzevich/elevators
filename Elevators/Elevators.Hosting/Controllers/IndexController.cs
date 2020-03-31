using System;
using Elevators.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elevators.Hosting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : Controller
    {
        private readonly ITradeService _tradeService;

        public IndexController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }
        
        [HttpGet("start")]
        public IActionResult Start()
        {
            try
            {
                _tradeService.Start();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("pause")]
        public IActionResult Pause()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}