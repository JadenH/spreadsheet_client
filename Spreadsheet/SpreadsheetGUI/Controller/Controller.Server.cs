﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SpreadsheetGUI
{
    public partial class Controller
    {
        public void MessageReceived(string message)
        {

            var items = message.Split('\t');
            switch (items.FirstOrDefault())
            {
                case "Change":
                case "hange":
                    UpdateCells(Spreadsheet.SetContentsOfCell(items[1], items[2]));
                    break;
                case "Startup":
                case "tartup":
                    MyClientId = items[1];
                    for (int i = 2; i + 1 < items.Length; i += 2)
                    {
                        UpdateCells(Spreadsheet.SetContentsOfCell(items[i], items[i + 1]));
                    }
                    IsTyping();
                    break;
                case "IsTyping":
                case "sTyping":
                    SetCellTyping(items[1], items[2]);
                    break;
                case "DoneTyping":
                case "oneTyping":
                    DoneTyping(items[1], items[2]);
                    break;
            }
        }

        public void SendCell(string cellName, string value)
        {
            _server.SendMessage($"Edit\t{cellName}\t{value}\t");
        }

        public void Undo()
        {
            _server.SendMessage($"Undo\t");
        }

        public void IsTyping()
        {
            if (SelectedCell.CellName.Length > 0)
            {
                _server.SendMessage($"IsTyping\t{MyClientId}\t{SelectedCell.CellName}\t");
            }
        }

        public void DoneTyping()
        {
            if (SelectedCell.CellName.Length > 0)
            {
                _server.SendMessage($"DoneTyping\t{MyClientId}\t{SelectedCell.CellName}\t");
            }
        }
    }
}