using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateHierarchicalReportInCode {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            XtraReport report = ReportCreator.CreateHierarchicalReport();

            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowRibbonPreviewDialog();
        }
    }
}
