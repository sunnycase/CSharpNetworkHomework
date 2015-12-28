using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatClientCallback))]
    public interface IChatService
    {
        [OperationContract(IsOneWay = true)]
        void Login(string name);

        [OperationContract(IsOneWay = true)]
        void Logoff(string name);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string name, string content);

        [OperationContract]
        ICollection<string> GetOnlineUsers();
    }

    public interface IChatClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(ChatMessage message);

        [OperationContract(IsOneWay = true)]
        void OnLogin(LoginMessage message);

        [OperationContract(IsOneWay = true)]
        void OnLogoff(LogoffMessage message);
    }
    
    [MessageContract]
    public class ChatMessage
    {
        [MessageHeader]
        public string Sender { get; set; }

        [MessageBodyMember]
        public string Content { get; set; }
    }

    [MessageContract]
    public class LoginMessage
    {
        [MessageHeader]
        public string Sender { get; set; }
    }

    [MessageContract]
    public class LogoffMessage
    {
        [MessageHeader]
        public string Sender { get; set; }
    }
}
