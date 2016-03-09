using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Windows.Forms;
using Formulas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using SS;

namespace ControllerTester
{
    [TestClass]
    [ExcludeFromCodeCoverage]
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
            
            Assert.AreEqual(controller.SelectedCell.CellColumn, 0);
            Assert.AreEqual(controller.SelectedCell.CellRow, 1);
            Assert.AreEqual(controller.SelectedCell.CellName, "A2");
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

            Assert.AreEqual(controller.SelectedCell.CellColumn, 0);
            Assert.AreEqual(controller.SelectedCell.CellRow, 0);
            Assert.AreEqual(controller.SelectedCell.GetCellValue(s), "");

            CellSelectionChange(0, 1);

            Assert.AreEqual(controller.SelectedCell.CellColumn, 0);
            Assert.AreEqual(controller.SelectedCell.CellRow, 1);
            Assert.AreEqual(controller.SelectedCell.GetCellValue(s), "testing");
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

            Assert.AreEqual("2", controller.SelectedCell.GetCellValue(s));
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

        /// <summary>
        /// Test creating a new document.
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            _setSelectionRan = false;
            SpreadsheetApplicationContext.GetContext().IsUnitTest = true; // Prevents it from trying to create a Window.
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            controller.ChangeSpreadsheet(s);
            CreateNew.Invoke();

            Assert.IsTrue(_setSelectionRan);
        }

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

        /// <summary>
        /// Test closing from controller.
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            DoClose();

            Assert.IsTrue(_closed);
        }

        static readonly string TestSaveFilePath1 = Environment.CurrentDirectory + "/test1.ss";
        static readonly string TestSaveFilePath2 = Environment.CurrentDirectory + "/test2.ss";

        /// <summary>
        /// Test saving a file through the controller.
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();
            controller.HandleSave(TestSaveFilePath1);

            Assert.IsTrue(File.Exists(TestSaveFilePath1));
        }

        /// <summary>
        /// Test opening (and saving) a file through the controller.
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            _cells = new Dictionary<Tuple<int, int>, string>();
            SpreadsheetApplicationContext.GetContext().IsUnitTest = true; // Prevents it from trying to create a Window.
            Controller controller = new Controller(this);
            Spreadsheet s = new Spreadsheet();

            s.SetContentsOfCell("A1", "testing!");

            controller.ChangeSpreadsheet(s);
            controller.HandleSave(TestSaveFilePath2);
            controller.HandleOpen(TestSaveFilePath2);

            //Test file exists and was open.
            Assert.IsTrue(File.Exists(TestSaveFilePath1));
            Assert.AreEqual("test2", _title);

            Assert.AreEqual("testing!", controller.Spreadsheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Test setting the cell value bar to an invalid formula. The cell value should not have changed.
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            SpreadsheetApplicationContext.GetContext().IsUnitTest = true; // Prevents it from trying to create a Window.
            Controller controller = new Controller(this);
            CellValueBoxTextChange.Invoke("=((");

            Assert.AreEqual(string.Empty, controller.SelectedCell.GetCellContents(controller.Spreadsheet));
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

        private Dictionary<Tuple<int, int>, string> _cells;   
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