using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Contracts;

namespace Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class ChatService : IChatService
    {
        private static Dictionary<string, IChatClientCallback> _callbacks = new Dictionary<string, IChatClientCallback>();

        public ICollection<string> GetOnlineUsers()
        {
            return _callbacks.Keys;
        }

        public void Login(string name)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IChatClientCallback>();
            _callbacks[name] = callback;
            foreach (var client in _callbacks)
                client.Value.OnLogin(new LoginMessage { Sender = name });
        }

        public void Logoff(string name)
        {
            foreach (var client in _callbacks)
                client.Value.OnLogoff(new LogoffMessage { Sender = name });
            _callbacks.Remove(name);
        }

        public void SendMessage(string name, string content)
        {
            foreach (var client in _callbacks)
                client.Value.ReceiveMessage(new ChatMessage { Sender = name, Content = content });
        }
    }
}
