using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

enum VESAL_CMD_CODE
{
    START_CMD = 1,
    CONFIRM_CMD = 2,
    MSG_CMD = 3,
    CTRL_CMD = 4,
    WIN_HWND = 5,
    END_CONTROL = 6
}


namespace netcomm
{
    public class packet
    {
        public byte _cmd_code;
        public uint _data_len;
        public byte[] _data;

        static public uint _head_len = 5;
        /*
        public byte[] create_output_buff()
        {
            byte[] buff = new byte[3 + _data_len];
            buff[0] = _cmd_code;
            buff[1] = (byte)(_data_len / 256);
            buff[2] = (byte)(_data_len % 256);
            _data.CopyTo(buff, 3);
            return buff;  
        }
         * */

        public static byte[] create_output_from_string(byte cmd_code, String str)
        {
            //修改编码方式
            byte[] textbuf = Encoding.UTF8.GetBytes(str);
            UInt32 len = (UInt32)textbuf.Length;
            byte[] buff = new byte[_head_len + len];
            buff[0] = cmd_code;

            buff[1] = (byte)(len >> 24);
            buff[2] = (byte)(len >> 16);
            buff[3] = (byte)(len >> 8);
            buff[4] = (byte)(len % 256);
            for (UInt32 i = 0; i < len; i++)
            {
                buff[i + _head_len] = textbuf[i];
            }

            return buff;
        }

        public static byte[] create_output_from_bytes(byte cmd_code, byte[] bs)
        {
            //修改编码方式
            byte[] textbuf = bs;
            UInt32 len = (UInt32)textbuf.Length;
            byte[] buff = new byte[_head_len + len];
            buff[0] = cmd_code;

            buff[1] = (byte)(len >> 24);
            buff[2] = (byte)(len >> 16);
            buff[3] = (byte)(len >> 8);
            buff[4] = (byte)(len % 256);
            for (UInt32 i = 0; i < len; i++)
            {
                buff[i + _head_len] = textbuf[i];
            }

            return buff;
        }
    }

    public class vesal_socket
    {
        bool _is_client = true;
        public Socket _sock = null;
        byte[] _head_buff = new byte[packet._head_len];
        byte[] _data_buff = new byte[5000000];
        uint _expect_len = packet._head_len;
        uint _offset = 0;
        bool _get_header = false;

        byte[] _cur_send_buff = null;
        List<byte[]> _buff_to_send = new List<byte[]>();

        private ManualResetEvent timeoutObject;
        public int fd()
        {
            return (int)_sock.Handle;
        }

        public void reset_recv_buff()
        {
            _offset = 0;
            _expect_len = packet._head_len;
            _get_header = false;
        }

        public static ushort get_vesal_port()
        {
            return 16666;
        }

        public void send_packet(byte cmd_code, byte[] bs)
        {
            byte[] buff = packet.create_output_from_bytes(cmd_code, bs);
            _buff_to_send.Add(buff);
        }

        public bool has_data()
        {
            if (_cur_send_buff != null)
            {
                return true;
            }

            if (_buff_to_send.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool send_data()
        {
            if (_cur_send_buff == null)
            {
                if (_buff_to_send.Count == 0)
                {
                    return true;
                }

                _cur_send_buff = _buff_to_send[0];
                _buff_to_send.RemoveAt(0);
            }

            try
            {
                int ret = _sock.Send(_cur_send_buff, _cur_send_buff.Length, SocketFlags.None);
                //vesal_log.vesal_write_log("ret "+ ret);
                //vesal_log.vesal_write_log("_cur_send_buff.Length " + _cur_send_buff.Length);
                if (ret == _cur_send_buff.Length)
                {
                    _cur_send_buff = null;
                    return true;
                }
                vesal_log.vesal_write_log("================================");
                byte[] tmp = new byte[_cur_send_buff.Length - ret];
                for (int i = 0; i < tmp.Length; i++)
                {
                    tmp[i] = _cur_send_buff[i + ret];
                }
                _cur_send_buff = tmp;
            }
            catch (Exception e)
            {
                int err = errno(e);
                //vesal_log.vesal_write_log(e.Message);
                //vesal_log.vesal_write_log(e.StackTrace);
                if (err == 10035) // block
                {
                    return true;
                }
                else if (err == 10054) // broken pipe
                {
                    vesal_log.vesal_write_log("big err == 10054");
                    return false;
                }
                vesal_log.vesal_write_log("err:" + err);
                return false;
            }

            return true;
        }

        public bool connect(String peer_ip, ushort port)
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // _sock.ReceiveTimeout = 1000;
            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(peer_ip), port);//服务器的IP和端口
            try
            {
                //因为客户端只是用来向特定的服务器发送信息，所以不需要绑定本机的IP和端口。不需要监听。
                // _sock.Connect(ie);
                IAsyncResult result = _sock.BeginConnect(ie, null, null);
                if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(30)))
                {
                    //vesal_log.vesal_write_log("=============Connect timeout ==============");
                    return false;
                    //throw new TimeoutException("connect Timeout!!!");
                }
                _sock.EndConnect(result);

            }
            catch (SocketException e)
            {
                //异常时，主线程调用阻塞
                //DebugLog.DebugLogInfo(e.ToString());
                //Console.WriteLine("unable to connect to server");
                //Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        public vesal_socket accept()
        {
            Socket sock = _sock.Accept();
            sock.Blocking = false;
            vesal_socket clisock = new vesal_socket();
            clisock._sock = sock;
            return clisock;
        }

        public bool listen(ushort port)
        {
            _is_client = false;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);//本机预使用的IP和端口
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _sock.Bind(ipep);//绑定
            }
            catch (Exception ex)
            {
                if (ex.Source != null)
                    //Console.WriteLine("listen source: {0}", ex.Source);
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

        static public uint get_pack_len(byte[] data)
        {
            uint b1 = (uint)data[1];
            uint b2 = (uint)data[2];
            uint b3 = (uint)data[3];
            uint b4 = (uint)data[4];
            return b1 * (256 * 256 * 256) +
                   b2 * (256 * 256) +
                   b3 * 256 +
                   b4;
        }

        public bool recv_packet(ref packet pk)
        {
            int ret = 0;
            if (!_get_header)
            {
                ret = _sock.Receive(_head_buff, (int)_offset, (int)(_expect_len - _offset), SocketFlags.None);
            }
            else
            {
                ret = _sock.Receive(_data_buff, (int)_offset, (int)(_expect_len - _offset), SocketFlags.None);
            }

            if (ret <= 0)
            {
                vesal_log.vesal_write_log("ret error");
                // link broken.
                throw new ArgumentNullException("value"); ;
            }

            _offset += (uint)ret;
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
                    _expect_len = packet._head_len;
                    return true;
                }
            }

