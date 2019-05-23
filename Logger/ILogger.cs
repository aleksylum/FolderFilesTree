using System;

namespace FolderFilesTree
{
    interface ILogger
    {

        void LogStartTime();
        void LogException(String e);
        void LogAccessDenied(String path);
        void LogFinalTime();

    }
}
