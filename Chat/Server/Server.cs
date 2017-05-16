using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace Chat
{  

    class Server
    {        
        internal static Hashtable htUsers = new Hashtable(100);         
        public static Hashtable htConnections = new Hashtable(100);        
        private IPAddress _ipAddress;
        private TcpClient _tcpClient;     
        
       

        public Server(IPAddress address)
        {
            _ipAddress = address;
        }
        
        
        private Thread thrListener;       
        private TcpListener tlsClient;       
        bool ServRunning = false;

       
        public static void AddUser(TcpClient tcpUser, string strUsername)
        {            
            Server.htUsers.Add(strUsername, tcpUser);
            Server.htConnections.Add(tcpUser, strUsername);
            SendAdminMessage(htConnections[tcpUser] + " подключился к серверу");
        }      

        
        public static void RemoveUser(TcpClient tcpUser)
        {            
            if (htConnections[tcpUser] != null)
            {
                SendAdminMessage(htConnections[tcpUser] + " отключился от сервера");                
                Server.htUsers.Remove(Server.htConnections[tcpUser]);
                Server.htConnections.Remove(tcpUser);
            }
        }
        
        internal static void SendAdminMessage(string Message)
        {
            StreamWriter swSenderSender;            
            
            TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];           
            Server.htUsers.Values.CopyTo(tcpClients, 0);            
            for (int i = 0; i < tcpClients.Length; i++)
            {              
                try
                {                     
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine(Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch 
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }
        
        public static void SendMessage(string From, string Message)
        {
            StreamWriter swSenderSender;            
                        
            TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];            
            Server.htUsers.Values.CopyTo(tcpClients, 0);           
            for (int i = 0; i < tcpClients.Length; i++)
            {                
                try
                {                  
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());                    
                    swSenderSender.WriteLine(From + ": " + Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch 
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }


        public void StartListening(int portNum)
        {
            IPAddress ipLocal = _ipAddress;            
            tlsClient = new TcpListener(ipLocal,portNum);            
            tlsClient.Start();            
            ServRunning = true;            
            thrListener = new Thread(KeepListening);
            thrListener.Start();
        }

        private void KeepListening()
        {
            while (ServRunning == true)
            {
                _tcpClient = tlsClient.AcceptTcpClient();
                Connection newConnection = new Connection(_tcpClient);
            }
        }
    }
}
