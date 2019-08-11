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
            Dim sales As New List(Of SalesData)() From { _
                New SalesData(0, -1, "Western Europe", 0.7), _
                New SalesData(1, 0, "Austria", 0.92), _
                New SalesData(2, 0, "Belgium", 0.16), _
                New SalesData(3, 0, "Denmark", 0.56), _
                New SalesData(4, 0, "Finland", 0.44), _
                New SalesData(5, 0, "France", 0.51), _
                New SalesData(6, 0, "Germany", 0.93), _
                New SalesData(7, 0, "Greece", 0.11), _
                New SalesData(9, 0, "Italy", 0.22), _
                New SalesData(11, 0, "Netherlands", 0.85), _
                New SalesData(12, 0, "Norway", 0.7), _
                New SalesData(13, 0, "Portugal", 0.5), _
                New SalesData(14, 0, "Spain", 0.82), _
                New SalesData(15, 0, "Switzerland", 0.14), _
                New SalesData(16, 0, "United Kingdom", 0.91), _
                New SalesData(17, -1, "Eastern Europe", 0.62), _
                New SalesData(18, 17, "Belarus", 0.34), _
                New SalesData(19, 17, "Bulgaria", 0.8), _
                New SalesData(20, 17, "Croatia", 0.29), _
                New SalesData(21, 17, "Czech Republic", 0.13), _
                New SalesData(22, 17, "Hungary", 0.14), _
                New SalesData(23, 17, "Poland", 0.52), _
                New SalesData(24, 17, "Romania", 0.3), _
                New SalesData(25, 17, "Russia", 0.85), _
                New SalesData(26, -1, "North America", 0.84), _
                New SalesData(27, 26, "USA", 0.87), _
                New SalesData(28, 26, "Canada", 0.64), _
                New SalesData(29, -1, "South America", 0.32), _
                New SalesData(30, 29, "Argentina", 0.88), _
                New SalesData(31, 29, "Brazil", 0.1), _
                New SalesData(32, -1, "Asia", 0.52), _
                New SalesData(34, 32, "India", 0.44), _
                New SalesData(35, 32, "Japan", 0.7), _
                New SalesData(36, 32, "China", 0.82) _
            }
            Return sales
        End Function
        Public Shared Function CreateHierarchicalReport() As XtraReport
            Dim report As New XtraReport() With { _
                .DataSource = GetDataSource(), _
                .StyleSheet = { _
                    New XRControlStyle() With { _
                        .Name = "CaptionStyle", _
                        .Font = New Font("Tahoma", 14F), _
                        .BackColor = Color.Gray, _
                        .ForeColor = Color.White _
                    }, _
                    New XRControlStyle() With { _
                        .Name = "EvenStyle", _
                        .BackColor = Color.LightGray _
                    } _
                } _
            }
            Dim pageHeaderBand = CreatePageHeader()
            Dim detailBand = CreateDetail()
            report.Bands.AddRange(New Band() { pageHeaderBand, detailBand })
            Return report
        End Function
        Private Shared Function CreatePageHeader() As PageHeaderBand
            Dim pageHeaderBand As New PageHeaderBand() With { _
                .Name = "PageHeader", _
                .HeightF = 40, _
                .StyleName = "CaptionStyle", _
                .Padding = New PaddingInfo(5, 5, 0, 0) _
            }
            Dim regionCaption As New XRLabel() With { _
                .Name = "RegionCaptionLabel", _
                .Text = "Region", _
                .BoundsF = New RectangleF(0, 0, 475, 40), _
                .TextAlignment = TextAlignment.MiddleLeft _
            }
            Dim marketShareCaption As New XRLabel() With { _
                .Name = "MarketShareCaptionLabel", _
                .Text = "MarketShare", _
                .BoundsF = New RectangleF(475, 0, 175, 40), _
                .TextAlignment = TextAlignment.MiddleRight _
            }
            pageHeaderBand.Controls.AddRange(New XRControl() { regionCaption, marketShareCaption })
            Return pageHeaderBand
        End Function
        Private Shared Function CreateDetail() As DetailBand
            Dim detailBand As New DetailBand() With { _
                .Name = "Detail", _
                .HeightF = 25, _
                .EvenStyleName = "EvenStyle", _
                .Padding = New PaddingInfo(5, 5, 0, 0), _
                .Font = New Font("Tahoma", 9F), _
                .ExpressionBindings = { New ExpressionBinding("Font.Bold", "[DataSource.CurrentRowHierarchyLevel] == 0") }, _
                .SortFields = { New GroupField("Region", XRColumnSortOrder.Ascending) } _
            }
            ' Specify Id-ParentID related fields
            detailBand.HierarchyPrintOptions.KeyFieldName = "RegionID"
            detailBand.HierarchyPrintOptions.ParentFieldName = "ParentRegionID"
            ' Specify the child node offset
            detailBand.HierarchyPrintOptions.Indent = 25

            ' Add an XRCheckBox control as the DetailBand's drill-down controll to allow end users to collapse and expand tree nodes
            Dim expandButton As New XRCheckBox() With { _
                .Name = "DrillDownCheckBox", _
                .ExpressionBindings = { New ExpressionBinding("Checked", "[ReportItems].[" & detailBand.Name & "].[DrillDownExpanded]") }, _
                .BoundsF = New RectangleF(0, 0, 25, 25) _
            }
            Dim checkedSvg As SvgImage = SvgImage.FromResources("CreateHierarchicalReportInCode.Expanded.svg", GetType(ReportCreator).Assembly)
            Dim uncheckedSvg As SvgImage = SvgImage.FromResources("CreateHierarchicalReportInCode.Collapsed.svg", GetType(ReportCreator).Assembly)
            expandButton.GlyphOptions.Alignment = HorzAlignment.Center
            expandButton.GlyphOptions.Size = New SizeF(16, 16)
            expandButton.GlyphOptions.CustomGlyphs(CheckState.Checked) = New ImageSource(checkedSvg)
            expandButton.GlyphOptions.CustomGlyphs(CheckState.Unchecked) = New ImageSource(uncheckedSvg)
            detailBand.DrillDownControl = expandButton
            detailBand.DrillDownExpanded = False

            Dim regionLabel As New XRLabel() With { _
                .Name = "RegionLabel", _
                .ExpressionBindings = { New ExpressionBinding("Text", "[Region]") }, _
                .BoundsF = New RectangleF(25, 0, 450, 25), _
                .TextAlignment = TextAlignment.MiddleLeft, _
                .AnchorHorizontal = HorizontalAnchorStyles.Both _
            }
            Dim marketShareLabel As New XRLabel() With { _
                .Name = "MarketShareLabel", _
                .ExpressionBindings = { New ExpressionBinding("Text", "[MarketShare]") }, _
                .TextFormatString = "{0:0%}", _
                .BoundsF = New RectangleF(475, 0, 175, 25), _
                .TextAlignment = TextAlignment.MiddleRight, _
                .AnchorHorizontal = HorizontalAnchorStyles.Right _
            }
            detailBand.Controls.AddRange(New XRControl() { expandButton, regionLabel, marketShareLabel })
            Return detailBand
        End Function
    End Class
End Namespace
