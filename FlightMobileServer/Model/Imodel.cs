using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FlightMobileServer.Model
{
    public interface Imodel
    {
        public Task<ResponseCode> Execute(Command cmd);
        public bool IsConnect();
    }
}