using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class AppCommon
    {

        public enum APPErrors
        {
            STATUS_OK,
            STATUS_ERR,
            STATUS_FG_STILL_RUNNING,
            STATUS_FG_PENDING,
            STATUS_FG_TIMEOUT
        }

        public enum MODULES
        {
            MANAGER_MODULE,
            FG_MODULE,
            UDP_MODULE,
            IP_MODULE
        }

        public struct UDPMessageHeader
        {
            public ushort opcode;
            public ushort size;
        }

        public struct UPayload
        {
            public UDPMessageHeader header;
            public float data;
            public ushort crc;
        }

        public static byte[] StructToByteArray<T>(T structVal) where T : struct
        {
            int size = Marshal.SizeOf(structVal);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structVal, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static void ByteArrayToStruct<T>(byte[] packet, ref T str)
        {
            GCHandle pinnedPacket = GCHandle.Alloc(packet, GCHandleType.Pinned);
            str = (T)Marshal.PtrToStructure(
                pinnedPacket.AddrOfPinnedObject(),
                typeof(T));
            pinnedPacket.Free();
        }


    }
}
