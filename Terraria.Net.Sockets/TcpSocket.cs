using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Terraria.Net.Sockets
{
	public class TcpSocket : ISocket
	{
		private TcpClient _connection;
		private TcpListener _listener;
		private SocketConnectionAccepted _listenerCallback;
		private RemoteAddress _remoteAddress;
		private bool _isListening;
		public TcpSocket()
		{
			this._connection = new TcpClient();
			this._connection.NoDelay = true;
		}
		public TcpSocket(TcpClient tcpClient)
		{
			this._connection = tcpClient;
			this._connection.NoDelay = true;
			IPEndPoint iPEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
			this._remoteAddress = new TcpAddress(iPEndPoint.Address, iPEndPoint.Port);
		}
		void ISocket.Close()
		{
			this._remoteAddress = null;
			this._connection.Close();
		}
		bool ISocket.IsConnected()
		{
			return this._connection != null && this._connection.Client != null && this._connection.Connected;
		}
		void ISocket.Connect(RemoteAddress address)
		{
			TcpAddress tcpAddress = (TcpAddress)address;
			this._connection.Connect(tcpAddress.Address, tcpAddress.Port);
			this._remoteAddress = address;
		}
		private void ReadCallback(IAsyncResult result)
		{
			Tuple<SocketReceiveCallback, object> tuple = (Tuple<SocketReceiveCallback, object>)result.AsyncState;
			tuple.Item1(tuple.Item2, this._connection.GetStream().EndRead(result));
		}
		private void SendCallback(IAsyncResult result)
		{
			Tuple<SocketSendCallback, object> tuple = (Tuple<SocketSendCallback, object>)result.AsyncState;
			try
			{
				this._connection.GetStream().EndWrite(result);
				tuple.Item1(tuple.Item2);
			}
			catch (Exception)
			{
				((ISocket)this).Close();
			}
		}
		void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
		{
			this._connection.GetStream().BeginWrite(data, 0, size, new AsyncCallback(this.SendCallback), new Tuple<SocketSendCallback, object>(callback, state));
		}
		void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
		{
			this._connection.GetStream().BeginRead(data, offset, size, new AsyncCallback(this.ReadCallback), new Tuple<SocketReceiveCallback, object>(callback, state));
		}
		bool ISocket.IsDataAvailable()
		{
			return this._connection.GetStream().DataAvailable;
		}
		RemoteAddress ISocket.GetRemoteAddress()
		{
			return this._remoteAddress;
		}
		bool ISocket.StartListening(SocketConnectionAccepted callback)
		{
			IPAddress any = IPAddress.Any;
			string ipString;
			if (Program.LaunchParameters.TryGetValue("-ip", out ipString) && !IPAddress.TryParse(ipString, out any))
			{
				any = IPAddress.Any;
			}
			this._isListening = true;
			this._listenerCallback = callback;
			if (this._listener == null)
			{
				this._listener = new TcpListener(any, Netplay.ListenPort);
			}
			try
			{
				this._listener.Start();
			}
			catch (Exception)
			{
				return false;
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenLoop));
			return true;
		}
		void ISocket.StopListening()
		{
			this._isListening = false;
		}
		private void ListenLoop(object unused)
		{
			while (this._isListening && !Netplay.disconnect)
			{
				try
				{
					ISocket socket = new TcpSocket(this._listener.AcceptTcpClient());
					Console.WriteLine(socket.GetRemoteAddress() + " is connecting...");
					this._listenerCallback(socket);
				}
				catch (Exception)
				{
				}
			}
			this._listener.Stop();
		}
	}
}
