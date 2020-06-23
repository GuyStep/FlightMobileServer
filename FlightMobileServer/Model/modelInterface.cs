using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FlightMobileServer.Model
{
    public interface modelInterface
    {
        public Task<ResponseCode> Execute(Command cmd);
        public bool IsConnected();
    }
}