Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.Utils
Imports DevExpress.Utils.Svg
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.Drawing
Imports DevExpress.XtraReports.UI

Namespace CreateHierarchicalReportInCode
    Public Class ReportCreator
        ' Define data records
        Public Class SalesData
            Public Sub New(ByVal regionId As Integer, ByVal parentRegionId As Integer, ByVal region As String, ByVal marketShare As Double)
                Me.RegionID = regionId
                Me.ParentRegionID = parentRegionId
                Me.Region = region
                Me.MarketShare = marketShare
            End Sub
            Public Property RegionID() As Integer
            Public Property ParentRegionID() As Integer
            Public Property Region() As String
            Public Property MarketShare() As Double
        End Class
        ' Get data records
        Public Shared Function GetDataSource() As List(Of SalesData)
            Dim sales As New List(Of SalesData)() From {
                New SalesData(0, -1, "Western Europe", 0.7),
                New SalesData(1, 0, "Austria", 0.92),
                New SalesData(2, 0, "Belgium", 0.16),
                New SalesData(3, 0, "Denmark", 0.56),
                New SalesData(4, 0, "Finland", 0.44),
                New SalesData(5, 0, "France", 0.51),
                New SalesData(6, 0, "Germany", 0.93),
                New SalesData(7, 0, "Greece", 0.11),
                New SalesData(9, 0, "Italy", 0.22),
                New SalesData(11, 0, "Netherlands", 0.85),
                New SalesData(12, 0, "Norway", 0.7),
                New SalesData(13, 0, "Portugal", 0.5),
                New SalesData(14, 0, "Spain", 0.82),
                New SalesData(15, 0, "Switzerland", 0.14),
                New SalesData(16, 0, "United Kingdom", 0.91),
                New SalesData(17, -1, "Eastern Europe", 0.62),
                New SalesData(18, 17, "Belarus", 0.34),
                New SalesData(19, 17, "Bulgaria", 0.8),
                New SalesData(20, 17, "Croatia", 0.29),
                New SalesData(21, 17, "Czech Republic", 0.13),
                New SalesData(22, 17, "Hungary", 0.14),
                New SalesData(23, 17, "Poland", 0.52),
                New SalesData(24, 17, "Romania", 0.3),
                New SalesData(25, 17, "Russia", 0.85),
                New SalesData(26, -1, "North America", 0.84),
                New SalesData(27, 26, "USA", 0.87),
                New SalesData(28, 26, "Canada", 0.64),
                New SalesData(29, -1, "South America", 0.32),
                New SalesData(30, 29, "Argentina", 0.88),
                New SalesData(31, 29, "Brazil", 0.1),
                New SalesData(32, -1, "Asia", 0.52),
                New SalesData(34, 32, "India", 0.44),
                New SalesData(35, 32, "Japan", 0.7),
                New SalesData(36, 32, "China", 0.82)
            }
            Return sales
        End Function
        Public Shared Function CreateHierarchicalReport() As XtraReport
            Dim report As New XtraReport() With {
                .DataSource = GetDataSource(),
                .StyleSheet = {
                    New XRControlStyle() With {
                        .Name = "CaptionStyle",
                        .Font = New Font("Tahoma", 14F),
                        .BackColor = Color.Gray,
                        .ForeColor = Color.White
                    },
                    New XRControlStyle() With {
                        .Name = "EvenStyle",
                        .BackColor = Color.LightGray
                    }
                }
            }
            Dim pageHeaderBand = CreatePageHeader()
            Dim detailBand = CreateDetail()
            report.Bands.AddRange(New Band() { pageHeaderBand, detailBand })
            Return report
        End Function
        Private Shared Function CreatePageHeader() As PageHeaderBand
            Dim pageHeaderBand As New PageHeaderBand() With {
                .Name = "PageHeader",
                .HeightF = 40,
                .StyleName = "CaptionStyle",
                .Padding = New PaddingInfo(5, 5, 0, 0)
            }
            Dim regionCaption As New XRLabel() With {
                .Name = "RegionCaptionLabel",
                .Text = "Region",
                .BoundsF = New RectangleF(0, 0, 475, 40),
                .TextAlignment = TextAlignment.MiddleLeft
            }
            Dim marketShareCaption As New XRLabel() With {
                .Name = "MarketShareCaptionLabel",
                .Text = "MarketShare",
                .BoundsF = New RectangleF(475, 0, 175, 40),
                .TextAlignment = TextAlignment.MiddleRight
            }
            pageHeaderBand.Controls.AddRange(New XRControl() { regionCaption, marketShareCaption })
            Return pageHeaderBand
        End Function
        Private Shared Function CreateDetail() As DetailBand
            Dim detailBand As New DetailBand() With {
                .Name = "Detail",
                .HeightF = 25,
                .EvenStyleName = "EvenStyle",
                .Padding = New PaddingInfo(5, 5, 0, 0),
                .Font = New Font("Tahoma", 9F),
                .ExpressionBindings = { New ExpressionBinding("Font.Bold", "[DataSource.CurrentRowHierarchyLevel] == 0") },
                .SortFields = { New GroupField("Region", XRColumnSortOrder.Ascending) }
            }
            ' Specify Id-ParentID related fields
            detailBand.HierarchyPrintOptions.KeyFieldName = "RegionID"
            detailBand.HierarchyPrintOptions.ParentFieldName = "ParentRegionID"
            ' Specify the child node offset
            detailBand.HierarchyPrintOptions.Indent = 25

            ' Add an XRCheckBox control as the DetailBand's drill-down controll to allow end users to collapse and expand tree nodes
            Dim expandButton As New XRCheckBox() With {
                .Name = "DrillDownCheckBox",
                .ExpressionBindings = { New ExpressionBinding("Checked", "[ReportItems].[" & detailBand.Name & "].[DrillDownExpanded]") },
                .BoundsF = New RectangleF(0, 0, 25, 25)
            }
            Dim checkedSvg As SvgImage = SvgImage.FromResources("CreateHierarchicalReportInCode.Expanded.svg", GetType(ReportCreator).Assembly)
            Dim uncheckedSvg As SvgImage = SvgImage.FromResources("CreateHierarchicalReportInCode.Collapsed.svg", GetType(ReportCreator).Assembly)
            expandButton.GlyphOptions.Alignment = HorzAlignment.Center
            expandButton.GlyphOptions.Size = New SizeF(16, 16)
            expandButton.GlyphOptions.CustomGlyphs(CheckState.Checked) = New ImageSource(checkedSvg)
            expandButton.GlyphOptions.CustomGlyphs(CheckState.Unchecked) = New ImageSource(uncheckedSvg)
            detailBand.DrillDownControl = expandButton
            detailBand.DrillDownExpanded = False

            Dim regionLabel As New XRLabel() With {
                .Name = "RegionLabel",
                .ExpressionBindings = { New ExpressionBinding("Text", "[Region]") },
                .BoundsF = New RectangleF(25, 0, 450, 25),
                .TextAlignment = TextAlignment.MiddleLeft,
                .AnchorHorizontal = HorizontalAnchorStyles.Both
            }
            Dim marketShareLabel As New XRLabel() With {
                .Name = "MarketShareLabel",
                .ExpressionBindings = { New ExpressionBinding("Text", "[MarketShare]") },
                .TextFormatString = "{0:0%}",
                .BoundsF = New RectangleF(475, 0, 175, 25),
                .TextAlignment = TextAlignment.MiddleRight,
                .AnchorHorizontal = HorizontalAnchorStyles.Right
            }
            detailBand.Controls.AddRange(New XRControl() { expandButton, regionLabel, marketShareLabel })
            Return detailBand
        End Function
    End Class
End Namespace
