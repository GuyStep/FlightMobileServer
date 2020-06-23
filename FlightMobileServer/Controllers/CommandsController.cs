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
        private modelInterface simulatorModel;
        public CommandsController(modelInterface model)
        {
            this.simulatorModel = model;
        }

        // POST: api/Commands
        [HttpPost]
        public async Task<ActionResult<Command>> Post(Command value)
        {
            if (!checkCommand(value))
            {
                return Content("Command is not within constraints");
            }

            if (!this.simulatorModel.IsConnected())
            {
                return NotFound("No connection to simulator found");
            }

            var res = await this.simulatorModel.Execute(value);
            if (res == ResponseCode.Ok)
            {
                return Ok();
            }           
            return NotFound("Problem with receiving answer from simulator");
        }
        private bool checkCommand(Command value)
        {
            return !(value.Aileron > 1 || value.Aileron < -1 || value.Elevator > 1 || value.Elevator < -1 || value.Throttle > 1 ||
                value.Throttle < 0 || value.Rudder > 1 || value.Rudder < -1);
        }
    }
}