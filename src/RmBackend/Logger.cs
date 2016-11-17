using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmBackend
{
    public class Logger
    {
        public static Logger General { get; private set; } = null;
        public static Logger Exception { get; private set; } = null;

        public static void InitStatic(string general, string exception)
        {
            try
            {
                General = new Logger(general);
            }
            catch (Exception)
            {
                General = null;
            }

            try
            {
                Exception = new Logger(exception);
            }
            catch (Exception)
            {
                Exception = null;
            }

            General?.WriteLine("General log init.");
            Exception?.WriteLine("Exception log init.");
        }

        private object _logLock = new object();
        private StringBuilder _logWritten = new StringBuilder();
        private FileStream _logFs;
        private StreamWriter _logSw;

        public Logger(string file, bool overwrite = false)
        {
            _logFs = new FileStream(file, overwrite ? FileMode.Create : FileMode.Append);
            _logSw = new StreamWriter(_logFs);
        }

        ~Logger()
        {
            _logSw.Dispose();
            _logFs.Dispose();
        }

        private string TryFormat(string format, params object[] args)
        {
            try
            {
                return String.Format(format, args);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteLine(string msg, params object[] args)
        {
            lock (_logLock)
            {
                string log = TryFormat(msg, args) ?? "[Error formatting log]";
                log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {log}";

                _logSw?.WriteLine(log);
                _logSw?.Flush();
                _logFs?.Flush();
                _logWritten.AppendLine(log);
            }
        }

        public string WrittenLogs()
        {
            return _logWritten.ToString();
        }
    }
}
