using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using UnityEngine;

namespace vesal_network
{
    public class packet
    {
        public byte _cmd_code;
        public ushort _data_len;
        public byte[] _data;

        public byte[] create_output_buff()
        {
            byte[] buff = new byte[3 + _data_len];
            buff[0] = _cmd_code;
            buff[1] = (byte)(_data_len / 256);
            buff[2] = (byte)(_data_len % 256);
            _data.CopyTo(buff, 3);
            return buff;
        }

        public static byte[] create_output_from_string(byte cmd_code, String str)
        {
            byte[] textbuf = Encoding.Default.GetBytes(str);
            int len = textbuf.Length;
            byte[] buff = new byte[3 + len];
            buff[0] = cmd_code;

            buff[1] = (byte)(len / 256);
            buff[2] = (byte)(len % 256);
            for (int i = 0; i < len; i++)
            {
                buff[i + 3] = textbuf[i];
            }

            return buff;
        }
    }

    public class vesal_socket
    {
        public Socket _sock = null;
        byte[] _head_buff = new byte[3];
        byte[] _data_buff = new byte[15000];
        ushort _expect_len = 3;
        ushort _offset = 0;
        public bool _get_header = false;

        public void reset_recv_buff()
        {
            _offset = 0;
            _expect_len = 3;
            _get_header = false;
        }

