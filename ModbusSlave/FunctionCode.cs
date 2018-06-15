using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusSlave
{
    public enum FunctionCode : byte
    {
        READ_COILS = 0x1,
        READ_DISCRETE_INPUTS = 0x2,
        READ_HOLDING_REGISTERS = 0x3,
        READ_INPUT_REGISTERS = 0x4,
        WRITE_SINGLE_COIL = 0x5,
        WRITE_SINGLE_REGISTER = 0x6, 
        ERROR = 0x80
    }
}
