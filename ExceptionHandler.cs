using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    class ExceptionHandler : IExceptionHandler
    {
        Logger _logger;

        public ExceptionHandler()
        {
            _logger = Logger.GetLogger();
        }

        public void HandleException(Exception ex)
        {
            _logger.LogException(ex.Message);
        }
    }
}
