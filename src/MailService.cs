using System;
using src.Interfaces;
using src.Models;

namespace src
{
    public class MailService : IMessageService
    {
        public void SendErrorMessage(string subject, string errorMessage, NodeState state)
        {
            throw new NotImplementedException();
        }
    }
}