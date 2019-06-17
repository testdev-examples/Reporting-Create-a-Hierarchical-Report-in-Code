using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;

namespace CreateHierarchicalReportInCode {
    public class ReportCreator {
        // Define data records
        public class SalesData {
            public SalesData(int regionId, int parentRegionId, string region, double marketShare) {
                RegionID = regionId;
                ParentRegionID = parentRegionId;
                Region = region;
                MarketShare = marketShare;
            }
            public int RegionID { get; set; }
            public int ParentRegionID { get; set; }
            public string Region { get; set; }
            public double MarketShare { get; set; }
        }
        // Get data records
        public static List<SalesData> GetDataSource() {
            List<SalesData> sales = new List<SalesData>() {
                new SalesData(0, -1, "Western Europe", 0.7),
                new SalesData(1, 0, "Austria", 0.92),
                new SalesData(2, 0, "Belgium", 0.16),
                new SalesData(3, 0, "Denmark", 0.56),
                new SalesData(4, 0, "Finland", 0.44),
                new SalesData(5, 0, "France", 0.51),
                new SalesData(6, 0, "Germany", 0.93),
                new SalesData(7, 0, "Greece", 0.11),
                new SalesData(9, 0, "Italy", 0.22),
                new SalesData(11, 0, "Netherlands", 0.85),
                new SalesData(12, 0, "Norway", 0.7),
                new SalesData(13, 0, "Portugal", 0.5),
                new SalesData(14, 0, "Spain", 0.82),
                new SalesData(15, 0, "Switzerland", 0.14),
                new SalesData(16, 0, "United Kingdom", 0.91),

                new SalesData(17, -1, "Eastern Europe", 0.62),
                new SalesData(18, 17, "Belarus", 0.34),
                new SalesData(19, 17, "Bulgaria", 0.8),
                new SalesData(20, 17, "Croatia", 0.29),
                new SalesData(21, 17, "Czech Republic", 0.13),
                new SalesData(22, 17, "Hungary", 0.14),
                new SalesData(23, 17, "Poland", 0.52),
                new SalesData(24, 17, "Romania", 0.3),
                new SalesData(25, 17, "Russia", 0.85),

                new SalesData(26, -1, "North America", 0.84),
                new SalesData(27, 26, "USA", 0.87),
                new SalesData(28, 26, "Canada", 0.64),

                new SalesData(29, -1, "South America", 0.32),
                new SalesData(30, 29, "Argentina", 0.88),
                new SalesData(31, 29, "Brazil", 0.1),

                new SalesData(32, -1, "Asia", 0.52),
                new SalesData(34, 32, "India", 0.44),
                new SalesData(35, 32, "Japan", 0.7),
                new SalesData(36, 32, "China", 0.82)
            };
            return sales;
        }
        public static XtraReport CreateHierarchicalReport() {
            XtraReport report = new XtraReport() {
                DataSource = GetDataSource(),
                StyleSheet = {
                    new XRControlStyle() { Name = "CaptionStyle", Font = new Font("Tahoma", 14f), BackColor = Color.Gray, ForeColor = Color.White },
                    new XRControlStyle() { Name = "EvenStyle", BackColor = Color.LightGray },
                }
            };
            var pageHeaderBand = CreatePageHeader();
            var detailBand = CreateDetail();
            report.Bands.AddRange(new Band[] { pageHeaderBand, detailBand });
            return report;
        }
        static PageHeaderBand CreatePageHeader() {
            PageHeaderBand pageHeaderBand = new PageHeaderBand() {
                Name = "PageHeader",
                HeightF = 40,
                StyleName = "CaptionStyle",
                Padding = new PaddingInfo(5, 5, 0, 0)
            };
            XRLabel regionCaption = new XRLabel() {
                Name = "RegionCaptionLabel",
                Text = "Region",
                BoundsF = new RectangleF(0, 0, 475, 40),
                TextAlignment = TextAlignment.MiddleLeft,
            };
            XRLabel marketShareCaption = new XRLabel() {
                Name = "MarketShareCaptionLabel",
                Text = "MarketShare",
                BoundsF = new RectangleF(475, 0, 175, 40),
                TextAlignment = TextAlignment.MiddleRight,
            };
            pageHeaderBand.Controls.AddRange(new XRControl[] { regionCaption, marketShareCaption });
            return pageHeaderBand;
        }
        static DetailBand CreateDetail() {
            DetailBand detailBand = new DetailBand() {
                Name = "Detail",
                HeightF = 25,
                EvenStyleName = "EvenStyle",
                Padding = new PaddingInfo(5, 5, 0, 0),
                Font = new Font("Tahoma", 9f),
                // Print root level nodes in bold
                ExpressionBindings = { new ExpressionBinding("Font.Bold", "[DataSource.CurrentRowHierarchyLevel] == 0") },
                // Sort data on each hierarchy level by the Region field
                SortFields = { new GroupField("Region", XRColumnSortOrder.Ascending) }
            };
            // Specify Id-ParentID related fields
            detailBand.HierarchyPrintOptions.KeyFieldName = "RegionID";
            detailBand.HierarchyPrintOptions.ParentFieldName = "ParentRegionID";
            // Specify the child node offset
            detailBand.HierarchyPrintOptions.Indent = 25;

            // Add an XRCheckBox control as the DetailBand's drill-down controll to allow end users to collapse and expand tree nodes
            XRCheckBox expandButton = new XRCheckBox() {
                Name = "DrillDownCheckBox",
                ExpressionBindings = { new ExpressionBinding("Checked", "[ReportItems].[" + detailBand.Name + "].[DrillDownExpanded]") },
                BoundsF = new RectangleF(0, 0, 25, 25),
            };
            SvgImage checkedSvg = SvgImage.FromResources("CreateHierarchicalReportInCode.Expanded.svg", typeof(ReportCreator).Assembly);
            SvgImage uncheckedSvg = SvgImage.FromResources("CreateHierarchicalReportInCode.Collapsed.svg", typeof(ReportCreator).Assembly);
            expandButton.GlyphOptions.Alignment = HorzAlignment.Center;
            expandButton.GlyphOptions.Size = new SizeF(16, 16);
            expandButton.GlyphOptions.CustomGlyphs[CheckState.Checked] = new ImageSource(checkedSvg);
            expandButton.GlyphOptions.CustomGlyphs[CheckState.Unchecked] = new ImageSource(uncheckedSvg);
            detailBand.DrillDownControl = expandButton;
            detailBand.DrillDownExpanded = false;

            XRLabel regionLabel = new XRLabel() {
                Name = "RegionLabel",
                ExpressionBindings = { new ExpressionBinding("Text", "[Region]") },
                BoundsF = new RectangleF(25, 0, 450, 25),
                TextAlignment = TextAlignment.MiddleLeft,
                // Anchor the label to both the left and right edges of the DetailBand so that it fit the page's width
                AnchorHorizontal = HorizontalAnchorStyles.Both
            };
            XRLabel marketShareLabel = new XRLabel() {
                Name = "MarketShareLabel",
                ExpressionBindings = { new ExpressionBinding("Text", "[MarketShare]") },
                TextFormatString = "{0:0%}",
                BoundsF = new RectangleF(475, 0, 175, 25),
                TextAlignment = TextAlignment.MiddleRight,
                // Anchor the label to the right edge of the DetailBand
                AnchorHorizontal = HorizontalAnchorStyles.Right
            };
            detailBand.Controls.AddRange(new XRControl[] { expandButton, regionLabel, marketShareLabel });
            return detailBand;
        }
    }
}