        public static ushort get_vesal_port()
        {
            return 16666;
        }
        public static ushort get_WeiKePlayer_port()
        {
            return 17000;
        }
        public bool connect(String peer_ip, ushort port)
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(peer_ip), port);//服务器的IP和端口
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                _sock.Connect(ie);
            }
            catch (SocketException e)
            {

                return false;
            }

            return true;
        }

        public Socket accept()
        {
            return _sock.Accept();
        }

        public bool listen(ushort port)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);//本机预使用的IP和端口
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _sock.Bind(ipep);//绑定
            }
            catch (Exception ex)
            {
                if (ex.Source != null)

                    return false;
            }

            _sock.Listen(3);//监听
            return true;
        }

        public int send_confirm()
        {
            byte[] data = packet.create_output_from_string((byte)VESAL_CMD_CODE.CONFIRM_CMD, "ok");
            return send_data(data);
        }

        public int send_data(byte[] data)
        {
            return _sock.Send(data, data.Length, SocketFlags.None);
        }

        static public ushort get_pack_len(byte[] data)
        {
            ushort b1 = (ushort)data[1];
            ushort b2 = (ushort)data[2];
            return (ushort)(b1 * 256 + b2);
        }

        public bool recv_packet(ref packet pk)
        {
            int ret = 0;
            if (!_get_header)
            {
                ret = _sock.Receive(_head_buff, _offset, _expect_len - _offset, SocketFlags.None);
            }
            else
            {
                ret = _sock.Receive(_data_buff, _offset, _expect_len - _offset, SocketFlags.None);
            }

            if (ret <= 0)
            {
                // link broken.
                throw new ArgumentNullException("value"); ;
            }

            _offset += (ushort)ret;
            if (_offset == _expect_len)
            {
                _offset = 0;
                if (!_get_header)
                {
                    // 包头接收完毕。
                    _get_header = true;
                    _expect_len = get_pack_len(_head_buff);
                    return false;
                }
                else
                {
                    // 一个包收完了。
                    _get_header = false;

                    //输出包
                    pk._cmd_code = _head_buff[0];
                    pk._data_len = _expect_len;
                    pk._data = new byte[_expect_len];
                    for (int j = 0; j < _expect_len; j++)
                    {
                        pk._data[j] = _data_buff[j];
                    }
                    _expect_len = 3;
                    return true;
                }
            }

            return false;
        }

        public void close()
        {
            _sock.Close();
        }
    }


    class process_info
    {
        public static void record_process_info()
        {
            FileStream file = new FileStream("d:\\unity.txt", FileMode.Create);
            String pid = Process.GetCurrentProcess().Id.ToString() + "\n";
            String hwnd = Process.GetCurrentProcess().MainWindowHandle.ToString() + "\n";
            file.Write(System.Text.Encoding.Default.GetBytes(pid), 0, pid.Length);
            file.Write(System.Text.Encoding.Default.GetBytes(hwnd), 0, hwnd.Length);
            file.Close();
        }
    }


    public class Vesal_Network
    {
        //是否离线
        public static bool get_network_is_acitve()
        {


#if UNITY_STANDALONE_WIN
            if (PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.PPTPlayer || PPTGlobal.PPTEnv == PPTGlobal.PPTEnvironment.WeiKePlayer) {
                #if UNITY_EDITOR
                #else
                    return false;
                #endif
            }
            System.Net.NetworkInformation.NetworkInterface[] interf = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < interf.Length; i++)
            {
                vesal_log.vesal_write_log(interf[i].Name);
                if ((interf[i].OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up) &&
                    (!interf[i].Name.ToLower().Contains("loopback")))
                    return true;
            }
            return false;
#else
            switch (Application.internetReachability)
            {
                case  NetworkReachability.ReachableViaCarrierDataNetwork:
                    vesal_log.vesal_write_log("网络环境------------------  234G");
                    return true;
                case  NetworkReachability.ReachableViaLocalAreaNetwork  :
                    vesal_log.vesal_write_log("网络环境------------------  WiFi");
                    return true;
                case  NetworkReachability.NotReachable  :
                vesal_log.vesal_write_log("离线----------------");
                    return false;
                default:
                vesal_log.vesal_write_log("离线----------------");
                    return false;
            }
#endif
            }
        //是否可以连接服务器
        public static bool get_vesal_server_is_available(String server_url)
        {
//            #if UNITY_EDITOR || UNITY_IOS
//            return true;
//#endif
            try
            {
                string str = Vesal_Http_Oper.http_get(server_url, "");
                if (str == "")
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static string get_low_res_ablist(String url)
        {
            try
            {
                string str = Vesal_Http_Oper.http_get(url, "");
                if (str == "")
                {
                    return "";
                }
                return str;
            }
            catch
            {
                return "";
            }
        }


        public static string get_ipfromlist(String url)
        {
            //try
            //{
            string str = Vesal_Http_Oper.http_get(url, "");
            if (str == "")
            {
                return "";
            }
            ip_info info = JsonConvert.DeserializeObject<ip_info>(str);
            for (int i = 0; i < info.list.Length; i++)
            {
                vesal_log.vesal_write_log("服务器地址： " + info.list[i].server + PublicClass.fix_server_interface);
                var list_info = info.list[i];
//                #if UNITY_EDITOR || UNITY_IOS
//                return list_info.server;
//#endif
                if (get_network_is_acitve() && get_vesal_server_is_available(list_info.server + PublicClass.fix_server_interface))
                {
                    return list_info.server;
                }
            }
            return "";
            // throw new NotImplementedException();
            //}
            //catch (Exception)
            //{
            //}
        }
    }


    public class ip_info
    {
        public string msg;
        public string code;
        public iplist[] list;//资源列表
    }
    public class iplist
    {
        public string server;
        public string name;
    }
    public class Vesal_Http_Oper
    {
        public long _dld_data_count = 0;
        public long _totl_size = 0;
        private static readonly string DefaultUserAgent = "web_spider";

        static public String http_get(String Url, String get_para)
        {
            try
            {
                String Query_url = Url + (get_para == "" ? "" : "?") + get_para;
                vesal_log.vesal_write_log("Query_url " + Query_url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Query_url);
                request.Method = "GET";
                request.ContentType = "application/json;charset=UTF-8";
                request.UserAgent = DefaultUserAgent;



                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception ex)
            {
                vesal_log.vesal_write_log("httpget失败 " + ex.Message);
                return "";
            }
        }

        public static HttpWebResponse http_post(string url, IDictionary<string, string> parameters, Encoding charset)
        {
            HttpWebRequest request = null;
            //HTTPSQ请求
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Proxy = null;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = DefaultUserAgent;


            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = charset.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }

}