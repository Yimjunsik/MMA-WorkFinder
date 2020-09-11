using MMA_WorkFinder.Models;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MMA_WorkFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadImageReady();

            //dynamic activeX = this.ViewPage.GetType().InvokeMember("ActiveXInstance",
            //        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            //        null, this.ViewPage, new object[] { });
            //activeX.Silent = true;
        }

        private readonly List<compData> compList = new List<compData>();

        private void SearchQueryInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (!SearchQueryInput.Text.Equals(string.Empty) && e.Key.Equals(Key.Enter))
            {
                compList.Clear();
                ResultListView.Items.Clear();

                string result = Network.Request("https://work.mma.go.kr/caisBYIS/search/byjjecgeomsaek.do?eopjong_gbcd=" + (PositionSelector.SelectedIndex + 1) + "&eopche_nm=" + SearchQueryInput.Text);
                Document doc = NSoup.Parse.Parser.Parse(result, "https://work.mma.go.kr");
                Elements elms = doc.Select("th.title.t-alignLt.pl20px");

                if (elms.Count.Equals(0))
                {
                    ResultListView.Items.Add("조회된 기업 없음");
                    LoadImageReady();
                }

                foreach (var item in elms)
                {
                    compData compData = new compData();
                    compData.SetName(item.Text());
                    compData.SetId(GetMiddleString(item.Select("a").Attr("href"), "byjjeopche_cd=", "&"));
                    compList.Add(compData);
                    ResultListView.Items.Add(compData.GetName() + Environment.NewLine + "기업코드 : " + compData.GetId());
                }
            }
        }

        private void PositionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SearchQueryInput?.Text == null)
                    return;

                if (!SearchQueryInput.Text.Equals(string.Empty))
                {
                    compList.Clear();
                    ResultListView.Items.Clear();
                    string result = Network.Request("https://work.mma.go.kr/caisBYIS/search/byjjecgeomsaek.do?eopjong_gbcd=" + (PositionSelector.SelectedIndex + 1) + "&eopche_nm=" + SearchQueryInput.Text);
                    Document doc = NSoup.Parse.Parser.Parse(result, "https://work.mma.go.kr");
                    Elements elms = doc.Select("th.title.t-alignLt.pl20px");
                    foreach (var item in elms)
                    {
                        compData compData = new compData();
                        compData.SetName(item.Text());
                        compData.SetId(GetMiddleString(item.Select("a").Attr("href"), "byjjeopche_cd=", "&"));
                        compList.Add(compData);
                        ResultListView.Items.Add(compData.GetName() + Environment.NewLine + "기업코드: " + compData.GetId());
                    }
                }
            }
            catch { }
        }

        public static string GetMiddleString(string str, string begin, string end)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            string result = null;
            if (str.IndexOf(begin) > -1)
            {
                str = str.Substring(str.IndexOf(begin) + begin.Length);
                if (str.IndexOf(end) > -1) 
                    result = str.Substring(0, str.IndexOf(end));
                else 
                    result = str;
            }
            return result;
        }

        private void LoadImageReady()
        {
            ViewPage.NavigateToString(Formatter.GetHtml("<div class=\"layer\"><div class=\"layer_inner\"><img class=\"content\" draggable=\"false\" src=\"https://www.mma.go.kr/download/content/usr0000248/img3.gif\"><div></div>"));
        }

        private void ResultListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string result = Network.Request("https://work.mma.go.kr/caisBYIS/search/byjjecgeomsaekView.do?byjjeopche_cd=" + compList[ResultListView.SelectedIndex].GetId());
            Document doc = NSoup.Parse.Parser.Parse(result, "https://work.mma.go.kr");
            doc.Select("table").RemoveClass("table_row").AddClass("table table-bordered table-hover");
            doc.Select("caption").Remove();
            doc.Select("div.blist_btn_n").Remove();
            string html = doc.Select("div[id=content]").OuterHtml();
            ViewPage.NavigateToString(Formatter.GetHtml(html + "<center>(우)35208 대전광역시 서구 청사로 189, 정부대전청사 2동 대표전화 : 1588-9090<br>COPYRIGHT(C) Military Manpower Administration ALL RIGHTS RESERVED.</center>"));

        }

    }
}
