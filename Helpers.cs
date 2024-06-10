using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace EMVLogger
{
    public partial class Helpers
    {
        private static readonly TaskFactory _taskFactory = new
    TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static void RunSync(Func<Task> func)
            => _taskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public static string ByteToStringArray(byte[] arr)
        {
            return string.Join(string.Empty, Array.ConvertAll(arr, element => element.ToString("X2")));
        }

        public static byte[] ToBcdByteArray(string hex)
        {
            if (hex.Length % 2 > 0)
            {
                hex += "0";
            }
            string fstr = "";
            for (int i = 0; i < hex.Length; i++)
            {
                char current_char = hex[i];
                if ((current_char >= '0' && current_char <= '9') ||
                    (current_char >= 'a' && current_char <= 'f') ||
                    (current_char >= 'A' && current_char <= 'F'))
                {
                    fstr += hex[i];
                }
                else
                {
                    fstr += 'F';
                }
            }
            return Enumerable.Range(0, fstr.Length)
              .Where(x => x % 2 == 0)
              .Select(x => Convert.ToByte(fstr.Substring(x, 2), 16))
              .ToArray();
        }
    }

}
