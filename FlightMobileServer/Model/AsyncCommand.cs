using FlightMobileServer.Model;
using System.Threading.Tasks;

namespace FlightMobileServer.Model
{
    public enum ResponseCode { Ok, NotOk, Timeout, Disconnected, ConnectionLost, FailUpdate }
    public class AsyncCommand
    {
        public Command Command { get; private set; }

        public Task<ResponseCode> Task { get => Completion.Task; }
        public TaskCompletionSource<ResponseCode> Completion { get; private set; }

        public AsyncCommand(Command cmd)
        {
            this.Command = cmd;

            Completion = new TaskCompletionSource<ResponseCode>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}