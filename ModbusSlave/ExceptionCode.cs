using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusSlave
{
    public enum ExceptionCode : byte
    {
        ILLEGAL_FUNCTION = 0x1,
        ILLEGAL_DATA_ADDRESS = 0x2,
        ILLEGAL_DATA_VALUE = 0x3, 
        SLAVE_DEVICE_FAILURE = 0x4
    }
}
