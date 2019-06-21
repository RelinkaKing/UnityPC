using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace netcomm
{
    public class Program : sock_manager
    {
        public delegate void ThreadSocketDelegate();
        public event ThreadSocketDelegate OnpeerBroken;
        public delegate void RecieveDelegate(vesal_socket vsock,byte cmd, byte[] str);
        public event RecieveDelegate OnpeerRecive;
        private ThreadStart threadstart;
        public Thread thread;
        public vesal_socket client_soket;
        public string role_msg;
        public void ThreadingOpera(Action callback)
        {
            //threadstart = new ThreadStart(callback);
            //thread = new Thread(threadstart);
            //thread.Start();

            thread = new Thread(delegate ()
            {
                if (callback != null)
                    callback();
            });
            //开启子线程
            thread.IsBackground = true;
            thread.Start();
        }

        public void StopWork()
        {
            if(thread!=null)
                thread.Join(1000);
        }

        public void Work()
        {
            ThreadingOpera(() =>
            {
                for (; ; )
                {
                    Thread.Sleep(1);
                    base.work();
                }
            });
        }

        public void send_cmd(byte cmd, String str)
        {
            base.brodcast_data(cmd, Encoding.UTF8.GetBytes(str));
        }

        public void send_cmd_fd(int fd, byte cmd, String str)
        {
            //client_soket_arr[0].fd()
            base.send_packet(fd, cmd, Encoding.UTF8.GetBytes(str));
        }

        public void send_byte(byte cmd, byte[] str)
        {
            //sm.send_packet(client_soket_arr[0].fd(),3,data);
            base.brodcast_data(cmd, str);
        }

        public vesal_socket GetClientSocket()
        {
            return client_soket;
        }

        public void CloseClientSocket()
        {
            if(client_soket != null)
                client_soket.close();
        }

        public bool Main_start(string[] args, string server_ip)
        {
            if (args.Length == 0)
            {
                return false;
            }
            StopWork();
            role_msg = args[0];
            switch (args[0])
            {
                case "server":
                    vesal_socket lssock = new vesal_socket();
                    lssock.listen(17000);
                    base.add_sock(lssock, true);
                    ScreenData.instance.ShowText("server start ip: " + server_ip);
                    break;
                case "client":
                    vesal_socket clisock = new vesal_socket();
                    bool ret = clisock.connect(server_ip, 17000);
                    if (!ret)
                    {
                        DebugLog.DebugLogInfo("00ff00", "客户端未连接");
                        return false;
                    }
                    base.add_sock(clisock, false);
                    client_soket = clisock;
                    break;
                default:
                    break;
            }
            return true;
        }

        public override void on_peer_connected(vesal_socket vsock)
        {
            base.on_peer_connected(vsock);
            base.add_sock(vsock, false);
            ScreenData.instance.ShowText(vsock.fd() + " connet");
        }

        public override void on_packet_recved(vesal_socket vsock, packet pk)
        {
            base.on_packet_recved(vsock, pk);

            byte[] buff = new byte[pk._data_len];

            for (int j = 0; j < pk._data_len; j++)
            {
                buff[j] = pk._data[j];
            }
            //DebugLog.DebugLogInfo("ff0000", buff.Length + "");
            //接收
            OnpeerRecive(vsock, pk._cmd_code, buff);
            // string str = System.Text.Encoding.UTF8.GetString(buff);
            // if (pk._cmd_code == (byte)VESAL_CMD_CODE.MSG_CMD)
            // {
            //     UnityEngine.Debug.Log("realMessage:" + str);
            //     OnpeerRecive(str);
            // }
        }

        public override void on_link_broken(vesal_socket vsock)
        {
            base.on_link_broken(vsock);
            UnityEngine.Debug.Log(role_msg+" "+vsock.fd() + " broken");
            client_soket = null;
            OnpeerBroken();
        }

        //remove bom head
        public static string GetUTF8String(byte[] buffer)
        {
            if (buffer == null)
                return null;

            if (buffer.Length <= 3)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0]
                && buffer[1] == bomBuffer[1]
                && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
