using System.Collections.Generic;
using System.Threading.Tasks;
using FlightMobileServer.Model;
using Microsoft.AspNetCore.Mvc;

namespace ServerMobileApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private Imodel simulatorModel;
        public CommandsController(Imodel model)
        {
            this.simulatorModel = model;
        }
        // GET: api/Commands
        [HttpGet] 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        } 

        // GET: api/Commands/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Commands
        [HttpPost]
        public async Task<ActionResult<Command>> Post(Command value)
        {
            if (!checkCommand(value))
            {
                return Content("Bad command");
            }

            if (!this.simulatorModel.IsConnect())
            {
                return NotFound("The fightgear is not connected to the server");
            }

            var res = await this.simulatorModel.Execute(value);
            if (res == ResponseCode.Ok)
            {
                return Ok();
            }
            if (res == ResponseCode.Timeout)
            {
                return NotFound("Timeout of getting a result from the FlightGear");

            }
            if (res == ResponseCode.Disconnected)
            {
                return NotFound("The fightgear has been disconnected");

            }
            if (res == ResponseCode.ConnectionLost)
            {
                return NotFound("The connection with the flightgear has been lost");

            }
            if (res == ResponseCode.NotOk)
            {
                return NotFound("other problem");
            }
            if (res == ResponseCode.FailUpdate)
            {
                return NotFound("Failed to update command");
            }
            return NotFound();
        }
        private bool checkCommand(Command value)
        {

            return !(value.Aileron > 1 || value.Aileron < -1 || value.Elevator > 1 || value.Elevator < -1 || value.Throttle > 1 ||
                value.Throttle < 0 || value.Rudder > 1 || value.Rudder < -1);

        }
    }
}