﻿using System;
using System.Windows.Forms;
using WindowsInput;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using MeowPass.Tools;
using static MeowPass.MeowTool;

namespace MeowPass
{
    public partial class MeowDemo : Form
    {
        public MeowDemo()
        {
            InitializeComponent();

            Process[] processes = Process.GetProcessesByName(Application.CompanyName);
            if (processes.Length > 1)
            {
                MessageBox.Show("已有一个实例正在运行,请不要重复启动", "MeowPass", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;
            MaximizeBox = false;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            encryptBox.SelectedIndex = 0;
            HotKey.RegisterHotKey(Handle, 233, 3, Keys.Enter);
            HotKey.RegisterHotKey(Handle, 234, 3, Keys.M);

            if (checkBoxHidePass.Checked)
            {
                passBox.PasswordChar = '*';
            }
        }

        private void CheckBoxHidePass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHidePass.Checked)
            {
                passBox.PasswordChar = '*';
            }
            else
            {
                passBox.PasswordChar = new char();
            }
        }

        private void ButtonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(passBox.Text);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:
                    if (m.WParam.ToString() == "233")
                    {
                        if (!string.IsNullOrEmpty(passBox.Text))
                        {
                            new InputSimulator().Keyboard.TextEntry(passBox.Text);
                        }
                    }
                    if (m.WParam.ToString() == "234")
                    {
                        menuItemShow.PerformClick();
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void MeowDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            HotKey.UnregisterHotKey(Handle, 233);
            HotKey.UnregisterHotKey(Handle, 234);
            notifyIcon.Visible = false;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                NormalForm();
            }
        }

        private void MeowDemo_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                MinimizedForm();
            }
        }

        private void MenuItemExit_Click(object sender, EventArgs e) => Close();

        private void MenuItemShow_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                NormalForm();
                menuItemShow.Text = "最小化";
            }
            else
            {
                MinimizedForm();
                menuItemShow.Text = "显示";
            }
        }

        public void MinimizedForm()
        {
            WindowState = FormWindowState.Minimized;
            Thread.Sleep(200);
            Visible = false;
            ShowInTaskbar = false;
        }

        public void NormalForm()
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            Activate();
            ShowInTaskbar = false;
        }

        private string GenMeowPass(string pass,string tag,int passLength)
        {
            string uPassCrypto = "";
            string uTagCrypto = MyMd5Hash(tag);
            if (shaRButton.Checked)
            {
                uPassCrypto = MyMd5Hash(MySHAHash(uPassBox.Text.ToString()) + uPassBox.Text.ToString());
            }
            else if (md5RButton.Checked)
            {
                uPassCrypto = MyMd5Hash(MyMd5Hash(uPassBox.Text.ToString()) + uPassBox.Text.ToString());
            }
            else if (crcRButton.Checked)
            {
                uPassCrypto = MyMd5Hash(MyCRSHash(uPassBox.Text.ToString()) + uPassBox.Text.ToString());
            }
            switch (encryptBox.SelectedIndex)
            {
                case 0:
                    pass = MyDESCrypto(uTagCrypto, uPassCrypto);
                    break;
                case 1:
                    pass = MyTripleDESCrypto(uTagCrypto, uPassCrypto);
                    break;
                case 2:
                    pass = MyAESCrypto(uTagCrypto, uPassCrypto);
                    break;
                case 3:
                    pass = MyRC2Crypto(uTagCrypto, uPassCrypto);
                    break;
                case 4:
                    pass = MyBlowFishCrypto(uTagCrypto, uPassCrypto);
                    break;
                default:
                    break;
            }
            if (pass.Length < passLength + 1)
            {
                while (!(pass.Length < passLength + 1))
                {
                    pass += pass;
                }
            }
            return pass.Substring(0, passLength);
        }

        private void GenMeowPass()
        {
            passBox.Text = GenMeowPass(uPassBox.Text, tagBox.Text, Convert.ToInt32(passUpDown.Value));
        }

        private void EncryptBox_SelectedIndexChanged(object sender, EventArgs e) => GenMeowPass();

        private void TableLayoutPanel2_Click(object sender, EventArgs e) => GenMeowPass();

        private void PassUpDown_ValueChanged(object sender, EventArgs e) => GenMeowPass();

        private void TagBox_TextChanged(object sender, EventArgs e) => GenMeowPass();

        private void UPassBox_TextChanged(object sender, EventArgs e) => GenMeowPass();

        private void Md5RButton_CheckedChanged(object sender, EventArgs e) => GenMeowPass();

        private void ShaRButton_CheckedChanged(object sender, EventArgs e) => GenMeowPass();

        private void CrcRButton_CheckedChanged(object sender, EventArgs e) => GenMeowPass();

        private void CheckBoxHideUPass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHideUPass.Checked)
            {
                uPassBox.PasswordChar = '*';
            }
            else
            {
                uPassBox.PasswordChar = new char();
            }
        }

        private void MeowDemo_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        private void MeowDemo_Deactivate(object sender, EventArgs e)
        {
            try
            {
                Opacity = 0.5;
            }
            catch
            {}
        }

        private void MenuItemAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
            MinimizedForm();
        }

        private void Label5_DoubleClick(object sender, EventArgs e)
        {
            new MeowBeta().Show();
        }
    }
}
