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
        }

        private void CpFConclusion_OnReceived()
        {
            Console.WriteLine("CpFConclusion_OnReceived");
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            //계좌번호 얻기                        
            Console.WriteLine(m_TdUtil.AccountNumber.ToString());
            m_arAccount = (Array)m_TdUtil.AccountNumber;
            Console.WriteLine(m_arAccount.GetValue(0));
        }
    }
}
