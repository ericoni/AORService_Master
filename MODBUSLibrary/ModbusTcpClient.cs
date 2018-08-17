using FTN.Common.SCADA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MODBUSLibrary
{
	public class ModbusTcpClient
	{
		private int protocolId = 0x0000;
		private int port = 502; // 502 - Easy modbus  4000 - Atlaga
		private int headerSize = 7;
		private int requestMessageSize = 12;
		private static int transactionId = 0;
		private TcpClient tcpClient;
		private Stream stream;

		public ModbusTcpClient()
		{
			tcpClient = new TcpClient();
		}

		/// <summary>
		/// Method to write single digital value.
		/// </summary>
		public void WriteSingleCoil(int registerAddress, short value)
		{   
            byte[] messageToSend = new byte[requestMessageSize];
			Array.Clear(messageToSend, 0, messageToSend.Length);

			WriteHelper(registerAddress, value, 0x05, messageToSend, true);

			SendData(messageToSend);
        }

		/// <summary>
		/// Method to write single analog value. (old)
		/// </summary>
		public void WriteSingleHoldingRegister(int registerAddress, short value)
		{
			byte[] messageToSend = new byte[requestMessageSize];
			Array.Clear(messageToSend, 0, messageToSend.Length);

			WriteHelper(registerAddress, value, 0x06, messageToSend, false);

			SendData(messageToSend);
		}

		/// <summary>
		/// Method to write single analog value.
		/// </summary>
		public void WriteSingleHoldingRegister2(int registerAddress, float value)
		{
			byte[] messageToSend = new byte[requestMessageSize];
			Array.Clear(messageToSend, 0, messageToSend.Length);

			byte[] response = new byte[requestMessageSize];
			Array.Clear(response, 0, messageToSend.Length);

			byte[] bytes = BitConverter.GetBytes(value);
			short upper = BitConverter.ToInt16(bytes, 0);
			short lower = BitConverter.ToInt16(bytes, 2);

			BitPacking(0x06, registerAddress++, messageToSend, upper);
			SendData(messageToSend);
			GetData(response);
			BitPacking(0x06, registerAddress, messageToSend, lower);
			SendData(messageToSend);
			GetData(response);
        }

		private void BitPacking(short functionCode, int registerAddress, byte[] messageToSend, short value)
		{
			byte[] transactionIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(transactionId++)));
			byte[] protocolIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)protocolId));
			byte[] lengthArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)0x0001)); // number of following bytes
			byte[] unitId = BitConverter.GetBytes(0x01);  // end of modbus header

			byte[] funCode = BitConverter.GetBytes(functionCode);
			byte[] registerAddressArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)registerAddress));
			byte[] valueArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));

			Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			Buffer.BlockCopy(funCode, 0, messageToSend, 7, 1);
			Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			Buffer.BlockCopy(valueArr, 0, messageToSend, 10, 2);
		}

		/// <summary>
		/// Method to read digital values.
		/// </summary>
		public void ReadDiscreteInputs(int registerAddress, short quantity)
		{
			short counter = 0;
			byte[] messageToSend = new byte[requestMessageSize];
			Array.Clear(messageToSend, 0, messageToSend.Length);

			byte[] transactionIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(transactionId++)));
			byte[] protocolIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)protocolId));
			byte[] lengthArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)0x0001)); // number of following bytes
			byte[] unitId = BitConverter.GetBytes(0x01);  // end of modbus header

			byte[] functionCode = BitConverter.GetBytes(0x02);
			byte[] registerAddressArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)registerAddress));
			byte[] quantityArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)quantity));

			Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			Buffer.BlockCopy(functionCode, 0, messageToSend, 7, 1);
			Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			Buffer.BlockCopy(quantityArr, 0, messageToSend, 10, 2);

			int bytesReceived = (quantity % 8 == 0) ? quantity / 8 : quantity / 8 + 1;
			byte[] response = new byte[headerSize + 2 + bytesReceived];
			Array.Clear(response, 0, response.Length);

			SendData(messageToSend);

			GetData(response);

            for (int i = response.Length - bytesReceived; i < response.Length; i++)
			{
				byte tempResponse = response[i];

				for (int j = 0; j < 8; j++)
				{
					if (counter == quantity)
					{
						return;
					}

					if ((tempResponse & 1) == 1)
					{
						Trace.Write("\nOpened breaker at starting address + " + counter++.ToString());
                    }
					else
					{
						Trace.Write("\nClosed breaker at starting address + " + counter++.ToString());
                    }
					tempResponse >>= 1;
				}
			}
		}

		/// <summary>
		/// Method to read analog values. //TO BE replaced with ReadHoldingRegisters2
		/// </summary>
		public int ReadHoldingRegisters(int registerAddress, short quantity)
		{
			int value = 0;
			byte[] messageToSend = new byte[requestMessageSize];
			Array.Clear(messageToSend, 0, messageToSend.Length);

			byte[] transactionIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(transactionId++)));
			byte[] protocolIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)protocolId));
			byte[] lengthArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)0x0001)); // number of following bytes
			byte[] unitId = BitConverter.GetBytes(0x01);  // end of modbus header

			byte[] functionCode = BitConverter.GetBytes(0x03);
			byte[] registerAddressArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)registerAddress));
			byte[] quantityArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)quantity));

			Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			Buffer.BlockCopy(functionCode, 0, messageToSend, 7, 1);
			Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			Buffer.BlockCopy(quantityArr, 0, messageToSend, 10, 2);

			byte[] response = new byte[headerSize + 2 + quantity * 2];
			Array.Clear(response, 0, response.Length);

			SendData(messageToSend);

			GetData(response);

			for (int i = 9, j = 10; i <= response.Length - 2; i += 2, j += 2)
			{
				value = response[j] | response[i] << 8;  // posle "byte count" u response ( posle 8. bajta) prvi bajt cuva Hi, a sledeci Low od a.value
				Trace.Write("\nAnalog value: " + value.ToString());
			}
			return value;
		}

		/// <summary>
		/// Method to read analog values.
		/// </summary>
		public List<float> ReadHoldingRegisters2(int registerAddress, short quantity)
		{
			byte[] messageToSend = new byte[requestMessageSize];
			float packedFloat = 0;
			byte[] byteArray = null;
			List<float> floatList = new List<float>();
			Array.Clear(messageToSend, 0, messageToSend.Length);

			byte[] transactionIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(transactionId++)));
			byte[] protocolIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)protocolId));
			byte[] lengthArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)0x0001)); // number of following bytes
			byte[] unitId = BitConverter.GetBytes(0x01);  // end of modbus header

			byte[] functionCode = BitConverter.GetBytes(0x03);
			byte[] registerAddressArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)registerAddress));
			byte[] quantityArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(quantity * 2)));

			Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			Buffer.BlockCopy(functionCode, 0, messageToSend, 7, 1);
			Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			Buffer.BlockCopy(quantityArr, 0, messageToSend, 10, 2);

			byte[] response = new byte[headerSize + 2 + quantity * 4];  //byte[] response = new byte[headerSize + 2 + quantity * 2];
			Array.Clear(response, 0, response.Length);

			SendData(messageToSend);

			GetData(response);

			Trace.Write("Values acquired: \n");

            for (int i = 9, j = 10; i <= response.Length - 3; i += 2, j += 2)
			{
				byteArray = new byte[4];

				Buffer.BlockCopy(response, j, byteArray, 0, 1); // zbog endiana se radi konverzija
				Buffer.BlockCopy(response, i, byteArray, 1, 1);

				i += 2;
				j += 2;

				Buffer.BlockCopy(response, j, byteArray, 2, 1);
				Buffer.BlockCopy(response, i, byteArray, 3, 1);

				packedFloat = System.BitConverter.ToSingle(byteArray, 0);
				floatList.Add(packedFloat);
				Trace.Write(packedFloat.ToString() + " " + "\n" );
            }

			return floatList;
		}

		/// <summary>
		///  Connects the client to the default port on the localhost.
		/// </summary>
		public void Connect()
		{
			try
			{
				tcpClient.Connect("127.0.0.1", port);
            }
			catch (Exception e)
			{
                throw new Exception(e.Message);
			}

		}

		/// <summary>
		/// Disposes System.Net.Sockets.TcpClient instance and requests that the underlying
		/// TCP connection be closed.
		/// </summary>
		public void Close()
		{
			stream.Dispose();
			tcpClient.Close();
        }

		private void SendData(byte[] messageToSend)
		{
			try
			{
				stream = tcpClient.GetStream();
				stream.Write(messageToSend, 0, messageToSend.Length);
            }
			catch (Exception e)
			{
				Trace.Write("Error has occurred {0}", e.StackTrace);
            }
		}

		private void GetData(byte[] response)
		{
			try
			{
				stream = tcpClient.GetStream();
				stream.Read(response, 0, response.Length);
            }
			catch (Exception e)
			{
				Trace.Write("Error has occurred {0}", e.StackTrace);
            }
		}

		private void WriteHelper(int registerAddress, short value, short functionCode, byte[] messageToSend, bool isDigital)
		{
			byte[] transactionIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)(transactionId++)));
			byte[] protocolIdArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)protocolId));
			byte[] lengthArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)0x0001)); // number of following bytes
			byte[] unitId = BitConverter.GetBytes(0x01);  // end of modbus header

			byte[] funCode = BitConverter.GetBytes(functionCode);
			byte[] registerAddressArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)registerAddress));
			byte[] valueArr = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));

			if (isDigital)
			{
				byte[] onCommandArr = { 0xff, 0x00 };
				byte[] offCommandArr = { 0x00, 0x00 };
				valueArr = (value == 0) ? offCommandArr : onCommandArr;
			}

			Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			Buffer.BlockCopy(funCode, 0, messageToSend, 7, 1);
			Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			Buffer.BlockCopy(valueArr, 0, messageToSend, 10, 2);
		}

	}
}
