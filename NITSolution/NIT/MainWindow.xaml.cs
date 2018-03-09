using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using SuperSocket.ClientEngine;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;
using Path = System.IO.Path;

namespace NIT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyNetWork MyNetWork = new  MyNetWork("ct.rudi.ir",40);
        public ClientSession Client;
        private string FileNameStr = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
         
           // Task.Factory.StartNew(Init);


        }

        public void Init()
        {
    
            
        
        }

        private void ClientOnConnected(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("Connect");
        }

        private void ClientOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            MessageBox.Show(errorEventArgs.Exception.Message);
        }

        private void ClientOnDataReceived(object sender, DataEventArgs dataEventArgs)
        {
            var data = Encoding.UTF8.GetString(dataEventArgs.Data, 0, dataEventArgs.Data.Length);
            MessageBox.Show(data);
        }

        private void Card_Drop(object sender, DragEventArgs e)
        {


            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
               
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var fileName = FileNameStr = files[0];
                FileNametxt.FontSize = 12;
                switch (Path.GetExtension(fileName)?.ToLower())
                {
                    case ".c":
                        c.IsChecked = true;
                        break;
                    case ".cpp":
                        cpp.IsChecked = true;
                        break;
                    case ".py":
                        py.IsChecked = true;
                        break;
                    case ".py3":
                        py3.IsChecked = true;
                        break;
                    case ".sh":
                        sh.IsChecked = true;
                        break;
                    case ".java":
                        java.IsChecked = true;
                        break;
                }
                FileNametxt.Text = fileName;
                submitBtn.IsEnabled = true;
             
            }
        }

        private  void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text == string.Empty)
            {
                submitStatus.Text = "check fields";
            }
            else
            {
                if (password.Text == string.Empty)
                {
                    submitStatus.Text = "check fields";
                }
                else
                {
                    if (cid.Text == string.Empty)
                    {
                        submitStatus.Text = "check fields";
                    }
                    else
                    {
                        Task.Factory.StartNew(submit);
                    }
                }
            }
           
            
           
        }

        public void submit()
        {
            var tempName = String.Empty;
            var tempPass = String.Empty;
            var tempCid = String.Empty;


            Dispatcher.Invoke(() =>
            {
                tempName = username.Text;
                tempPass = password.Text;
                tempCid = cid.Text;

            });
            var str = MyNetWork.Submit(tempName, tempPass, FileNameStr, tempCid);
            Dispatcher.Invoke(() => { submitStatus.Text = str; });
        }

        private void getreportBtn_Click(object sender, RoutedEventArgs e)
        {
           
            Task.Factory.StartNew(Report);
        }

        public void Report()
        {
            DataView data=null;
            Dispatcher.Invoke(() =>
            {
                data = MyNetWork.GetReport(reportTitle.Text).DefaultView;

            });
            Dispatcher.Invoke(() => { Grid.ItemsSource = data; });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Task.Factory.StartNew(Register);
            Register();
        }

        public void Register()
        {
            try
            {

                string name = "";
                string passWord = "";
                string outPut;
                Dispatcher.Invoke(() =>
                {
                    name = RegUserName.Text;
                    passWord = RegPassWord.Text;
                });
                outPut = MyNetWork.Register(name, passWord);
                // MessageBox.Show(outPut);
                Dispatcher.Invoke(() => { LogTextBlock.Text += $"{outPut} {Environment.NewLine}"; });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Dispatcher.Invoke(() => { LogTextBlock.Text += $"{e.Message} {Environment.NewLine}"; });
            }

           

        }

        private void OpenSource_Click(object sender, RoutedEventArgs e)
        {
            string targetURL = "https://github.com/includeamin/NITCT";
            System.Diagnostics.Process.Start(targetURL);
        }
    }
}
