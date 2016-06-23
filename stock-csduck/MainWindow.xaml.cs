

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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace stock_csduck
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //*********************************************************
        DSCBO1Lib.CpFConclusionClass m_CpFConclusion = new DSCBO1Lib.CpFConclusionClass();
        DSCBO1Lib.FutureCurrClass m_FutureCurr = new DSCBO1Lib.FutureCurrClass();
        DSCBO1Lib.FutureMst m_FutureMst = new DSCBO1Lib.FutureMst();
        DSCBO1Lib.StockMst m_StockMst = new DSCBO1Lib.StockMst();   // 거래가격, 거래량


        CPTRADELib.CpTdUtilClass m_TdUtil = new CPTRADELib.CpTdUtilClass();
        CPTRADELib.CpTd6831Class m_6831 = new CPTRADELib.CpTd6831Class();
        CPTRADELib.CpTdNew5331A m_CpTdNew5331A = new CPTRADELib.CpTdNew5331A();
        CPTRADELib.CpTdNew5331B m_CpTdNew5331B = new CPTRADELib.CpTdNew5331B();

        //주식, 업종, ELW의 차트데이터를 수신합니다.
        CPSYSDIBLib.StockChart m_StockChart = new CPSYSDIBLib.StockChart();

        CPUTILLib.CpFutureCode m_FCode = new CPUTILLib.CpFutureCode();
        CPUTILLib.CpStockCode m_CpStockCode = new CPUTILLib.CpStockCode();
        

        Array m_arAccount;
        //*********************************************************

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);



        private readonly string FILE_PATH = "config.ini";

        //// ini파일에 쓰기
        //private void WriteIni_Click(object sender, EventArgs e)
        //{
        //    WritePrivateProfileString("섹션1", "키1", "값1", FilePath);
        //}


        public MainWindow()
        {
            InitializeComponent();


            //주문 초기화
            // 초기화 에러 발생시 메시지 출력
            try
            {
                int nRet = m_TdUtil.TradeInit(0);
            }
            catch(Exception e) {
                addMsg("EXCEPTION", e.Message);
                return;
            }
                                    
            // 단일최대 주문금액
            StringBuilder sb = new StringBuilder(255);
            int maxBuy = GetPrivateProfileString("CONFIG", "MAX_BUY", "100000", sb, 255, FILE_PATH);
            int sellPercent = GetPrivateProfileString("CONFIG", "SELL_PERCENT", "10", sb, 255, FILE_PATH);


            //주문이벤트등록
            m_CpFConclusion.Received += new DSCBO1Lib._IDibEvents_ReceivedEventHandler(CpFConclusion_OnReceived);
            m_CpFConclusion.Subscribe();

            //계좌번호 얻기                        
            m_arAccount = (Array)m_TdUtil.AccountNumber;
            mAccountNumber.Content = m_arAccount.GetValue(0);

            // 종목 코드 얻기
            btnStockCode_Click(null, null);

            //보유 종목 얻기
            //CpTdNew5331A_Click(null, null);

            //todo 보유 종목에 대해서, 팔지 여부를 판단한다. → 팔아야 하면 바로 실행
            // 10% 이하.

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Current.Shutdown();
        }

        private void CpFConclusion_OnReceived()
        {
            Console.WriteLine("CpFConclusion_OnReceived");
        }

        /*
         * CpTdNew5331B
         * 설명: 계좌별 매도주문 가능 수량 데이터를 요청하고 수신한다
         * 통신종류: Request/Reply
         * 모듈위치: CpTrade.dll
         */
        private void CpTdNew5331B_Click(object sender, RoutedEventArgs e)
        {
            addMsg();
            m_CpTdNew5331B.SetInputValue(0, mAccountNumber.Content);

            CpTdNew5331B.IsEnabled = false;
            m_CpTdNew5331B.BlockRequest();
            CpTdNew5331B.IsEnabled = true;

            Object cnt = m_CpTdNew5331B.GetHeaderValue(0);
            addMsg("cnt", cnt);
            
        }

        // https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute(v=vs.110).aspx
        public void addMsg(String key="", Object val=null, [CallerMemberName]string caller = "") {
            // 잔고
            String now = System.DateTime.Now.ToString("MM-dd HH:mm:ss");
            String timeText = "[" + now + "]";
            String callerText = "[" + caller + "]";
            String keyText = "[" + key + "]";
            String text = timeText + callerText + keyText + ":" + val + "\n";

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                mTextarea.AppendText(text);
                mTextarea.ScrollToEnd();
            }));
        }

        /*
         * CpTdNew5331A
         * 설명: 계좌별 매수주문 가능 금액/수량 데이터를 요청하고 수신한다
         * 통신종류: Request/Reply
         * 모듈위치: CpTrade.dll
         */
        private void CpTdNew5331A_Click(object sender, RoutedEventArgs e)
        {

            SettingWindow cw = new SettingWindow();
            cw.ShowInTaskbar = false;
            cw.Owner = this;
            cw.Show();


            addMsg();
            m_CpTdNew5331A.SetInputValue(0, mAccountNumber.Content);

            CpTdNew5331A.IsEnabled = false;
            m_CpTdNew5331A.BlockRequest();
            CpTdNew5331A.IsEnabled = true;

            //for (int i = 0; i <= 54; i++) {
            //    Object rst = m_CpTdNew5331A.GetHeaderValue(i);
            //    addMsg(i + " " + rst);
            //}

            // 47 : 가용예수금
            object availableDeposits = m_CpTdNew5331A.GetHeaderValue(47);
            mAvailableDeposits.Content = availableDeposits;

        }

        private void btnStockCode_Click(object sender, RoutedEventArgs e)
        {

            short cnt = m_CpStockCode.GetCount();
            addMsg("StockCodeCount", cnt.ToString());

            //for (short i = 0; i < cnt; i++) {
            for (short i = (short)(cnt - 1); i >= 0 ; i--)
            {
                Object code = m_CpStockCode.GetData(0, i);
                Object name = m_CpStockCode.GetData(1, i);
                Object fullCode = m_CpStockCode.GetData(2, i);
                //addMsg("code", code);
                //addMsg("name", name);
                //addMsg("fullCode", fullCode);


                Regex regex = new Regex(@"KOSPI|KODEX|SMART|ARIRANG|KBSTAR|TIGER|KINDEX|KOSEF|대신B\d{3}");
                if (regex.IsMatch(name.ToString())) continue;
                
                StockManager.addStock(code, name, fullCode);
            }

            cbStockCode.ItemsSource = StockManager.getStockList();
            cbStockCode.SelectedIndex = 0;
        }

        private void btnStockMst_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;

            BackgroundWorker _backgroundWorker = new BackgroundWorker();
            // BackgroundWorker의 이벤트 처리기
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            // BackgroundWorker 실행
            // 매개변수를 넣어 실행시키는 것이 가능합니다.
            // 매개변수라 다수인경우, 배열을 사용하면 됩니다.
            _backgroundWorker.RunWorkerAsync();
        }

        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do something
            Object argument = e.Argument;
            // BackgroundWorker에서 수행할 일을 정의합니다.
            IEnumerable<Stock> stockList = StockManager.getStockList().Cast<Stock>();
            int cnt = 1;
            foreach (Stock st in stockList)
            {
                setStatus("조회중" + "[" + cnt++ + "]", st.name);
                RequestCountUtil.add();

                setPastPrice(st.code);
                StockManager.evaluateBuy(st.code);

                if (st.buyPoint > 7)
                {
                    addMsg("name", st.name);
                    addMsg("buyPoint", st.buyPoint);
                    StockPrice sp = st.getLastPrice();
                    addMsg("rate5", sp.rate5);
                }
            }
        }
        void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // ProgressChanged
            // 진행률에 변화가 있을때 이벤트가 발생합니다.
            // 현재 얼마나 진행했는지 보여주는 코드를
            // 이곳에 작성합니다.
        }
        // Completed Method
        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                setStatus("Cancelled", "_backgroundWorker");                
            }
            else if (e.Error != null)
            {
                setStatus("Exception Thrown", "_backgroundWorker");
            }
            else
            {
                // Do Something
                // 일이 모두 마쳤을때 수행되어야할
                // 코드를 이곳에 정의합니다.                
                setStatus("Completed", "_backgroundWorker");
            }

            enableButton();
        }

        private void enableButton()
        {
            btnStockMst.IsEnabled = true;
        }

        private void setStatus(string key, object val)
        {
            String now = System.DateTime.Now.ToString("MM-dd HH:mm:ss");
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {   
                txStatus.Text = "[" + now + "]" + "[" + key + "]" + " : " + val;
            }));
            
        }

        private void setPastPrice(object code)
        {
            m_StockChart.SetInputValue(0, code);    // 종목코드
            m_StockChart.SetInputValue(1, '2'); // 개수로 요청
            m_StockChart.SetInputValue(4, 120);  // 요청개수

            // 0: 날짜
            // 1: 시가
            // 5: 종가
            // 8: 거래량
            object[] types = { 0, 1, 5, 8 };
            m_StockChart.SetInputValue(5, types);   // 요청 데이터 타입

            m_StockChart.SetInputValue(6, 'D');   // 챠트 구분
            m_StockChart.BlockRequest();

            object cnt = m_StockChart.GetHeaderValue(3);
            //addMsg("cnt", cnt);

            for (int i = 0; i < Convert.ToInt16(cnt); i++)
            {
                object date = m_StockChart.GetDataValue(0, i);
                object priceStart = m_StockChart.GetDataValue(1, i);
                object priceEnd = m_StockChart.GetDataValue(2, i);
                object volumn = m_StockChart.GetDataValue(3, i);
                //addMsg("date", date);
                //addMsg("priceStart", priceStart);
                //addMsg("priceEnd", priceEnd);
                //addMsg("volumn", volumn);
                StockPrice stockPrice = new StockPrice(date, priceStart, priceEnd, volumn);

                StockManager.addStockPrice(code, stockPrice);
            }

            // 모두 입력후에 평균을 계산한다.
            StockManager.calculateAvg(code);
        }
    }
}
