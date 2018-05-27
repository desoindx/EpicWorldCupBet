using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    public class MessageDetail
    {
        public string UserName { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
    }

    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }

    public class ChatHub : Hub
    {
        #region Data Members

        static readonly Dictionary<string, HashSet<string>> ConnectedUsers = new Dictionary<string, HashSet<string>>();
        static List<MessageDetail> CurrentMessage;
        private readonly object _messageLock = new object();

        public ChatHub()
        {
            if (CurrentMessage != null)
            {
                return;
            }
            lock (_messageLock)
            {
                var chat = HttpRuntime.AppDomainAppPath + ("Chat/chat.xml");
                if (File.Exists(chat))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(MessageDetail[]));
                    using (StreamReader rd = new StreamReader(chat))
                    {
                        CurrentMessage = ((MessageDetail[])(xs.Deserialize(rd))).ToList();
                    }
                }
                else
                {
                    CurrentMessage = new List<MessageDetail>(100);
                }
            }
        }

        #endregion

        #region Methods

        public void Connect(string userName)
        {
            if (userName == null)
            {
                return;
            }

            lock (this)
            {
                var id = Context.ConnectionId;
                HashSet<string> connectionIds;
                if (!ConnectedUsers.TryGetValue(userName, out connectionIds) || !connectionIds.Any())
                {
                    connectionIds = new HashSet<string>();
                    ConnectedUsers[userName] = connectionIds;
                    Clients.AllExcept(id).onNewUserConnected(userName);
                }
                connectionIds.Add(id);
                Clients.Caller.onConnected(userName, ConnectedUsers.Keys, CurrentMessage);
            }
        }

        public void SendMessageToAll(string userName, string message)
        {
            var time = string.Format("{0:dd/MM - HH:mm}", DateTime.UtcNow.AddHours(1));
            Clients.All.messageReceived(userName, message, time);
            AddMessageinCache(userName, message, time);
        }

        public void SendPrivateMessage(string toUserName, string message)
        {
            lock (this)
            {
                string fromUserId = Context.ConnectionId;
                HashSet<string> toUserIds;
                if (ConnectedUsers.TryGetValue(toUserName, out toUserIds) && toUserIds.Any())
                {
                    var fromUser = ConnectedUsers.Where(x => x.Value.Contains(fromUserId)).Select(x => x.Key).FirstOrDefault();
                    if (fromUser != null)
                    {
                        foreach (var toUserId in toUserIds)
                        {
                            Clients.Client(toUserId).sendPrivateMessage(fromUser, toUserName, message, false);
                        }
                        Clients.Caller.sendPrivateMessage(fromUser, toUserName, message, true);
                    }
                }
            }
        }

        public override Task OnDisconnected(bool stop)
        {
            lock (this)
            {
                var connectionId = Context.ConnectionId;
                var item = ConnectedUsers.Where(x => x.Value.Contains(connectionId)).Select(x => x.Key).FirstOrDefault();
                if (item != null)
                {
                    var connectedUser = ConnectedUsers[item];
                    connectedUser.Remove(connectionId);
                    if (!connectedUser.Any())
                    {
                        Clients.All.onUserDisconnected(item);
                    }
                }
            }
            return base.OnDisconnected(stop);
        }


        #endregion

        #region private Messages

        private void AddMessageinCache(string userName, string message, string time)
        {
            lock (_messageLock)
            {
                CurrentMessage.Add(new MessageDetail { UserName = userName, Message = message, Time = time});
                if (CurrentMessage.Count > 100)
                {
                    CurrentMessage.RemoveAt(0);
                }
                var chat = HttpRuntime.AppDomainAppPath + ("Chat/chat.xml");
                if (File.Exists(chat))
                {
                    File.Delete(chat);
                }

                XmlSerializer xs = new XmlSerializer(typeof(MessageDetail[]));
                using (StreamWriter wr = new StreamWriter(chat))
                {
                    xs.Serialize(wr, CurrentMessage.ToArray());
                }
            }
        }

        #endregion
    }

}