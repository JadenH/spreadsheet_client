using System;
using System.Drawing;
using System.Dynamic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SS;

namespace ControllerTester
{
    [TestClass]
    public class ControllerTester : ISpreadsheetView
    {

        /// <summary>
        /// Test that the selected cell changes.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            Controller controller = new Controller(this);
            CellSelectionChange.Invoke(0,1);
            
            Assert.AreEqual(controller._selectedCell.CellColumn, 0);
            Assert.AreEqual(controller._selectedCell.CellRow, 1);
            Assert.AreEqual(controller._selectedCell.CellName, "A2");
        }

        /// <summary>
        /// Test setting cell contents and getting them from the selected cell.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A2", "testing");

            Assert.AreEqual(controller._selectedCell.CellColumn, 0);
            Assert.AreEqual(controller._selectedCell.CellRow, 0);
            Assert.AreEqual(controller._selectedCell.GetCellValue(s), "");

            CellSelectionChange(0, 1);

            Assert.AreEqual(controller._selectedCell.CellColumn, 0);
            Assert.AreEqual(controller._selectedCell.CellRow, 1);
            Assert.AreEqual(controller._selectedCell.GetCellValue(s), "testing");
        }

        /// <summary>
        /// Test changing the selected cell value.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            controller.ChangeSpreadsheet(s);
            CellValueBoxTextChange.Invoke("=1+1");

            Assert.AreEqual("2", controller._selectedCell.GetCellValue(s));
        }

        /// <summary>
        /// Test closing a document fails since we can't press ok on the confirm dialog.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            controller.ChangeSpreadsheet(s);
            bool test = HandleClose.Invoke();

            Assert.AreEqual(false, test);
        }

        /// <summary>
        /// Test opening a new document. Should run set selection to set the default selected cell.
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            Controller controller = new Controller(this);
            CreateNew.Invoke();
            Assert.IsTrue(_setSelectionRan);
        }

//        /// <summary>
//        /// Test creating a new document.
//        /// </summary>
//        [TestMethod]
//        public void TestMethod6()
//        {
//            Controller controller = new Controller(this);
//            Spreadsheet s = new Spreadsheet();
//            controller.ChangeSpreadsheet(s);
//            CreateNew.Invoke();
//
//            Assert.IsTrue(_setSelectionRan);
//        }

        /// <summary>
        /// Test changing the saved path.
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            controller.ChangeSpreadsheet(s);
            controller.SetSavedPath("testing.ss");

            Assert.AreEqual("testing", _title);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            DoClose();

            Assert.IsTrue(_closed);
        }

        public event Func<bool> HandleClose;
        public event Action<string> CellValueBoxTextChange;
        public event Action<int, int> CellSelectionChange;
        public event Action CreateNew;
        public event Action HandleOpen;
        public event Action HandleSave;
        public event Action HandleSaveAs;
        public event Action HandleHelp;
        public string InfoBarText { get; set; }
        public Color InfoBarColor { get; set; }
        public string SelectedCellText { get; set; }
        public string CellValueBoxText { get; set; }

        private bool _setSelectionRan;
        public void SetSelection(int col, int row)
        {
            _setSelectionRan = true;
        }

        private bool _updateCellRan;
        public void UpdateCell(int col, int row, string value)
        {
        }

        private bool _closed;
        public void DoClose()
        {
            bool cancel = HandleClose.Invoke();
            if (!cancel) _closed = true;

        }

        private string _title;
        public void SetTitle(string spreadsheetName)
        {
            _title = spreadsheetName;
        }
    }
}