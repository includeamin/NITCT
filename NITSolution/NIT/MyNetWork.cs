using CT;
using System;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace NIT
{
    public class MyNetWork
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public TcpClient Client;
        AsynchronousClient obj;
        public MyNetWork(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
            Client = new TcpClient();
            Client.Connect(Hostname, Port);
            obj = new AsynchronousClient(Hostname, Port);

        }

        public void Reconnect()
        {
            Client = new TcpClient();
            Client.Connect(Hostname, Port);
        }

        public string Register(string userName, string passWord)
        {
            if (!Client.Connected)
            {
                Reconnect();

            }
           // Console.WriteLine($"[]");
           return Send($"register {userName} {passWord}");
        }

        public DataTable GetReport(string ctId)
        {
            if (!Client.Connected)
            {
                Reconnect();

            }
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("param 1", typeof(string));
            dataTable.Columns.Add("param 2", typeof(string));
            dataTable.Columns.Add("param 3", typeof(string));
            dataTable.Columns.Add("param 4", typeof(string));
            dataTable.Columns.Add("param 5", typeof(string));
            var result = Send($"report {ctId}");
            var data = result.Split(null);
            for (int i = 0; i < data.Length -6; i += 6)
            {
                dataTable.Rows.Add(data[i], data[1 + 1], data[i + 2], data[i + 3], data[i + 5]);

            }
            return dataTable;
        }

        public string Submit(string userName, string passWord, string filePath, string ctId)
        {
           
            string type = "";
            Console.WriteLine(Path.GetExtension(filePath));
            switch (Path.GetExtension(filePath)?.ToLower())
            {
                case ".c":
                    type = "c";
                    break;
                case ".cpp":
                    type = "c++";
                    break;
                case ".py":
                    type = "py";
                    break;
                case ".py3":
                    type = "py3";
                    break;
                case ".sh":
                    type = "sh";
                    break;
                case ".java":
                    type = "java";
                    break;
            }




            //  Console.WriteLine(request);
            try
            {
                string str;
                //using (NetworkStream stream = Client.GetStream())
                //{
                //    MessageBox.Show($"submit {userName} {passWord} {ctId} {type} EOF");

                //    var dataOne = Encoding.ASCII.GetBytes($"submit {userName} {passWord} {ctId} {type} EOF");
                //    var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);
                //    var eOf = Encoding.ASCII.GetBytes("EOF");
                //    stream.Write(dataOne, 0, dataOne.Length);
                //    stream.Write(newLine, 0, newLine.Length);
                //    Client.Client.SendFile(filePath);
                //    stream.Write(eOf, 0, eOf.Length);
                //    stream.Write(newLine, 0, newLine.Length);


                //    byte[] data1 = new byte[1024];
                //    using (MemoryStream ms = new MemoryStream())
                //    {


                //        int numBytesRead;
                //        while ((numBytesRead = stream.Read(data1, 0, data1.Length)) > 0)
                //        {
                //            ms.Write(data1, 0, numBytesRead);


                //        }
                //        str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                //    }
                //}
                obj = new AsynchronousClient(Hostname, Port);
                str=  obj.StartClient(userName, passWord,filePath, ctId, type);


                Console.WriteLine($"[{str}]");
                
               return str;


            }
            catch (Exception e)
            {
              return e.Message;

            }
        }

        public string Send(string request)
        {
            try
            {
                string str;
                using (NetworkStream stream = Client.GetStream())
                {
                    var data = Encoding.ASCII.GetBytes(request);
                    stream.Write(data, 0, data.Length);
                    var newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    stream.Write(newline, 0, newline.Length);

                    byte[] data1 = new byte[1024];
                    using (MemoryStream ms = new MemoryStream())
                    {

                        int numBytesRead;
                        while ((numBytesRead = stream.Read(data1, 0, data1.Length)) > 0)
                        {
                            ms.Write(data1, 0, numBytesRead);


                        }
                        str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                    }
                }

             //   Console.WriteLine(str);

                return str;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }
    }
}