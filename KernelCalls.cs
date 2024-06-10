using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EMVLogger
{
    public static partial class KernelCalls
    {
        public delegate void LogCallbackDelegate(string log);
        public delegate IntPtr PinCallbackDelegate();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2Version();

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2Init(string readerName, LogCallbackDelegate logCb, PinCallbackDelegate pinCb);

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2AddCaKey(string rid, byte keyId, string modules, string exponent);

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2AddAidPrms(string aid, string data);

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2GetTag(UInt32 tag, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int maxLen);

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2Start(byte ucTranTypet95, byte ucAccountType, string amount, string otherAmount);

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2CardReset();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2ApplicationSelection();

        [LibraryImport("lib/corelib.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        public static partial int emvl2Gpo(byte ucTranTypet95, byte ucAccountType, string amount, string otherAmount);

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2ReadAppData();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2OfflineDataAuth();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2ProcessRestrict();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2ProcessCVM();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2TerminalRiskMng();

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2TermActionAnalysis(ref byte TerminalDecision);

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2GenAC1(byte TerminalDecision, ref byte CardDecision);

        [LibraryImport("lib/corelib.dll", SetLastError = true)]
        public static partial int emvl2GenAC2(byte isOnlineError, ref byte Decision, ref byte AdviceReversal);
    }
}
