using KNXLib.Universal.Utils;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KNXLib.Universal
{
    public class KnxTunneling : KnxBase
    {
        private int _timerPeriod = 60000;
        private byte _rxSequenceNumber;
        private byte _sequenceNumber;

        public byte ChannelId { get; private set; }
        public object SequenceNumberLock { get; private set; }
        private Timer StateRequestTimer { get; set; }
        private UdpSocketReceiver UdpClient { get; set; }

        public KnxTunneling(string remoteIpAddress, int remotePort, string localIpAddress, int localPort)
        {
            LocalEndpoint = new IPEndPoint(IPAddress.Parse(localIpAddress), localPort);
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpAddress), remotePort);
            ChannelId = 0x00;
            SequenceNumberLock = new object();
            StateRequestTimer = new Timer(OnStateRequest, null, Timeout.Infinite, _timerPeriod);
        }

        public async void Connect()
        {
            try
            {
                if (UdpClient != null)
                {
                    try
                    {
                        await UdpClient.StopListeningAsync();
                        UdpClient.Dispose();
                    }
                    catch(Exception ex)
                    {
                        if (IsDebug)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                }

                UdpClient = new UdpSocketReceiver();
                UdpClient.MessageReceived += OnMessageReceived;
            }
            catch (SocketException ex)
            {
                if (IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return;
                }
            }

            await UdpClient.StartListeningAsync(LocalEndpoint.Port);

            try
            {
                ConnectRequest();
            }
            catch(Exception ex)
            {
                if (IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void OnMessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
        {
            try
            {
                switch (Networking.GetServiceType(e.ByteData))
                {
                    case ServiceType.ConnectionResponse:
                        ProcessConnectResponse(e.ByteData);
                        break;
                    case ServiceType.ConnectionStateResponse:
                        ProcessConnectionStateResponse(e.ByteData);
                        break;
                    case ServiceType.TunnelingAck:
                        ProcessTunnelingAck(e.ByteData);
                        break;
                    case ServiceType.DisconnectRequest:
                        ProcessDisconnectRequest(e.ByteData);
                        break;
                    case ServiceType.TunnelingRequest:
                        ProcessDatagramHeaders(e.ByteData);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private async void OnStateRequest(object state)
        {
            var datagram = DatagramProcessing.CreateStateRequest(LocalEndpoint, ChannelId);

            try
            {
                await SendData(datagram);
            }
            catch(Exception ex)
            {
                if(IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private async void ConnectRequest()
        {
            var datagram = DatagramProcessing.CreateConnectRequest(LocalEndpoint);

            await SendData(datagram);
        }

        private async void DisconnectRequest()
        {
            var datagram = DatagramProcessing.CreateDisconnectRequest(LocalEndpoint, ChannelId);

            await SendData(datagram);
        }

        public async Task<bool> SendData(byte[] datagram)
        {
            try
            {
                await UdpClient.SendToAsync(datagram, RemoteEndPoint.Address.ToString(), RemoteEndPoint.Port);
                return true;
            }
            catch(Exception ex)
            {
                if(IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                return false;
            }
        }

        public async void SendTunnelingAck(byte sequenceNumber)
        {
            // HEADER
            var datagram = DatagramProcessing.CreateTunnelingAck(sequenceNumber, ChannelId); 

            await SendData(datagram);
        }

        public void Disconnect()
        {
            try
            {
                TerminateStateRequest();
                DisconnectRequest();
            }
            catch(Exception ex)
            {
                if (IsDebug)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }            
        }

        private void InitializeStateRequest()
        {
            StateRequestTimer.Change(0, _timerPeriod);
        }

        private void TerminateStateRequest()
        {
            StateRequestTimer.Change(Timeout.Infinite, _timerPeriod);
        }

        ~KnxTunneling()
        {
            StateRequestTimer.Dispose();
        }

        private void ProcessDatagramHeaders(byte[] datagram)
        {
            // HEADER
            // TODO: Might be interesting to take out these magic numbers for the datagram indices
            var knxDatagram = new KnxDatagram
            {
                HeaderLength = datagram[0],
                ProtocolVersion = datagram[1],
                ServiceType = new[] { datagram[2], datagram[3] },
                TotalLength = datagram[4] + datagram[5]
            };

            var channelId = datagram[7];
            if (channelId != ChannelId)
                return;

            var sequenceNumber = datagram[8];
            var process = true;
            lock (SequenceNumberLock)
            {
                if (sequenceNumber <= _rxSequenceNumber)
                    process = false;

                _rxSequenceNumber = sequenceNumber;
            }

            if (process)
            {
                // TODO: Magic number 10, what is it?
                var cemi = new byte[datagram.Length - 10];
                Array.Copy(datagram, 10, cemi, 0, datagram.Length - 10);

                switch(DatagramProcessing.ProcessCEMI(ref knxDatagram, cemi, ThreeLevelGroupAddressing, IsDebug))
                {
                    case ResponseType.Event:
                        base.EventReceived(knxDatagram.DestinationAddress, knxDatagram.Data);
                        break;
                    case ResponseType.Status:
                        base.StatusReceived(knxDatagram.DestinationAddress, knxDatagram.Data);
                        break;
                }
            }

            SendTunnelingAck(sequenceNumber);
        }

        private async void ProcessDisconnectRequest(byte[] datagram)
        {
            var channelId = datagram[6];
            if (channelId != ChannelId)
                return;

            await UdpClient.StopListeningAsync();
            UdpClient.Dispose();

            base.Disconnected();
        }

        private void ProcessTunnelingAck(byte[] datagram)
        {
            // do nothing
        }

        private void ProcessConnectionStateResponse(byte[] datagram)
        {
            // HEADER
            // 06 10 02 08 00 08 -- 48 21
            var knxDatagram = new KnxDatagram
            {
                HeaderLength = datagram[0],
                ProtocolVersion = datagram[1],
                ServiceType = new[] { datagram[2], datagram[3] },
                TotalLength = datagram[4] + datagram[5],
                ChannelID = datagram[6]
            };

            var response = datagram[7];

            if (response != 0x21)
                return;

            if (IsDebug)
                System.Diagnostics.Debug.WriteLine("KnxReceiverTunneling: Received connection state response - No active connection with channel ID {0}", knxDatagram.ChannelID);

            Disconnect();
        }

        private void ProcessConnectResponse(byte[] datagram)
        {
            // HEADER
            var knxDatagram = new KnxDatagram
            {
                HeaderLength = datagram[0],
                ProtocolVersion = datagram[1],
                ServiceType = new[] { datagram[2], datagram[3] },
                TotalLength = datagram[4] + datagram[5],
                ChannelID = datagram[6],
                Status = datagram[7]
            };

            if (knxDatagram.ChannelID == 0x00 && knxDatagram.Status == 0x24)
            {
                if (IsDebug)
                    System.Diagnostics.Debug.WriteLine("KnxReceiverTunneling: Received connect response - No more connections available");
            }
            else
            {
                ChannelId = knxDatagram.ChannelID;
                ResetSequenceNumber();

                InitializeStateRequest();
                base.Connected();
            }
        }

        public byte GenerateSequenceNumber()
        {
            return _sequenceNumber++;
        }

        public void RevertSingleSequenceNumber()
        {
            _sequenceNumber--;
        }

        public void ResetSequenceNumber()
        {
            _sequenceNumber = 0x00;
        }

        protected async override void SendAction(string destinationAddress, byte[] data)
        {
            await SendData(CreateActionDatagram(destinationAddress, data));
        }

        protected async override void SendRequestStatus(string destinationAddress)
        {
            await SendData(CreateRequestStatusDatagram(destinationAddress));
        }

        private byte[] CreateActionDatagram(string destinationAddress, byte[] data)
        {
            lock (SequenceNumberLock)
            {
                byte newSequenceNumber = GenerateSequenceNumber();
                try
                {
                    var dataLength = DataProcessing.GetDataLength(data);

                    // HEADER
                    var datagram = new byte[10];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;

                    var totalLength = BitConverter.GetBytes(dataLength + 20);
                    datagram[04] = totalLength[1];
                    datagram[05] = totalLength[0];

                    datagram[06] = 0x04;
                    datagram[07] = ChannelId;
                    datagram[08] = newSequenceNumber;
                    datagram[09] = 0x00;

                    return DatagramProcessing.CreateActionDatagramCommon(destinationAddress, data, datagram, ActionMessageCode);
                }
                catch
                {
                    RevertSingleSequenceNumber();

                    return null;
                }
            }
        }

        private byte[] CreateRequestStatusDatagram(string destinationAddress)
        {
            lock (SequenceNumberLock)
            {
                byte newSequenceNumber = GenerateSequenceNumber();
                try
                {
                    // HEADER
                    var datagram = new byte[21];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;
                    datagram[04] = 0x00;
                    datagram[05] = 0x15;

                    datagram[06] = 0x04;
                    datagram[07] = ChannelId;
                    datagram[08] = newSequenceNumber;
                    datagram[09] = 0x00;

                    return DatagramProcessing.CreateRequestStatusDatagramCommon(destinationAddress, datagram, 10, ActionMessageCode);
                }
                catch
                {
                    RevertSingleSequenceNumber();

                    return null;
                }
            }
        }
    }
}
