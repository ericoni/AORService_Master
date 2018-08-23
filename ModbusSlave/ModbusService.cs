using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusSlave
{
   //         Buffer.BlockCopy(transactionIdArr, 0, messageToSend, 0, 2);
			//Buffer.BlockCopy(protocolIdArr, 0, messageToSend, 2, 2);
			//Buffer.BlockCopy(lengthArr, 0, messageToSend, 4, 2);
			//Buffer.BlockCopy(unitId, 0, messageToSend, 6, 1);

			//Buffer.BlockCopy(funCode, 0, messageToSend, 7, 1);
			//Buffer.BlockCopy(registerAddressArr, 0, messageToSend, 8, 2);
			//Buffer.BlockCopy(valueArr, 0, messageToSend, 10, 2);
	public class ModbusService
	{
		private const short transactionIdArrayLength = 2;
		private const short protocolIdArrayLength = 2;
		private const short lengthArrayLength = 2;
		private const short unitIdLength = 1;
		private const short headerLength = transactionIdArrayLength + protocolIdArrayLength + lengthArrayLength + unitIdLength;
		private const short functionCodeLength = 1;
		private const short startingAddressLength = 2;
		private const short quantityOfCoilsLength = 2;
		private const short countByteLength = 1;
		private const short registerValueLength = 2;
		private const short exceptionCodeLength = 1;

		private TcpListener listener = null;
		private Dictionary<int, short> holdingRegisters = null;
		private IPAddress address = null;
		private int port;

		private static object lockObj = new object();
		private static object functionLock = new object();

		public ModbusService(string ipAddress, int port)
		{
			this.address = IPAddress.Parse(ipAddress);
			this.port = port;

			this.listener = new TcpListener(this.address, this.port);

			InitializeHoldingRegisters();
		}

		/// <summary>
		/// Initialize of holding registers
		/// </summary>
		public void InitializeHoldingRegisters()
		{
			holdingRegisters = new Dictionary<int, short>();

			for (int i = 0; i < UInt16.MaxValue; i++)
			{
				holdingRegisters.Add(i, 0);
			}
		}

		/// <summary>
		/// Start listener
		/// </summary>
		public void Start()
		{
			try
			{
				listener.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Start accept client requests
		/// </summary>
		public void Run()
		{
			while (true)
			{
				Console.WriteLine("Waiting for client connection...");
				Socket client = listener.AcceptSocket();

				// Novi client novi thread koji ga opsluzuje
				Thread thread = new Thread(() => ObserveClient(client));
				thread.Start();
			}
		}

		private void ObserveClient(Socket client)
		{
			Console.WriteLine("Connection accepted.");

			byte[] receivedMessage = new byte[12];

			while (true)
			{
				try
				{
					try
					{
						client.Receive(receivedMessage, 0);
					}
					catch(Exception ex)
					{
						Console.WriteLine("{0} Error: {1}", DateTime.Now, ex.Message);
						return;
					}

					byte[] functionCode = GetFunctionCode(receivedMessage);

					Console.WriteLine("Data time: {0}, Requeste from client - Function code: {1}", DateTime.Now, (FunctionCode)functionCode.FirstOrDefault());

					switch ((FunctionCode)functionCode.FirstOrDefault())
					{
						case FunctionCode.READ_COILS:
							throw new NotImplementedException();

						case FunctionCode.READ_DISCRETE_INPUTS:
							throw new NotImplementedException();
					 
						case FunctionCode.READ_HOLDING_REGISTERS:

							byte[] response_read_holding_registers = null;

							lock (functionLock)
							{
								response_read_holding_registers = ReadHoldingRegisters(receivedMessage);
							}

							try
							{
								client.Send(response_read_holding_registers);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								return;
							}

							break;
						case FunctionCode.READ_INPUT_REGISTERS:
							throw new NotImplementedException();

						case FunctionCode.WRITE_SINGLE_COIL:
							throw new NotImplementedException();
					 
						case FunctionCode.WRITE_SINGLE_REGISTER:

							byte[] response_write_single_register = null;

							lock (functionLock)
							{
								response_write_single_register = WriteSingleRegister(receivedMessage);

								try
								{
									client.Send(response_write_single_register);
								}
								catch(Exception ex)
								{
									Console.WriteLine(ex.Message);
									return;
								}  
							}

							break;
						default:

							byte[] response_error = new byte[headerLength + functionCodeLength + exceptionCodeLength]; ;

							byte[] errorFunctionCode = new byte[functionCodeLength];

							// File error function code
							errorFunctionCode[0] = Convert.ToByte((short)FunctionCode.ERROR);

							byte[] exceptionCode = new byte[exceptionCodeLength];

							// File exception code
							exceptionCode[0] = Convert.ToByte((short)ExceptionCode.ILLEGAL_FUNCTION);

							Buffer.BlockCopy(receivedMessage, 0, response_error, 0, headerLength);
							Buffer.BlockCopy(errorFunctionCode, 0, response_error, headerLength, functionCodeLength);
							Buffer.BlockCopy(exceptionCode, 0, response_error, headerLength + functionCodeLength, exceptionCodeLength);

							try
							{
								client.Send(response_error);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								return;
							}

							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("{0} Error: {1}", DateTime.Now, ex.Message);
					return;
				}
			}
		}

		/// <summary>
		/// Read holding registers
		/// </summary>
		/// <param name="byteRequest"></param>
		/// <returns></returns>
		private byte[] ReadHoldingRegisters(byte[] byteRequest)
		{
			// Get registar address
			byte[] registarAddress = GetRegisterAddress(byteRequest);

			// Get quantity
			byte[] quantityBytes = GetRegisterValue(byteRequest);

			ushort registarAddress_ushort= BitConverter.ToUInt16(registarAddress, 0);
			short registarAddress_short = IPAddress.HostToNetworkOrder((short)registarAddress_ushort);

			ushort quantity_ushort = BitConverter.ToUInt16(quantityBytes, 0);
			short quantity_short = IPAddress.HostToNetworkOrder((short)quantity_ushort);

			byte[] values = new byte[2 * quantity_short];

			lock (lockObj)
			{
				for (int i = 0; i < quantity_short; i++)
				{
					byte[] temp = BitConverter.GetBytes(holdingRegisters[registarAddress_short + i]);
					values[2 * i] = temp[1];
					values[2 * i + 1] = temp[0];
					Console.WriteLine("Read holding register request from client: Address: {0}, Register Value: {1}", registarAddress_short + i, 
																									 holdingRegisters[registarAddress_short + i]);
				}
			}

			// Byte count
			int byteCount_int = 2 * quantity_short;
			byte[] byteCount = new byte[countByteLength];

			// File byte count
			byteCount[0] = Convert.ToByte(byteCount_int);


			byte[] response = new byte[headerLength + functionCodeLength +  countByteLength + byteCount_int];

			Buffer.BlockCopy(byteRequest, 0, response, 0, headerLength);
			Buffer.BlockCopy(byteRequest, headerLength, response, headerLength, functionCodeLength);
			Buffer.BlockCopy(byteCount, 0, response, headerLength + functionCodeLength, countByteLength);
			Buffer.BlockCopy(values, 0, response, headerLength + functionCodeLength + countByteLength, byteCount_int);

			return response;
		}

		private byte[] WriteSingleRegister(byte[] byteRequest)
		{
			// Response
			byte[] response = new byte[headerLength + functionCodeLength + registerValueLength];

			// Get registar address
			byte[] registarAddress = GetRegisterAddress(byteRequest);

			// Get registar value
			byte[] registarValue = GetRegisterValue(byteRequest);

			ushort registarAddress_ushort = BitConverter.ToUInt16(registarAddress, 0);
			short registarAddress_short = IPAddress.HostToNetworkOrder((short)registarAddress_ushort);

			short value_short = IPAddress.HostToNetworkOrder((short)BitConverter.ToUInt16(registarValue, 0));


			if (!holdingRegisters.ContainsKey(registarAddress_short))
			{
				Console.WriteLine("Error: Address does not exist.");

				byte[] functionCode = new byte[functionCodeLength];

				// File function code
				functionCode[0] = Convert.ToByte((short)FunctionCode.WRITE_SINGLE_REGISTER + (short)FunctionCode.ERROR);

				byte[] exceptionCode = new byte[exceptionCodeLength];

				// File exception code
				exceptionCode[0] = Convert.ToByte((short)ExceptionCode.ILLEGAL_DATA_ADDRESS);

				Buffer.BlockCopy(byteRequest, 0, response, 0, headerLength);
				Buffer.BlockCopy(functionCode, 0, response, headerLength, functionCodeLength);
				Buffer.BlockCopy(exceptionCode, 0, response, headerLength + functionCodeLength, exceptionCodeLength);

				return response;
			}

			lock (lockObj)
			{
				holdingRegisters[registarAddress_short] = value_short;

				Console.WriteLine("Write single register request from client: Address: {0}, Register Value: {1}", 
					registarAddress_short, value_short);
			}

			response = byteRequest;

			return response;
		}

		private byte[] GetQuantity(byte[] byteRequest)
		{
			byte[] quantity = new byte[quantityOfCoilsLength];

			int registerValuePosition = headerLength + functionCodeLength + registerValueLength;

			// Gives you hexadecimal
			string hexString = registerValuePosition.ToString("X");

			// Back to int again.
			int position = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

			Buffer.BlockCopy(byteRequest, position, quantity, 0, quantityOfCoilsLength);

			return quantity;
		}

		/// <summary>
		/// Get function code
		/// </summary>
		/// <param name="byteRequest">received message</param>
		/// <returns>function code</returns>
		private byte[] GetFunctionCode(byte[] byteRequest)
		{
			byte[] functionCode = new byte[functionCodeLength];

			Buffer.BlockCopy(byteRequest, headerLength, functionCode, 0, functionCodeLength);

			return functionCode;
		}

		private byte[] GetRegisterAddress(byte[] byteRequst)
		{
			byte[] registerAddress = new byte[registerValueLength];

			Buffer.BlockCopy(byteRequst, headerLength + functionCodeLength, registerAddress, 0, registerValueLength);

			return registerAddress;
		}

		private byte[] GetRegisterValue(byte[] byteRequst)
		{
			byte[] registerValue = new byte[registerValueLength];

			int  registerValuePosition = headerLength + functionCodeLength + registerValueLength;

			// Gives you hexadecimal
			string hexString= registerValuePosition.ToString("X");

			// Back to int again.
			int position = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber); 

			Buffer.BlockCopy(byteRequst, position, registerValue, 0, registerValueLength);

			return registerValue;
		}

		public void Stop()
		{
			try
			{
				listener.Stop();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
