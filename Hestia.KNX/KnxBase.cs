using KNXLib.Universal.DPT;
using KNXLib.Universal.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace KNXLib.Universal
{
    public abstract class KnxBase
    {
        private readonly KnxLockManager _lockManager = new KnxLockManager();

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string, string> OnStatus;
        public event Action<string, string> OnEvent;

        public bool IsDebug { get; set; }
        public bool ThreeLevelGroupAddressing { get; set; }
        public byte ActionMessageCode { get; set; }
        public IPEndPoint RemoteEndPoint { get; protected set; }
        protected IPEndPoint LocalEndpoint { get; set; }

        public KnxBase()
        {
            IsDebug = false;
            ThreeLevelGroupAddressing = true;
            ActionMessageCode = 0x00;
        }

        protected abstract void SendAction(string destinationAddress, byte[] data);

        protected abstract void SendRequestStatus(string destinationAddress);

        #region DataPoint Conversions
        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, string data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, byte[] data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, string value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, object value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }
        #endregion

        protected virtual void Connected()
        {
            if (OnConnected != null)
                OnConnected();

            _lockManager.UnlockConnection();
        }

        protected virtual void Disconnected()
        {
            _lockManager.LockConnection();

            if (OnDisconnected != null)
                OnDisconnected();
        }

        protected virtual void EventReceived(string address, string data)
        {
            if (OnEvent != null)
                OnEvent(address, data);
        }

        protected virtual void StatusReceived(string address, string data)
        {
            if (OnStatus != null)
                OnStatus(address, data);
        }

        #region Actions
        public void Action(string address, bool data)
        {
            byte[] val;

            try
            {
                val = new[] { Convert.ToByte(data) };
            }
            catch
            {
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val);
        }

        public void Action(string address, string data)
        {
            byte[] val;
            try
            {
                val = Encoding.ASCII.GetBytes(data);
            }
            catch
            {
                throw new InvalidKnxDataException(data);
            }

            if (val == null)
                throw new InvalidKnxDataException(data);

            Action(address, val);
        }

        public void Action(string address, int data)
        {
            var val = new byte[2];
            if (data <= 255)
            {
                val[0] = 0x00;
                val[1] = (byte)data;
            }
            else if (data <= 65535)
            {
                val[0] = (byte)data;
                val[1] = (byte)(data >> 8);
            }
            else
            {
                // allowing only positive integers less than 65535 (2 bytes), maybe it is incorrect...???
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val);
        }

        public void Action(string address, byte data)
        {
            Action(address, new byte[] { 0x00, data });
        }

        public void Action(string address, byte[] data)
        {
            _lockManager.PerformLockedOperation(() => SendAction(address, data));
        }
        #endregion

        public void RequestStatus(string address)
        {
            _lockManager.PerformLockedOperation(() => SendRequestStatus(address));
        }
    }
}
