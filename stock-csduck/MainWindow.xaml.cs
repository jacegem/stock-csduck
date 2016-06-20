

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

        public MainWindow()
        {
            InitializeComponent();


            //주문 초기화
            int nRet;
            do
            {
                nRet = m_TdUtil.TradeInit(0);
            } while (nRet != 0);

            //주문이벤트등록
            m_CpFConclusion.Received += new DSCBO1Lib._IDibEvents_ReceivedEventHandler(CpFConclusion_OnReceived);
            m_CpFConclusion.Subscribe();

            //계좌번호 얻기                        
            m_arAccount = (Array)m_TdUtil.AccountNumber;
            mAccountNumber.Content = m_arAccount.GetValue(0);

            // 종목 코드 얻기
            btnStockCode_Click(null, null);

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
            addMsgCnt(cnt);

            
        }


        public void addMsgCnt(Object cnt) {
            addMsg("결과 갯수:" + cnt);
        }


        // https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute(v=vs.110).aspx
        public void addMsg(String key="", Object val=null, [CallerMemberName]string caller = "") {
            // 잔고
            String now = System.DateTime.Now.ToString("MM-dd hh:mm:ss");
            String timeText = "[" + now + "]";
            String callerText = "[" + caller + "]";
            String keyText = "[" + key + "]";
            String text = timeText + callerText + keyText + ":" + val + "\n";
            mTextarea.AppendText(text);            
            mTextarea.ScrollToEnd();
        }

        /*
         * CpTdNew5331A
         * 설명: 계좌별 매수주문 가능 금액/수량 데이터를 요청하고 수신한다
         * 통신종류: Request/Reply
         * 모듈위치: CpTrade.dll
         */
        private void CpTdNew5331A_Click(object sender, RoutedEventArgs e)
        {
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
            int cnt = m_CpStockCode.GetCount();
            addMsg("StockCodeCount", cnt.ToString());

            for (short i = 0; i < cnt; i++) {
                Object code = m_CpStockCode.GetData(0, i);
                Object name = m_CpStockCode.GetData(1, i);
                Object fullCode = m_CpStockCode.GetData(2, i);
                //addMsg("code", code);
                //addMsg("name", name);
                //addMsg("fullCode", fullCode);
                StockManager.addStock(code, name, fullCode);
            }

            cbStockCode.ItemsSource = StockManager.getStockList();
            cbStockCode.SelectedIndex = 0;
        }

        private void btnStockMst_Click(object sender, RoutedEventArgs e)
        {
            object code = cbStockCode.SelectedValue;
            m_StockMst.SetInputValue(0, code);
            m_StockMst.BlockRequest();
            object price = m_StockMst.GetHeaderValue(11);

            addMsg("price", price);

            m_StockChart.SetInputValue(0, code);    // 종목코드
            m_StockChart.SetInputValue(1, '2'); // 개수로 요청
            m_StockChart.SetInputValue(4, 120);  // 요청개수
            
            // 0: 날짜
            // 1: 시가
            // 5: 종가
            // 8: 거래량
            object[] types= { 0, 1, 5, 8 };
            m_StockChart.SetInputValue(5, types);   // 요청 데이터 타입

            m_StockChart.SetInputValue(6, 'D');   // 챠트 구분
            m_StockChart.BlockRequest();

            object cnt = m_StockChart.GetHeaderValue(3);
            addMsg("cnt", cnt);

            for (int i = 0; i < Convert.ToInt16(cnt); i++) {
                object date = m_StockChart.GetDataValue(0, i);
                object priceStart = m_StockChart.GetDataValue(1, i);
                object priceEnd = m_StockChart.GetDataValue(2, i);
                object volumn = m_StockChart.GetDataValue(3, i);
                addMsg("date", date);
                addMsg("priceStart", priceStart);
                addMsg("priceEnd", priceEnd);
                addMsg("volumn", volumn);
                StockPrice stockPrice = new StockPrice(date, priceStart, priceEnd, volumn);

                StockManager.addStockPrice(code, stockPrice);
            }

            calculateAvg(code);
            evaluation(code);


            IEnumerable<Stock> stockList = StockManager.getStockList().Cast<Stock>();
            foreach (Stock st in stockList) {
                SortedList sl = st.getStockPriceList();
                int listCnt = sl.Count;

                for (int i = 0; i < listCnt; i++) {
                    StockPrice sp = (StockPrice)sl.GetByIndex(i);
                    addMsg("date", sp.date);
                    addMsg("avg120", sp.avg120);
                    addMsg("avg60", sp.avg60);
                    addMsg("avg20", sp.avg20);
                    addMsg("avg5", sp.avg5);
                }            
            }
            
        }

        private void evaluation(object code)
        {
            StockManager.evaluation(code);
        }

        private void calculateAvg(object code)
        {
            StockManager.calculateAvg(code);
        }
    }
}
