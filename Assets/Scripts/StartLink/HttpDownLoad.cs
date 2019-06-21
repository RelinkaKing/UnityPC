using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Threading;
using VesalCommon;
using FileDownloader;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;


public class HttpDownLoad
{

    //下载进度
    public float progress { get; private set; } //C#私有属性，set 前面加private 告诉编译器属性是只读的，外部不能给属性赋值，只能读取其值
    //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
    private bool isStop;
    ////子线程负责下载，否则会阻塞主线程，Unity界面会卡主
    public Thread thread;
    ////表示下载是否完成
    public bool isDone { get; private set; }

    //const int oneReadLen = 16384;           // 一次读取长度 16384 = 16*kb
    //const int ReadWriteTimeOut = 2 * 1000;  // 超时等待时间
    //const int TimeOutWait = 5 * 1000;       // 超时等待时间
    //const int MaxTryTime = 3;
    Action succes_method = null;
    Action<Exception> fail_method = null;
    string savepath = "";
    string temppath = "";
    IFileDownloader fileDownloader;
    bool sameNameSubDir = false;
    public string state = "download";
    bool isZipFile = false;
    public bool DownLoad(string downUrl, string savePath, Action callBack = null, Action<Exception> errorCallBack = null, bool NameSubDir= false)
    {
        state = "download";
        sameNameSubDir = NameSubDir;
        vesal_log.vesal_write_log("-- download: "+downUrl+savePath);
        savepath = savePath;
        temppath = savepath + ".temp";
        string suffix = Vesal_DirFiles.get_name_suffix(savepath);
        if (suffix.ToLower() == "zip")
            isZipFile = true;
        Vesal_DirFiles.DelFile(temppath);
        if(callBack!=null)
            succes_method = callBack;
        if (errorCallBack != null)
            fail_method = errorCallBack;
        try
        {
            Uri url = new Uri(downUrl);
            TimeSpan x = new TimeSpan(0, 0, 8);
            fileDownloader = new FileDownloader.FileDownloader();
            fileDownloader.MaxAttempts = 10;
            fileDownloader.SafeWaitTimeout = x;
            fileDownloader.DownloadProgressChanged += DownLoadFileChanged;
            fileDownloader.DownloadFileCompleted += DownloadFileCompleted;
            fileDownloader.DownloadFileAsync(url, temppath);
            vesal_log.vesal_write_log("-- downalod asy started");
        }
        catch
        {
            return false;
        }
        return true;
    }
    void DownLoadFileChanged(object sender, DownloadFileProgressChangedArgs eventArgs)
    {
        if(isZipFile)
            progress = eventArgs.ProgressPercentage / 200.0f;
        else
            progress=eventArgs.ProgressPercentage/100.0f;
    }

    void DownloadFileCompleted(object sender, DownloadFileCompletedArgs eventArgs)
    {
        if (eventArgs.State == CompletedState.Succeeded && eventArgs.BytesReceived == eventArgs.BytesTotal)
        {
            //download completed
            vesal_log.vesal_write_log("completed file percent:" + eventArgs.DownloadProgress);
            vesal_log.vesal_write_log("bytes:" + eventArgs.BytesReceived.ToString()+"::"+eventArgs.BytesTotal);

            string suffix = Vesal_DirFiles.get_name_suffix(savepath);

            if (suffix == "assetbundle")
            {
//                state = "saveab";
                abfile_process();
            }
            else if (suffix.ToLower() == "zip") 
            {
//                state = "zip";
                unzip_process();
            }
            else
            {

                File.Move(temppath, savepath);
                if (succes_method != null)
                    succes_method();
            }
            
        }
        else 
        {
            vesal_log.vesal_write_log("download fail:" + eventArgs.DownloadProgress);
            //download failed
            Vesal_DirFiles.DelFile(temppath);
            if (fail_method != null)
                fail_method(eventArgs.Error);
        }
        if (fileDownloader != null)
            fileDownloader.Dispose();
    }

