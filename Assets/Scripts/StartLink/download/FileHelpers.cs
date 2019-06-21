//----------------------------------------------------------------------------------------------------
// <copyright company="Avira Operations GmbH & Co. KG and its licensors">
// © 2016 Avira Operations GmbH & Co. KG and its licensors.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------------

using System;
using System.IO;

namespace FileDownloader
{
    internal static class FileHelpers
    {

        public static bool TryGetFileSize(string filename, out long filesize)
        {
            try
            {
                var fileInfo = new FileInfo(filename);
                filesize = fileInfo.Length;
            }
            catch (Exception e)
            {
                filesize = 0;
                return false;
            }
            return true;
        }

        public static bool TryFileDelete(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool ReplaceFile(string source, string destination)
        {
            if (!destination.Equals(source, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    File.Delete(destination);
                    File.Move(source, destination);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }
    }
}