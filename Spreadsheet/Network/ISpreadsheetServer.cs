using System;

namespace SpreadsheetGUI
{
    public interface ISpreadsheetServer
    {
        event Action<string> MessageReceived;
        void SendMessage(string s);
    }
}