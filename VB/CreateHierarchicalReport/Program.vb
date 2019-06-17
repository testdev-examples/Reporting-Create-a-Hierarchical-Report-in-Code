Imports DevExpress.XtraReports.UI
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace CreateHierarchicalReportInCode
	Friend Class Program
		<STAThread>
		Shared Sub Main(ByVal args() As String)
			Dim report As XtraReport = ReportCreator.CreateHierarchicalReport()

			Dim printTool As New ReportPrintTool(report)
			printTool.ShowRibbonPreviewDialog()
		End Sub
	End Class
End Namespace
