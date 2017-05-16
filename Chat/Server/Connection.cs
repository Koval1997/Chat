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
    class Connection
    {
        private TcpClient tcpClient { get; set; }
        private Thread thrSender { get; set; }
        private StreamReader srReceiver { get; set; }
        private StreamWriter swSender { get; set; }
        private string currUser { get; set; }
        private string strResponse { get; set; }

       
        public Connection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;
            thrSender = new Thread(AcceptClient);            
            thrSender.Start();
        }

        private void CloseConnection()
        {
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
        }

        
        private void AcceptClient()
        {
            srReceiver = new StreamReader(tcpClient.GetStream());
            swSender = new StreamWriter(tcpClient.GetStream());

           
            currUser = srReceiver.ReadLine();
            
            
            if (currUser != "")
            {
                
                if (Server.htUsers.Contains(currUser) == true)
                {
                    
                    swSender.WriteLine("0");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currUser == "")
                {
                    
                    swSender.WriteLine("0");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else
                {
                    
                    swSender.WriteLine("1");
                    swSender.Flush();

                    Server.AddUser(tcpClient, currUser);                    

                }
            }
            else
            {
                CloseConnection();
                return;
            }

            try
            {
                
                while ((strResponse = srReceiver.ReadLine()) != "")
                {
                    
                    if (strResponse == null)
                    {
                        Server.RemoveUser(tcpClient);
                    }
                    else
                    {
                        
                        Server.SendMessage(currUser, strResponse);
                    }
                }
            }
            catch
            {
                
                Server.RemoveUser(tcpClient);
            }
        }
    }
}
