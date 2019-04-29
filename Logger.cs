using System;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace FolderFilesTree
{

    public class Logger : ILogger
    {
        private string _path;
        System.Diagnostics.Stopwatch _sw;
        static Logger _logger;
        private Logger()
        {
            _path = "log.txt";
            _sw = new Stopwatch();
            using (StreamWriter s = new StreamWriter(_path, false, Encoding.Default)) ;
        }

        public static Logger GetLogger()
        {
            if (_logger == null)
            {
                _logger = new Logger();
            }
            return _logger;
        }

        public void LogStartTime()
        {

            using (StreamWriter s = new StreamWriter(_path, true, Encoding.Default))
            {
                _sw.Start();
                s.WriteLine("START TIME: " + DateTime.Now);
            }


        }

        public void LogException(String e)
        {
            try
            {
                using (StreamWriter s = new StreamWriter(_path, true, Encoding.Default))

                    s.WriteLine(DateTime.Now + " " + e);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Conflict with write LogException");
            }
        }

        public void LogAccessDenied(String path)
        {
            try
            {
                using (StreamWriter s = new StreamWriter(_path, true, Encoding.Default))
                {
                    s.WriteLine(DateTime.Now + " Access denied " + path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" LogAccessDenied");
            }
        }

        public void LogFinalTime()
        {
            _sw.Stop();
            using (StreamWriter s = new StreamWriter(_path, true, Encoding.Default))
            {
                s.WriteLine("FINAL TIME: " + DateTime.Now);
                s.WriteLine("TIME: " + (_sw.ElapsedMilliseconds / 1000.0).ToString());
            }

        }

    }
}