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
    public class FlightAppModel : Imodel
    {
        Client telnetClient;
        private readonly BlockingCollection<AsyncCommand> _queueCommand;

        public string elevatorCommand = "/controls/flight/elevator";
        public string rudderCommand = "/controls/flight/rudder";
        public string aileronCommand = "/controls/flight/aileron";
        public string throttleCommand = "/controls/engines/current-engine/throttle";


        public FlightAppModel(string ip, int port)
        {
            _queueCommand = new BlockingCollection<AsyncCommand>();
            this.telnetClient = new Client(ip, port);
            this.telnetClient.Connect();
            this.telnetClient.SetTimeOutRead(10000);
            start();

        }
        Task<ResponseCode> Imodel.Execute(Command cmd)
        {
            var asyncCommand = new AsyncCommand(cmd);
            _queueCommand.Add(asyncCommand);
            return asyncCommand.Task;
        }
        public void Run(string ip, int port)
        {
            // Set ip and port.
            this.telnetClient = new Client(ip, port);
            // Connect to the simulator.
            this.Connect();
            // Set time out to 10 seconds.
            this.telnetClient.SetTimeOutRead(10000);

            start();
        }
        public void Connect()
        {
            try
            {
                // Try connect.
                telnetClient.Connect();
                this.telnetClient.Write("data\n");
            }
            catch (Exception e)
            {
                // We couldn't connect.
                if (e.Message == "not connected")
                {
                    Console.WriteLine("not connected");
                }

            }
        }
        public void Disconnect()
        {
            // Stop the connection with the simulator.
            telnetClient.Disconnect();
        }
        public void start()
        {
            Task.Factory.StartNew(ProcessCommand);
        }

        private void ProcessCommand()
        {
/*            if (!FlightMobile.model.IsConnect())
            {
                return;
            }*/
            NetworkStream stream = telnetClient.getTcpClient().GetStream();
            foreach (AsyncCommand command in _queueCommand.GetConsumingEnumerable())
            {
                ResponseCode result;

                result = this.send(elevatorCommand, command.Command.Elevator);
                if (result != ResponseCode.Ok)
                {
                    command.Completion.SetResult(result);
                    continue;
                }
                result = this.send(rudderCommand, command.Command.Rudder);
                if (result != ResponseCode.Ok)
                {
                    command.Completion.SetResult(result);
                    continue;

                }
                result = this.send(aileronCommand, command.Command.Aileron);
                if (result != ResponseCode.Ok)
                {
                    command.Completion.SetResult(result);
                    continue;

                }
                result = this.send(throttleCommand, command.Command.Throttle);
                if (result != ResponseCode.Ok)
                {
                    command.Completion.SetResult(result);
                    continue;

                }

                command.Completion.SetResult(ResponseCode.Ok);
            }

        }

        //execptions:
        //0 = "OK".
        //1 = "Timeout of getting a result from the FlightGear"
        //2 = "The fightgear has been disconnected"
        //3 = "The connection with the flightgear has been lost"
        //4 =

        public ResponseCode send(string data, double value)
        {
            try
            {
                telnetClient.Write("set " + data + " " + value + "\r\n");
                telnetClient.Write("get " + data + "\r\n");

                if (Double.Parse(telnetClient.Read()) != value)
                {
                    return ResponseCode.FailUpdate;
                }
            }
            catch (IOException e)
            {
                if (e.ToString().Contains("A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond."))
                {
                    return ResponseCode.Timeout;
                }
                else
                {
                    return ResponseCode.Disconnected;

                }
            }
            catch (Exception)
            {
                if (!telnetClient.IsConnect())
                {
                    return ResponseCode.ConnectionLost;
                }
                return ResponseCode.NotOk;
            }
            return ResponseCode.Ok;
        }
        public bool IsConnect()
        {
            return this.telnetClient.IsConnect();
        }


    }
}