using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Chat
{
    public partial class ClientForm : Form
    {
      
        private string _userName;
        private StreamWriter _swSender;
        
        private StreamReader _srReceiver;
        
        private TcpClient _tcpServer;
        
        private delegate void UpdateLogCallback(string strMessage);
        private delegate void UpdateTextBoxCallback(string strName);  
        private delegate void CloseConnectionCallback(string strReason);
        private Thread _thrMessaging;
        
        private IPAddress _ipAddr;
        private bool _Connected;
        public int portNum;
        List <string> Clients = new List<string>();

        public ClientForm()
        {            
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }

        
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (_Connected == true)
            {               
                _Connected = false;
                _swSender.Close();
                _srReceiver.Close();
                _tcpServer.Close();
            }
        }

        private void bConnect_Click(object sender, EventArgs e)
        {           
            InitializeConnection();               
        }

        private void InitializeConnection()
        {
           
            _ipAddr = IPAddress.Parse("127.0.0.1");
            portNum = 8888;            
            _tcpServer = new TcpClient();
            _tcpServer.Connect(_ipAddr, portNum);
            _Connected = true;            
            _userName = txtUser.Text;
           
            txtUser.Enabled = false;
            txtMessage.Enabled = true;
            bSend.Enabled = true;
            textBox1.Enabled = true;          
            
            _swSender = new StreamWriter(_tcpServer.GetStream());
            _swSender.WriteLine(txtUser.Text);           
            _swSender.Flush();
           
           
            _thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
           
            _thrMessaging.Start();
        }

        private void ReceiveMessages()
        {
            string name = null;
            bool root = true;
            _srReceiver = new StreamReader(_tcpServer.GetStream());
            
            string ConResponse = _srReceiver.ReadLine();

            if (ConResponse[0] == '1')
            {
                this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { "Успешно подключен!" });               
            }
            else 
            {
                this.Invoke(new CloseConnectionCallback(this.CloseConnection), new object[] { "НЕ ПОДКЛЮЧЕН" });                
                return;
            }
            name = _srReceiver.ReadLine();
            while (_Connected)
            {
                if (root)
                {
                    this.Invoke(new UpdateTextBoxCallback(this.UpdateBox), new object[] { name });
                }
                root = false;
                this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { _srReceiver.ReadLine() });               
            }
        }


        private void UpdateLog(string strMessage)
        {
            txtLog.AppendText(strMessage + "\r\n");            
        }

        private void UpdateBox(string UserName1)
        {
            textBox1.AppendText(UserName1 + "\r\n"); 
        }  

        private void CloseConnection(string Reason)
        {
            
            txtLog.AppendText(Reason + "\r\n");            
            txtUser.Enabled = true;
            txtMessage.Enabled = false;
            bSend.Enabled = false;
           
            _Connected = false;
            _swSender.Close();
            _srReceiver.Close();
            _tcpServer.Close();
        }

        
        private void SendMessage()
        {
            if (txtMessage.Lines.Length >= 1)
            {
                _swSender.WriteLine(txtMessage.Text);
                _swSender.Flush();
                txtMessage.Lines = null;
            }
            txtMessage.Text = "";
        }

       

        private void bSend_Click(object sender, EventArgs e)
        {
            SendMessage();           
        }

        //private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        //{          
            
        //    if (e.KeyChar == (char)13)
        //    {
        //        SendMessage();
        //    }
        //}

        private void bClose_Click(object sender, EventArgs e)
        {
            if (_Connected == true)
            {                
                _Connected = false;
                _swSender.Close();
                _srReceiver.Close();
                _tcpServer.Close();
            }        
            Close();
         }

      

            

       

        
    }
}
