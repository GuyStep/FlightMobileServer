using FlightMobileServer.Model;
using System.Threading.Tasks;

namespace FlightMobileServer.Model
{
    public enum ResponseCode {Ok, NotOk}
    public class AsyncCommand
    {
        public Command Command { get; private set; }

        public Task<ResponseCode> Task { get => Completion.Task; }
        public TaskCompletionSource<ResponseCode> Completion { get; private set; }

        public AsyncCommand(Command command)
        {
            this.Command = command;

            Completion = new TaskCompletionSource<ResponseCode>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}