    public bool unzip_process()
    {

        vesal_log.vesal_write_log("--unzip file start  ");

        thread = new Thread(delegate()
        {
            string fname=Vesal_DirFiles.get_file_name_from_full_path(savepath).Split('.')[0];
            string targetdir;
            if (!sameNameSubDir)
            {
                targetdir = Vesal_DirFiles.get_dir_from_full_path(savepath);
//                Vesal_DirFiles.UnZipAsync(temppath, targetdir, zip_progress, true);
            }
            else
            {
                targetdir = Vesal_DirFiles.get_dir_from_full_path(savepath) + fname + "/";
                if (!Directory.Exists(targetdir))
                    Directory.CreateDirectory(targetdir);
//                Vesal_DirFiles.UnZipAsync(temppath, targetdir, zip_progress, true);
            }
            StreamReader streamReader = new StreamReader(temppath, Encoding.Default);
            long sum = streamReader.BaseStream.Length;
            using (ZipInputStream s = new ZipInputStream(streamReader.BaseStream))
            {
                //File.OpenRead(sourcePath)
                //s.setEncoding("UTF-8");
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    progress = 0.5f + (float)s.Position / sum * 0.5f;
                    theEntry.IsUnicodeText = true;
                    string directoryName = Path.GetDirectoryName(theEntry.Name);

                    //UnityEngine.Debug.Log("aaa:" + theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(targetdir + directoryName);
                    }
                    string filePath = targetdir + theEntry.Name;
                    if (File.Exists(filePath))
                    {
                            File.Delete(filePath);
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(filePath))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                progress = 0.5f + (float)s.Position / sum * 0.5f;
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }




            Vesal_DirFiles.DelFile(temppath);
            if (succes_method != null)
                succes_method();
        });
        //开启子线程
        thread.IsBackground = true;
        thread.Start();
        //vesal_log.vesal_write_log("-- unzip thread started");

