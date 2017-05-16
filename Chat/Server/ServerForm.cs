using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Chat
{
    public partial class ServerForm : Form
    {
                
        public ServerForm()
        {
            InitializeComponent();
        }
                
        private void btnListen_Click(object sender, EventArgs e)
        {
            int portNum = 8888;
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");          
            Server mainServer = new Server(ipAddr);
            mainServer.StartListening(portNum);
            txtLog.Text = "Сервер запущен! " + "\r\n";
        }
       
        private void btnClose_Click(object sender, EventArgs e)
        {
                TcpListener closeConnection;
                int portNum = 8888;
                IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
                closeConnection = new TcpListener(ipAddr, portNum);
                closeConnection.Stop();               
                Application.Exit();              
        }
              
    }
}
