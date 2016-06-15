#define RUN

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

        CPTRADELib.CpTdUtilClass m_TdUtil = new CPTRADELib.CpTdUtilClass();
        CPTRADELib.CpTd6831Class m_6831 = new CPTRADELib.CpTd6831Class();
        CPUTILLib.CpFutureCode m_FCode = new CPUTILLib.CpFutureCode();

        CPTRADELib.CpTdNew5331A m_CpTdNew5331A = new CPTRADELib.CpTdNew5331A();
        CPTRADELib.CpTdNew5331B m_CpTdNew5331B = new CPTRADELib.CpTdNew5331B();

        Array m_arAccount;
        //*********************************************************

        public MainWindow()
        {
            InitializeComponent();

#if RUN
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
#endif
        }

        private void CpFConclusion_OnReceived()
        {
            Console.WriteLine("CpFConclusion_OnReceived");
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            
        }

        int idx = 0;

        /// <summary>
        /// 모든 계좌 정보를 얻는다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            
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
        public void addMsg(String msg="", [CallerMemberName]string memberName = "") {
            // 잔고
            String now = System.DateTime.Now.ToString("MM-dd hh:mm:ss");
            String text = "[" + now + "] ";
            if (msg.Length == 0)
            {
                text += "함수호출: " + memberName;
            }
            else {
                text += msg;
            }

            text += "\n";
            
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
    }
}
