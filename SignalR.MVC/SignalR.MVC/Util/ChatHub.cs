using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Concurrent;

namespace SignalR.MVC.Util
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {

        static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        public void Send(string name, string message)
        {

            // Call the addNewMessageToPage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void Private(string toUser, string name, string message)
        {

            Clients.Caller.broadcastMessage(name, message);
            if (!string.IsNullOrEmpty(toUser))
            {
                Clients.Client(dic[toUser]).broadcastMessage(name, message);
            }

        }

        public void Notify(string name, string id)
        {
            if (dic.ContainsKey(name))
            {
                Clients.Caller.differentName();
            }
            else
            {
                dic.TryAdd(name, id);

                foreach (KeyValuePair<String, String> entry in dic)
                {
                    Clients.Caller.online(entry.Key);
                }

                Clients.Others.enters(name);
            }
        }

        public void Online(string name, string id)
        {
           

                    dic.TryAdd(name, id);
                    foreach (KeyValuePair<String, String> entry in dic)
                    {
                        Clients.Caller.online(entry.Key);
                    }


                    Clients.Others.enters(name);
               
               
           
            
        }

        public void Disconnected(string name)
        {

            string s;
            dic.TryRemove(dic.FirstOrDefault(x => x.Key == name).Key, out s);
            Clients.All.disconnected(dic.FirstOrDefault(x => x.Key == name).Key);
        }

    }
}