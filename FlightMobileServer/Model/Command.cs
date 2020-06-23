using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightMobileServer.Model
{
    public class Command
    {
        [JsonPropertyName("aileron")]
        public double Aileron { get; set; }
        [JsonPropertyName("rudder")]
        public double Rudder { get; set; }
        [JsonPropertyName("elevator")]
        public double Elevator { get; set; }
        [JsonPropertyName("throttle")]
        public double Throttle { get; set; }
    }
}
