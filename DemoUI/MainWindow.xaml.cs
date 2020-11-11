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
using JasperUnloadUI.Model;
using BingLibrary.hjb.PLC;
using ViewROI;
using BingLibrary.HVision;
using HalconDotNet;
using System.Windows.Forms;

namespace DemoUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量
        Scan myScan = new Scan();
        ThingetPLC myPLC = new ThingetPLC();
        bool isPLCConnect = false;
        bool[] M2000;
        double _D3000 = 0;

        HWndCtrl viewController1;//halcon窗口控制器
        ROIController roiController1;
        CameraOperate Camera = new CameraOperate();//相机操作
        #endregion
        #region 构造函数
        public MainWindow()
        {
            InitializeComponent();
            PLCButton.AddHandler(System.Windows.Controls.Button.MouseDownEvent, new RoutedEventHandler(Button_MouseDown), true);
            PLCButton.AddHandler(System.Windows.Controls.Button.MouseUpEvent, new RoutedEventHandler(Button_MouseUp), true);

            viewController1 = new HWndCtrl(ImageWindow1);//HWindowControlWPF窗口给到窗口控制器
            roiController1 = new ROIController();
            viewController1.useROIController(roiController1);
            viewController1.setViewState(HWndCtrl.MODE_VIEW_MOVE);
        }
        #endregion
        #region 功能
        void AddMessage(string str)
        {
            string[] s = MsgTextBox.Text.Split('\n');
            if (s.Length > 1000)
            {
                MsgTextBox.Text = "";
            }
            if (MsgTextBox.Text != "")
            {
                MsgTextBox.Text += "\r\n";
            }
            MsgTextBox.Text += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        async void Run()
        {
            
            bool _m2000 = false;
            await Task.Run(() => {
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    isPLCConnect = myPLC.ReadSM(0);
                    if (isPLCConnect)
                    {
                        M2000 = myPLC.ReadMultiMCoil(2000);
                        _D3000 = myPLC.ReadW(3000);
                        if (_m2000 != M2000[0])
                        {
                            _m2000 = M2000[0];
                            if (_m2000)
                            {
                                this.Dispatcher.Invoke(new Action(() => {
                                    AddMessage("触发扫码");
                                }));                                
                            }
                        }
                    }
                    else
                    {
                        myPLC.ModbusDisConnect();
                        myPLC.ModbusInit("COM13", 19200, System.IO.Ports.Parity.Even, 8, System.IO.Ports.StopBits.One);
                        myPLC.ModbusConnect();
                    }
                }
            });

        }
        async void UpdateUI()
        {
            while (true)
            {
                await Task.Delay(100);
                PLCConnect.Fill = isPLCConnect ? Brushes.Green : Brushes.Red;
                CounterText.Text = _D3000.ToString(); ;
            }
        }
        #endregion
        #region 事件响应函数
        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            try
            {
                myPLC.SetM(3000, true);
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
            }
        }

        private void Button_MouseUp(object sender, RoutedEventArgs e)
        {
            try
            {
                myPLC.SetM(3000, false);
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
            }
        }
        private void MsgTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MsgTextBox.ScrollToEnd();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myScan.ini("COM7");
            AddMessage("软件加载完成");
            string openimgrst = Camera.OpenCamera("[0] Integrated Camera", "DirectShow") ? "相机打开成功" : "相机打开失败";
            AddMessage(openimgrst);
            Run();
            UpdateUI();
        }

        private void FuncButtonClick(object sender, RoutedEventArgs e)
        {

            AddMessage("按钮被按下");
        }
        void MyGetBarcodeCallback(string barcode)
        {
            AddMessage(barcode);
        }
        private void MainPageSelect(object sender, RoutedEventArgs e)
        {
            MainPageGrid.Visibility = Visibility.Visible;
            ParameterGrid.Visibility = Visibility.Collapsed;
        }

        private void ParameterPageSelect(object sender, RoutedEventArgs e)
        {
            MainPageGrid.Visibility = Visibility.Collapsed;
            ParameterGrid.Visibility = Visibility.Visible;
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e)
        {
            myScan.GetBarCode(MyGetBarcodeCallback);
        }
        #endregion

        private void GrabImageButtonClick(object sender, RoutedEventArgs e)
        {
            Camera.GrabImage();
            viewController1.addIconicVar(Camera.CurrentImage);
            viewController1.repaint();
        }

        private void DrawROIButtonClick(object sender, RoutedEventArgs e)
        {
            HObject myregion;
            HOperatorSet.SetDraw(viewController1.viewPort.HalconWindow, "margin");
            HOperatorSet.SetColor(viewController1.viewPort.HalconWindow,"red");
            HTuple row1, column1, row2, column2;
            HOperatorSet.DrawRectangle1(viewController1.viewPort.HalconWindow,out row1,out column1,out row2,out column2);
            HOperatorSet.GenRectangle1(out myregion, row1, column1, row2, column2);
            viewController1.addIconicVar(myregion);//添加XLD到控件
            viewController1.viewPort.HalconWindow.SetColor("green");//设置窗口显示控件的颜色
            viewController1.repaint();//更新
            HOperatorSet.WriteRegion(myregion, System.Environment.CurrentDirectory + "\\ROI.hobj");
        }

        private void ReadROIButtonClick(object sender, RoutedEventArgs e)
        {
            HObject myregion;
            HOperatorSet.ReadRegion(out myregion, System.Environment.CurrentDirectory + "\\ROI.hobj");
            viewController1.addIconicVar(myregion);//添加XLD到控件
            viewController1.viewPort.HalconWindow.SetColor("green");//设置窗口显示控件的颜色
            viewController1.repaint();//更新
        }

        private void ReadImageButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//打开读取文件对话框
            ofd.Filter = "Image文件(*.bmp;*.jpg)|*.bmp;*.jpg|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = ofd.FileName;//获得选择的文件路径
                Camera.ReadImage(strFileName);//读取图片
                viewController1.addIconicVar(Camera.CurrentImage);//显示图片
                viewController1.repaint();//更新
            }
        }

        private void SaveImageButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();//打开保存文件对话框
            saveDlg.Filter = "图片(*.bmp)|*.bmp";
            if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Camera.SaveImage("bmp", saveDlg.FileName);//保存图片
            }
        }
    }
}