        return true;
    }

    public bool abfile_process()
    {

        vesal_log.vesal_write_log("--ab file start  ");

        thread = new Thread(delegate()
        {
            vesal_log.vesal_write_log("-- ab ss");
            Vesal_DirFiles.SaveAbfile(temppath,savepath, false);
            vesal_log.vesal_write_log("-- ab end");
            if (succes_method != null)
                succes_method();
        });
        //开启子线程
        thread.IsBackground = true;
        thread.Start();
        vesal_log.vesal_write_log("-- ab thread started");

        return true;
    }





    /// <summary>
    /// 下载方法(取消后再次点击，进行断点续传)
    /// </summary>
    /// <param name="url">URL下载地址</param>
    /// <param name="savePath">Save path保存路径</param>
    /// <param name="callBack">Call back回调函数</param>
    //public bool DownLoad(string downUrl, string savePath, Action callBack = null, Action<Exception> errorCallBack = null)
    //{
    //    vesal_log.vesal_write_log("--" + downUrl);

    //    if (System.IO.File.Exists(savePath) == true)
    //    { //如果本地文件存在储存路劲则
    //        if (callBack != null) callBack();
    //        return true;
    //    }
    //    isStop = false;
    //    //开启子线程下载,使用匿名方法

    //    vesal_log.vesal_write_log("--download thread start  ");

    //    thread = new Thread(delegate ()
    //    {
    //        try
    //        {
    //            vesal_log.vesal_write_log("--thread in --  ");

    //            //打开上次下载的文件
    //            long startPos = 0;
    //            string tempFile = savePath + ".temp";
    //            FileStream fs = null; // 文件
    //            //使用流操作文件

    //            if (File.Exists(tempFile))
    //            {
    //                vesal_log.vesal_write_log("--file exist --  ");
    //                fs = File.OpenWrite(tempFile);
    //                startPos = fs.Length;
    //                vesal_log.vesal_write_log("--file ex in --  ");

    //            }
    //            else
    //            {
    //                string direName = Path.GetDirectoryName(tempFile); //返回指定路径字符串的目录信息
    //                if (!Directory.Exists(direName)) Directory.CreateDirectory(direName); //在direName目录下创建目录
    //                vesal_log.vesal_write_log("--create dir --  ");

    //                fs = new FileStream(tempFile, FileMode.Create);//创建流文件
    //                vesal_log.vesal_write_log("--create temp file --  ");

    //            }
    //            //获取文件现在的长度
    //            long fileLength = fs.Length;
    //            //获取下载文件的总长度
    //            long totalLength = GetLength(downUrl);
    //            //如果没下载完
    //            if (fileLength < totalLength)
    //            {
    //                //断点续传核心，设置本地文件流的起始位置
    //                fs.Seek(fileLength, SeekOrigin.Begin);
    //                //创建网络请求
    //                HttpWebRequest request = HttpWebRequest.Create(downUrl) as HttpWebRequest;

    //                //断点续传核心，设置远程访问文件流的起始位置
    //                request.AddRange((int)fileLength);
    //                Stream stream = request.GetResponse().GetResponseStream();
    //                byte[] buffer = new byte[1024];
    //                //使用流读取内容到buffer中
    //                //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
    //                int length = stream.Read(buffer, 0, buffer.Length);
    //                vesal_log.vesal_write_log("--" + length);

    //                int reseek = 0;

    //                while (length > 0)
    //                {
    //                    if (isStop)
    //                    {
    //                        //Debug.Log("stop  length" + length);
    //                        //fs.Flush();
    //                        fs.Close();
    //                        break;
    //                    }

    //                    //将内容再写入本地文件中
    //                    fs.Write(buffer, 0, length);
    //                    //计算进度
    //                    fileLength += length;

    //                    // 判断是否下载完成
    //                    if (fileLength == totalLength)
    //                    {
    //                        vesal_log.vesal_write_log("--download function 999 --  ");
    //                        fs.Flush();// 清除该流的所有缓冲区，使得所有缓冲的数据都被写入到基础设备
    //                        fs.Close();// 关闭流
    //                        fs.Dispose();
    //                        fs = null;
    //                        if (File.Exists(savePath)) File.Delete(savePath);

    //                        if (Vesal_DirFiles.get_name_suffix(savePath) == "assetbundle")
    //                        {
    //                            Vesal_DirFiles.SaveAbfile(tempFile, savePath, false);
    //                        }
    //                        //else if (Vesal_DirFiles.get_name_suffix(savePath).ToLower() == "zip")
    //                        //{
    //                        //    vesal_log.vesal_write_log("zip savePath---------  "+savePath);
    //                        //    vesal_log.vesal_write_log("Vesal_DirFiles.get_dir_from_full_path(savePath)---  "+Vesal_DirFiles.get_dir_from_full_path(savePath));
    //                        //    Vesal_DirFiles.UnZip(tempFile, Vesal_DirFiles.get_dir_from_full_path(savePath), true);
    //                        //    // Vesal_DirFiles.UnZip(tempFile, savePath, true);
    //                        //    Vesal_DirFiles.DelFile(tempFile);
    //                        //}
    //                        else
    //                            File.Move(tempFile, savePath);  // 下载完成将temp文件，改成正式文件
    //                        progress = 1;
    //                        vesal_log.vesal_write_log("--download good --  ");
    //                        break;
    //                    }
    //                    progress = (float)fileLength / (float)totalLength;
    //                    //类似尾递归
    //                    try
    //                    {
    //                        length = stream.Read(buffer, 0, buffer.Length); // 返回读入缓冲区的最大字节
    //                        reseek = 0;
    //                    }
    //                    catch (Exception e)
    //                    {
    //                        vesal_log.vesal_write_log("--download function 888 --  ");
    //                        Debug.Log(e.Message);
    //                        reseek++;

    //                        continue;
    //                    }
    //                    if (reseek > 15)
    //                    {
    //                        vesal_log.vesal_write_log("--download function 666 --  ");

    //                        break;
    //                    }
    //                }
    //                if (progress != 1)
    //                {
    //                    vesal_log.vesal_write_log("--download function rrrr --  ");
    //                    stream.Close();    // 关闭链接
    //                    stream.Dispose(); //  这个链接不需要了，可以释放资源             
    //                }
    //            }
    //            else
    //            {
    //                vesal_log.vesal_write_log("--download function 333 --  ");
    //                progress = 1;
    //                fs.Flush();
    //                fs.Close();
    //                fs = null;
    //                if (File.Exists(savePath)) File.Delete(savePath); //如果文件里存在了，就删了
    //                File.Move(tempFile, savePath);
    //                vesal_log.vesal_write_log("--download function 222 --  ");

    //            }
    //            //如果下载完毕，执行回调
    //            if (progress == 1)
    //            {
    //                vesal_log.vesal_write_log("--download function 1111 --  ");
    //                isDone = true;
    //                if (callBack != null) callBack();
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            if (errorCallBack != null) errorCallBack(e);
    //            vesal_log.vesal_write_log("-HttpDownLoad-" + e.Message);
    //            vesal_log.vesal_write_log("-HttpDownLoad-" + e.StackTrace);
    //        }
    //        vesal_log.vesal_write_log("--download function end --  ");

    //    });
    //    //开启子线程
    //    thread.IsBackground = true;
    //    thread.Start();
    //    vesal_log.vesal_write_log("--download thread started");

    //    return true;
    //}


    /// <summary>
    /// 获取下载文件的大小
    /// </summary>
    /// <returns>The GetLength.</returns>
    /// <param name="url">URL.</param>

    public static long GetLength(string url)
    {
        HttpWebRequest request = null;
        WebResponse respone = null;
        long length = 0;
        try
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            //          request.Timeout = TimeOutWait;
            //          request.ReadWriteTimeout = ReadWriteTimeOut;
            //向服务器请求，获得服务器回应数据流
            respone = request.GetResponse();
            length = respone.ContentLength;
        }
        catch (WebException e)
        {
            throw e;
        }
        finally
        {
            if (respone != null) respone.Close();
            if (request != null) request.Abort();
        }
        return length;
    }

    //取消下载
    public void Close()
    {
        isStop = true;
    }
}