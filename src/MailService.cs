using System;
using src.Interfaces;
using src.Models;

namespace src
{
    public class MailService : IMessageService
    {
        public void SendMessage(string subject, string errorMEssage, ExpectedNodeState expectedState)
        {
            throw new NotImplementedException();
        }
    }
}