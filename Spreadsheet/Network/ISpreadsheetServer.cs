using System;

namespace SpreadsheetGUI
{
    public interface ISpreadsheetServer
    {
        Action<string> MessageReceived { get; set; }
        void SendMessage(string s);
    }
}