            return false;
        }

        public void close()
        {
            _sock.Close();
        }

        public static int errno(Exception e)
        {
            var w32ex = e as Win32Exception;
            if (w32ex == null)
            {
                w32ex = e.InnerException as Win32Exception;
            }
            if (w32ex != null)
            {
                int code = w32ex.ErrorCode;
                return code;
            }
            return 0;
        }
    }


    public class sock_manager
    {
        Dictionary<int, vesal_socket> _listen_socks = new Dictionary<int, vesal_socket>();
        public Dictionary<int, vesal_socket> _comm_socks = new Dictionary<int, vesal_socket>();

        public void add_sock(vesal_socket vsock, bool listen)
        {
            if (listen)
            {
                _listen_socks[(int)vsock._sock.Handle] = vsock;
            }
            else
            {
                _comm_socks[(int)vsock._sock.Handle] = vsock;
            }
        }

        //写数据时，socket broken
        public void work()
        {
            if (_listen_socks.Count > 0)
            {
                ArrayList sockList = new ArrayList();
                foreach (var item in _listen_socks)
                {
                    sockList.Add(item.Value._sock);
                }

                Socket.Select(sockList, null, null, 0);
                for (int i = 0; i < sockList.Count; i++)
                {
                    vesal_socket vsock = _listen_socks[(int)((Socket)sockList[i]).Handle];
                    vesal_socket newsock = vsock.accept();
                    _comm_socks[(int)newsock._sock.Handle] = newsock;
                    on_peer_connected(newsock);
                }
            }

            if (_comm_socks.Count > 0)
            {
                List<int> bad_socks = new List<int>();
                ArrayList sockList2 = new ArrayList();
                foreach (var item in _comm_socks)
                {
                    sockList2.Add(item.Value._sock);
                }

                Socket.Select(sockList2, null, null, 0);

                for (int i = 0; i < sockList2.Count; i++)
                {
                    int fd = (int)((Socket)sockList2[i]).Handle;
                    vesal_socket vsock = _comm_socks[fd];
                    try
                    {
                        packet pk = new packet();
                        bool getpack = vsock.recv_packet(ref pk);
                        if (getpack)
                        {
                            on_packet_recved(vsock, pk);
                        }
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("link read bad");
                        bad_socks.Add(fd);
                    }
                }

                clean_bad_fd(bad_socks);

                foreach (var item in _comm_socks)
                {
                    if (item.Value.has_data())
                    {
                        bool ret = item.Value.send_data();
                        if (!ret)
                        {
                            //Console.WriteLine("link send bad");
                            bad_socks.Add(item.Key);
                        }
                    }
                }

                clean_bad_fd(bad_socks);
            }

        }
        void clean_bad_fd(List<int> bad_socks)
        {
            for (int i = 0; i < bad_socks.Count; i++)
            {
                int fd = bad_socks[i];
                on_link_broken(_comm_socks[fd]);
                _comm_socks[fd].close();
                _comm_socks.Remove(fd);
            }
            bad_socks.Clear();
        }

        public void send_packet(int fd, byte cmd, byte[] data)
        {
            vesal_socket vsk = _comm_socks[fd];
            vsk.send_packet(cmd, data);
        }

        public void brodcast_data(byte cmd, byte[] data)
        {
            foreach (var item in _comm_socks)
            {
                vesal_socket vsk = _comm_socks[item.Key];
                vsk.send_packet(cmd, data);
            }
        }

        // 以下接口，由子类实现
        virtual public void on_peer_connected(vesal_socket vsock)
        {
            //Console.WriteLine("{0} link connected.", vsock.fd());
        }

        virtual public void on_packet_recved(vesal_socket vsock, packet pk)
        {
            //Console.WriteLine("{0} recv packet.", vsock.fd());
        }

        virtual public void on_link_broken(vesal_socket vsock)
        {
            //Console.WriteLine("{0} link broken.", vsock.fd());
        }

    }


}


