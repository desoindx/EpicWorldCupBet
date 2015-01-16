using System;
using System.Collections.Generic;
using SignalR.SQL;

namespace SignalR
{
    public static class Chats
    {
        private static readonly object Locker = new object();
        private static readonly Dictionary<int, Chat> _chats = new Dictionary<int, Chat>(); 

        public static string[] NewMessage(int universeId, string user, string message)
        {
            if (!Sql.IsUserAuthorizedOn(user, universeId))
                return null;

            lock (Locker)
            {
                Chat chat;
                if (!_chats.TryGetValue(universeId, out chat))
                {
                    chat = new Chat();
                    _chats[universeId] = chat;
                }

                return chat.NewMessage(user, message.Trim());
            }
        }
        public static List<string[]> GetChat(int universeId)
        {
            lock (Locker)
            {
                Chat chat;
                if (!_chats.TryGetValue(universeId, out chat))
                {
                    chat = new Chat();
                    _chats[universeId] = chat;
                }

                return chat.Messages;
            }
        }

        private class Chat
        {
            public List<string[]> Messages { get { return new List<string[]>(_messages); } }
            private readonly List<string[]> _messages = new List<string[]>();

            public string[] NewMessage(string user, string message)
            {
                if (string.IsNullOrEmpty(message))
                    return null;

                var lastMessage = new[] { string.Format("{0} ({1}) -", user, DateTime.Now.ToShortTimeString()), message };
                _messages.Add(lastMessage);
                return lastMessage;
            }
        }
    }
}