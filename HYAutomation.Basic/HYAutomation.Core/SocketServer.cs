using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HYAutomation.Core
{

    public class SocketServer
    {
        private static TcpListener _tcpServer;
        private static bool isRunning = false;
        public static event EventHandler<string> HasNewMsgEvent;
        public static event EventHandler ClientOfflineEvent;
        private static readonly List<TcpClient> tcpClients = new List<TcpClient>();
        private static Queue<TcpClient> removeQueue = new Queue<TcpClient>();
        private static Task _monitorTask;
        public static void Open(IPAddress iPAddress, int port)
        {
            try
            {
                _tcpServer = new TcpListener(iPAddress, port);
                if (!isRunning)
                {
                    if (_tcpServer != null)
                    {
                        isRunning = true;
                        if (_monitorTask == null || _monitorTask.IsCompleted)
                        {
                            _monitorTask = Task.Factory.StartNew(
                                () =>
                                {
                                    while (isRunning)
                                    {
                                        checkClientsStatus();
                                        System.Threading.Thread.Sleep(2000);
                                    }
                                }, TaskCreationOptions.LongRunning);
                        }
                        _tcpServer.Start();
                        _tcpServer.BeginAcceptSocket(new AsyncCallback(HasNewClient), _tcpServer);
                    }
                    else
                    {
                        isRunning = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _tcpServer = null;
                Console.WriteLine(ex.ToString());
            }
        }
        public static void Close()
        {
            if (_tcpServer != null)
            {
                try
                {
                    _tcpServer.Stop();
                    foreach (var item in tcpClients)
                    {
                        item.Close();
                    }
                    tcpClients.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        private static void HasNewClient(IAsyncResult asyncResult)
        {
            try
            {
                TcpListener listener = (TcpListener)asyncResult.AsyncState;
                TcpClient tcpClient = listener.EndAcceptTcpClient(asyncResult);
                tcpClient.Client.IOControl(IOControlCode.KeepAliveValues, GetKeepAliveData(), null);
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                tcpClients.Add(tcpClient);
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffRece = new byte[tcpClient.ReceiveBufferSize];
                networkStream.BeginRead(buffRece, 0, buffRece.Length, HasNewMsg, new KeyValuePair<byte[], NetworkStream>(buffRece, networkStream));
                _tcpServer.BeginAcceptSocket(new AsyncCallback(HasNewClient), _tcpServer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void HasNewMsg(IAsyncResult asyncResult)
        {
            try
            {
                KeyValuePair<byte[], NetworkStream> keyValue = (KeyValuePair<byte[], NetworkStream>)asyncResult.AsyncState;
                byte[] buffRece = keyValue.Key;
                NetworkStream networkStream = keyValue.Value;
                int recv = 0;
                try
                {
                    recv = networkStream.EndRead(asyncResult);
                }
                catch
                {
                }
                if (recv > 0)
                {
                    string message = Encoding.UTF8.GetString(buffRece, 0, recv);
                    HasNewMsgEvent?.BeginInvoke(networkStream, message, null, null);
                    Console.WriteLine($"收到客户端消息【{message}】");
                    networkStream.BeginRead(buffRece, 0, buffRece.Length, HasNewMsg, keyValue);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            System.Threading.Thread.Sleep(10);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SendData(string msg)
        {
            try
            {
                if (tcpClients.Count < 1) return false;
                foreach (var item in tcpClients)
                {
                    NetworkStream network = item.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    network.Write(buffer, 0, buffer.Length);
                    //var bw = new System.IO.BinaryWriter(item.GetStream());
                    //bw.Write(msg);
                }
                Console.WriteLine($"发送给客户端指令【{msg}】");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        private static void checkClientsStatus()
        {
            try
            {
                lock (tcpClients)
                {
                    foreach (var item in tcpClients)
                    {
                        //if (item.Client.Client.Poll(500, System.Net.Sockets.SelectMode.SelectRead) && (item.Client.Client.Available == 0))
                        if (!isClientConnected(item))
                        {
                            removeQueue.Enqueue(item);
                            ClientOfflineEvent?.Invoke(item, null);
                            continue;
                        }
                    }
                    while (removeQueue.Count > 0)
                    {
                        TcpClient item = removeQueue.Dequeue();
                        tcpClients.Remove(item);
                        try
                        {
                            item.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("关闭客户端连接" + ex.ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static bool isClientConnected(TcpClient ClientSocket)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in tcpConnections)
            {
                TcpState stateOfConnection = c.State;

                if (c.LocalEndPoint.Equals(ClientSocket.Client.LocalEndPoint) && c.RemoteEndPoint.Equals(ClientSocket.Client.RemoteEndPoint))
                {
                    if (stateOfConnection == TcpState.Established)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }

            return false;
        }

        private static byte[] GetKeepAliveData()
        {
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)500).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            return inOptionValues;
        }
    }
}
