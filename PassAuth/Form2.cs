using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PassAuth
{
    public partial class Form2 : Form
    {
        public static key kl;
        public Form2(key K)
        {
            InitializeComponent();
            kl = K;
        }

        private int i = 3;

        private void button1_Click(object sender, EventArgs e)
        {
            
            string oldpas = textBox1.Text;
            string newpas = textBox2.Text;
            string drive = "\\\\.\\" + kl.Dir;
            string codpass = Encrypt(oldpas, kl.Val); ;//кодирование пароля 
            byte[] ByteBuffer = new byte[512];//задаем размер буфера 
            byte[] temp = new byte[8];
            bool flag = true;
            FileReader fr = new FileReader();
            if (fr.OpenRead(drive)) //вызов для чтения 
            {
                int count = fr.Read(ByteBuffer, 512);

                for (int i = 54; i < 62; i++)
                {
                    temp[i - 54] = ByteBuffer[i];
                }
                fr.Close();

                byte[] oldpass = new byte[8];
                for (int i = 0; i < codpass.Length; i++)
                {
                    oldpass[i] = (byte)codpass[i];
                }

                for (int i = 0; i < 8; i++)
                {
                    if (temp[i] != oldpass[i])
                        flag = false;
                }
                if (flag != false)
                {
                    if (fr.OpenWrite(drive))
                    {
                        string codnewpass;
                        codnewpass = null;
                        Random rnd = new Random();
                        int l = 1 + rnd.Next(100);
                        kl.Val = GenerateKeyWord( l);
                        MessageBox.Show("Your key: " + kl.Val);
                        codnewpass = Encrypt(newpas, kl.Val);
                        byte[] newpass = new byte[8];
                        for (int i = 0; i < codnewpass.Length; i++)
                        {
                            newpass[i] = (byte)codnewpass[i];
                        }
                        for (int i = 54; i < 62; i++)
                        {
                            ByteBuffer[i] = newpass[i - 54];
                        }
                        fr.Write(ByteBuffer, 0, 512);
                        this.Hide();
                        MessageBox.Show("Пароль сменен");

                        Application.Exit();
                    }
                }
                else
                {
                    MessageBox.Show("Попыток:" + i);
                    i--;
                    if (i < 1)
                        Application.Exit();
                    // codpass = null; 
                    // temp = null; 
                    // oldpass = null; 
                    textBox1.Clear();
                    textBox2.Clear();

                }
            }
        }

        public static char[] characters = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','X','X','0','1','2','3','4','5','6','7','8','9','_'};

        public int N = characters.Length;

        public string Encrypt(string input, int[] key)
        {
            for (int i = 0; i < input.Length % 4; i++)
                input += input[i];

            string result = "";

            for (int i = 0; i < input.Length; i += 4)
            {
                char[] transposition = new char[4];

                for (int j = 0; j < 4; j++)
                    transposition[key[j] - 1] = input[i + j];

                for (int j = 0; j < 4; j++)
                    result += transposition[j];
            }

            return result;
        }

        public int[] GenerateKeyWord(int startSeed)
        {
            int[] key = new int[4];
            Random rand = new Random(startSeed);
            for (int i = 0; i < 4; i++)
                key[i] = rand.Next(1, 4);
            return key;
        }


        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
           // Form ifrm = Application.OpenForms[0];
         //   ifrm.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
            private void Form2_FormClosing(object sender, FormClosingEventArgs e)
            {
                Application.Exit();
            }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
