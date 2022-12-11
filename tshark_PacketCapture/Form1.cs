using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tshark_PacketCapture
{
    public partial class Form1 : Form
    {
        // プロセス tshark インターフェース一覧を表示
        Process processTsInterface = null;

        // プロセス tshark パケットキャプチャを表示
        Process processTsCap = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // コマンド
            // tsharkのパスを指定
            string executeCommand = "C:\\Program Files\\Wireshark\\tshark.exe";

            // コマンド引数
            string args = "-D";

            // プロセスを起動
            processTsInterface = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo(executeCommand, args);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            // 文字化け防止に、文字コードを指定する
            processStartInfo.StandardErrorEncoding = Encoding.UTF8;
            processStartInfo.StandardOutputEncoding = Encoding.UTF8;

            // 起動
            processTsInterface = Process.Start(processStartInfo);

            // 受信イベント
            processTsInterface.OutputDataReceived += dataReceivedTsInterface;
            processTsInterface.ErrorDataReceived += errReceivedTsInterface;

            processTsInterface.BeginErrorReadLine();
            processTsInterface.BeginOutputReadLine();
        }

        // tshark インターフェース一覧を表示 出力
        void dataReceivedTsInterface(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread(packetText, textBox1);
            }
        }

        // tshark インターフェース一覧を表示 エラー
        void errReceivedTsInterface(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread("ERR:" + packetText, textBox1);
            }
        }

        // テキストボックスに出力
        private void PrintTextBox(string msg, TextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                tb.Text = msg;
            }
            else
            {
                tb.Text = tb.Text + "\r\n" + msg;
            }

            // キャレット位置を末尾に移動
            tb.SelectionStart = tb.Text.Length;
            tb.Focus();
            tb.ScrollToCaret();
        }

        // 別スレッドからのテキストボックス更新
        private void PrintTextBoxByThread(string msg, TextBox tb)
        {
            this.Invoke((MethodInvoker)(() => PrintTextBox(msg, tb)));
        }


        // パケットキャプチャ ボタンクリック
        private void button2_Click(object sender, EventArgs e)
        {
            // 必須チェック
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("インターフェース番号を指定してください");
                return;
            }

            // コマンド
            string executeCommand = "C:\\Program Files\\Wireshark\\tshark.exe";

            // コマンド引数
            string args = String.Format("-i {0} -l", textBox2.Text);

            // プロセスを起動
            processTsCap = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo(executeCommand, args);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            // 文字化け防止に、文字コードを指定する
            processStartInfo.StandardErrorEncoding = Encoding.UTF8;
            processStartInfo.StandardOutputEncoding = Encoding.UTF8;

            // 起動
            processTsCap = Process.Start(processStartInfo);

            // 受信イベント
            processTsCap.OutputDataReceived += dataReceivedTsCap;
            processTsCap.ErrorDataReceived += errReceivedTsCap;

            processTsCap.BeginErrorReadLine();
            processTsCap.BeginOutputReadLine();
        }

        // パケットキャプチャを表示　出力
        void dataReceivedTsCap(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if(packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread(packetText, textBox3);
            }
        }

        // パケットキャプチャを表示　エラー
        void errReceivedTsCap(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if(packetText != null && packetText.Length > 0 )
            {
                PrintTextBoxByThread("ERR:" + packetText, textBox3);
            }
        }

        // フォームクローズ
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(processTsInterface != null && !processTsInterface.HasExited)
            {
                // プロセス終了
                processTsInterface.Kill();
            }

            if(processTsCap != null && !processTsCap.HasExited)
            {
                // プロセス終了
                processTsCap.Kill();
            }
        }

    }
}
