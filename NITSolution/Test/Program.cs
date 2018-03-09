using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Test
{
    public class MyNetWork
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public TcpClient Client;

        public MyNetWork(string hostname,int port)
        {
            Hostname = hostname;
            Port = port;
            Client = new TcpClient();
            Client.Connect(Hostname,Port);

        }


        public void Register(string userName, string passWord)
        {
            Console.WriteLine($"[{Send($"register {userName} {passWord}")}]");
            
        }

        public void GetReport(string ctId)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("param 1", typeof(string));
            dataTable.Columns.Add("param 2", typeof(string));
            dataTable.Columns.Add("param 3", typeof(string));
            dataTable.Columns.Add("param 4", typeof(string));
            dataTable.Columns.Add("param 5", typeof(string));
            var result = Send($"report ct{ctId}");
            var data = result.Split(null);
            for (int i = 0; i <= data.Length-6; i+=6)
            {
                dataTable.Rows.Add(data[i], data[1 + 1], data[i + 2], data[i + 3], data[i + 5]);

            }
            Console.WriteLine(dataTable.Rows.Count);
            Console.WriteLine(data.Length);
            Console.WriteLine(result);
        }

        public void Submit(string userName, string passWord, string filePath, string ctId)
        {
            if (!Client.Connected)
            {
                Client = new TcpClient();
                Client.Connect(Hostname, Port);
            }
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
                using (NetworkStream stream = Client.GetStream())
                {
                  
                    var dataOne = Encoding.ASCII.GetBytes($"submit {userName} {passWord} {ctId} {type} EOF");
                    var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);
                    var eOf = Encoding.ASCII.GetBytes("EOF");
                    stream.Write(dataOne,0,dataOne.Length);
                    stream.Write(newLine,0,newLine.Length);
                    Client.Client.SendFile(filePath);
                    stream.Write(eOf,0,eOf.Length);
                    stream.Write(newLine, 0, newLine.Length);
                    

                    byte[] data1 = new byte[1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Console.WriteLine("reading {0}",ms.Length);

                        int numBytesRead;
                        while ((numBytesRead = stream.Read(data1, 0, data1.Length)) > 0)
                        {
                            ms.Write(data1, 0, numBytesRead);

                            Console.WriteLine("1");
                        }
                        str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                    }
                }

                Console.WriteLine($"[{str}]");

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
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
    class Program
    {
        static void Main(string[] args)
        {

         
            

                MyNetWork myNetWork = new MyNetWork("ct.rudi.ir",40);

           
                myNetWork.Submit("temp12", "testtemp", "main.c","ct26");
              


            Console.Read();

        }
    }
}
