using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public static class CheckFolderAccess
    {
        public static bool CheckAccess(String path)
        {

            if (path == ($"C:\\System Volume Information"))
                return false;
            
            bool listDirAllow = false;
            bool listDirDeny = false;

            try
            {
                DirectorySecurity accessControlList = Directory.GetAccessControl(path);

                if (accessControlList == null)
                    return false;
   
                AuthorizationRuleCollection accessRules = accessControlList.
                    GetAccessRules(true, true,
                    typeof(System.Security.Principal.SecurityIdentifier));

                if (accessRules == null)
                    return false;


                foreach (FileSystemAccessRule rule in accessRules)
                {
                    if ((FileSystemRights.ListDirectory & rule.FileSystemRights) != FileSystemRights.ListDirectory)
                        continue;

                    if (rule.AccessControlType == AccessControlType.Allow)

                    {
                        listDirAllow = true;
                    }

                    else
                    {
                        if (rule.AccessControlType == AccessControlType.Deny)
                            listDirDeny = true;
                    }
                }
            }
            catch (Exception e)
            {
                IExceptionHandler h = new ExceptionHandler();
                h.HandleException(e);
            }

            return (listDirAllow && !listDirDeny);

        }
    }
}
