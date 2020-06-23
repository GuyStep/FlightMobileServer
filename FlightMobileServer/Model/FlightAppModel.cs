using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.Http;


namespace FlightMobileServer.Model
{
    public class FlightAppModel : modelInterface
    {
        Client telnetClient;
        private readonly BlockingCollection<AsyncCommand> commandsQueue;

        public string stringElevator = "/controls/flight/elevator";
        public string stringRudder = "/controls/flight/rudder";
        public string stringAileron = "/controls/flight/aileron";
        public string stringThrottle = "/controls/engines/current-engine/throttle";


        public FlightAppModel(string ip, int port)
        {
            commandsQueue = new BlockingCollection<AsyncCommand>();
            this.telnetClient = new Client(ip, port);
            this.telnetClient.Connect();
            this.telnetClient.SetTimeOutRead(10000);
            Task.Factory.StartNew(ProcessCommand);
        }
        Task<ResponseCode> modelInterface.Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            commandsQueue.Add(asyncCommand);
            return asyncCommand.Task;
        }
        public void Run(string ip, int port)
        {
            this.telnetClient = new Client(ip, port);
            this.Connect();
            this.telnetClient.SetTimeOutRead(10000);
            Task.Factory.StartNew(ProcessCommand);
        }
        public void Connect()
        {
            try
            {
                telnetClient.Connect();
                this.telnetClient.Write("data\n"); //Cancel ordinary prompt
            }
            catch
            {

                Console.WriteLine("Problem with sending message to simulator");
            }
        }
        public void Disconnect()
        {
            telnetClient.Disconnect();
        }

        private void ProcessCommand()
        {
            NetworkStream stream = telnetClient.getTcpClient().GetStream();
            foreach (AsyncCommand command in commandsQueue.GetConsumingEnumerable())
            {
                string[] commands = new string[] { stringElevator, stringThrottle, stringAileron, stringRudder };
                double[] values = new double[] { command.Command.Elevator, command.Command.Throttle, command.Command.Aileron, command.Command.Rudder };
                ResponseCode resultCode;
                for (int i = 0; i<4; i++)
                {
                    resultCode = this.send(commands[i], values[i]);
                    if (resultCode != ResponseCode.Ok)
                    {
                        command.Completion.SetResult(resultCode);
                        break;
                    }
                }
                try
                {
                    command.Completion.SetResult(ResponseCode.Ok);
                } catch
                {
                    continue; //In case completion status was in the exact moment when simulator falls/dc
                }
            }

        }

        public ResponseCode send(string data, double value)
        {
            try
            {
                telnetClient.Write("set " + data + " " + value + "\r\n");
                telnetClient.Write("get " + data + "\r\n");

                if (Double.Parse(telnetClient.Read()) != value)
                {
                    return ResponseCode.NotOk;
                }
            }
            catch {
                return ResponseCode.NotOk;
            }
            return ResponseCode.Ok;
        }
        public bool IsConnected()
        {
            return this.telnetClient.IsConnected();
        }
    }
}