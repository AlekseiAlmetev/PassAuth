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
    public partial class Form1 : Form
    {


        public Form1()

        {
            InitializeComponent();
            search_external_drives(comboBox1);
        }

        private int i = 3;

        private void search_external_drives(ComboBox input) //поиск носителей 
        {
            string mydrive;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && (d.DriveType == DriveType.Removable))
                {
                    mydrive = d.Name;
                    input.Items.Add(mydrive);
                }
            }
            if (input.Items.Count == 0)
            {
                input.Items.Add("Внешние носители отсутствуют");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password_1 = "";
            password_1 = textBox1.Text;
            string drive = "\\\\.\\" + comboBox1.Text.Remove(2);
            string pas = textBox1.Text;
            key Key = new key();
            Key.Dir = comboBox1.Text.Remove(2); ;
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

                if (temp[0] == 0)//пароль еще не записан на флешке 
                {
                    fr.OpenWrite(drive);
                    {
                        Random rnd = new Random();
                        int l = 1 + rnd.Next(100);
                        Key.Val = GenerateKeyWord(l);
                        MessageBox.Show("Your key: " + Key.Val);
                        string codpass = Encrypt(pas, Key.Val);
                        byte[] oldpass = new byte[8];
                        for (int i = 0; i < codpass.Length; i++)
                        {
                            oldpass[i] = (byte)codpass[i];
                        }
                        for (int i = 54; i < 62; i++)
                        {
                            ByteBuffer[i] = oldpass[i - 54];
                        }
                        count = fr.Write(ByteBuffer, 0, 512);
                        this.Hide();
                        Form2 f = new Form2(Key); //Вывод на экран окна успешного ввода 
                        f.Show();
                    }
                    fr.Close();
                }
                else
                {
                    int[] kl = new int[textBox2.Text.Length];
                    for (int i = 0; i < textBox2.Text.Length; ++i)
                    {
                        kl[i] = char.Parse("" + textBox2.Text[i]);
                    }
                    Key.Val = kl;
                    string codpass = Encrypt(pas, Key.Val);
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
                        // codpass = null; 
                        //System.Diagnostics.Process.Start(@"h:\\"); 
                        this.Hide();
                        Form2 f = new Form2(Key); //Вывод на экран окна успешного ввода 
                        f.Show();
                    }
                    else
                    {
                        MessageBox.Show("Попыток:" + i);
                        i--;
                        if (i < 1)
                            Application.Exit();
                        textBox1.Clear();
                    }
                }
            }
            /*
            string pas = textBox1.Text;
            string pass;
            FileInfo fileInf = new FileInfo("pass.txt");
            if (fileInf.Exists)
            {
                FileStream file = new FileStream("pass.txt", FileMode.Open);
                StreamReader reader = new StreamReader(file);
                pass = reader.ReadLine();
                reader.Close();
                key Key = new key();
                //int[] kl = textBox2.Text.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x)).ToArray();
                int[] kl = new int[textBox2.Text.Length];
                char[] chArray = textBox2.Text.ToCharArray();
                for (int i = 0; i < textBox2.Text.Length; ++i)
                {
                    kl[i] = char.Parse("" + textBox2.Text[i]);
                }
                Key.Val = kl;
                pas = Encrypt(pas,kl);
                if (pas == pass)
                {
                    Form ifrm = new Form2(Key);
                    ifrm.Show();
                    this.Hide();
                }
                else
                {
                    i--;
                    MessageBox.Show("Wrong password or key! You have " + i + " chances!");
                }
            }
            else
            {
                FileStream file = new FileStream("pass.txt", FileMode.Create);
                StreamWriter writer = new StreamWriter(file);
                Random rnd = new Random();
               
                int i = 1 + rnd.Next(100);
                int[] kl= GenerateKeyWord(i);
                string str = "";
                for (i = 0; i < kl.Length; i++)
                {
                    str += kl[i].ToString() + " ";
                }
                key Key = new key();
                Key.Val = kl;
                pas = Encrypt(pas, kl);
                writer.WriteLine(pas);
                MessageBox.Show("Your key: " + str);
                writer.Close();
                Form ifrm = new Form2(Key);
                ifrm.Show();
                this.Hide();
            }
            if (i == 0)
            {
                this.Close();
            }*/
        }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class key
    {
        private int[] val;
        public int[] Val
        {
            get { return val; }
            set { val = value; }
        }
        private string dir;
        public string Dir
        {
            get { return dir; }
            set { dir = value; }
        }

    }

    class FileReader //класс для работы с носителями информации (жесткий диск - файл) работа с жестким диском - путем открытия файла. 
    {
        const uint GENERIC_READ = 0x80000000; //для чтения 
        const uint GENERIC_WRITE = 0x40000000; //для записи 
        const uint OPEN_EXISTING = 3; //тип открытия файла 
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        System.IntPtr handle; //дескриптор окна 
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        //Kernel32.dll - динамически подключаемая библиотека, предоставляющая приложениям многие базовые функции 
        //API Win32, в частности: управление памятью, операции ввода/вывода, создание процессов и потоков и функции синхронизации. 
        //System.Runtime.InteropServices - пространство имен 
        //DllImport - вызов неуправляемого кода из управляемого приложения 
        //"kernel32" - библиотека 
        //SetLastError - значение true, чтобы показать, что вызывающий объект вызовет SetLastError 
        //ThrowOnUnmappableChar - исключение каждый раз Interop маршалер встречает unmappable характер 
        //CharSet = System.Runtime.InteropServices.CharSet.Ansi - Указывает, какой набор знаков должны использовать маршалированные строки 
        unsafe static extern System.IntPtr CreateFile
        (
        string FileName, // имя файла 
        uint DesiredAccess, // режим доступа 
        uint ShareMode, // режим 
        uint SecurityAttributes, // атрибуты безопасности 
        uint CreationDisposition, // как создать 
        uint FlagsAndAttributes, // атрибуты файла 
        int hTemplateFile // дескриптор файла шаблона 
        );
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        unsafe static extern bool ReadFile
        (
        System.IntPtr hFile, // дескриптор файла 
        void* pBuffer, // буфер данных 
        int NumberOfBytesToRead, // количество байт для чтения 
        int* pNumberOfBytesRead, // количество прочитанных байт 
        int Overlapped // перекрывающий буфер 
        );
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        unsafe static extern bool WriteFile
        (
        System.IntPtr hFile, // дескриптор файла 
        void* lpBuffer, // буфер данных 
        int NumberOfBytesToRead, // количество байт для чтения 
        int* pNumberOfBytesRead, // количество прочитанных байт 
        int Overlapped // перекрывающийся буфер 
        );
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        unsafe static extern bool CloseHandle
        (
        System.IntPtr hObject // обращение к объекту 
        );
        public bool OpenRead(string FileName) //работа с нулевым сектором 
        {
            // открытие существующего файла для чтения 
            handle = CreateFile(FileName, GENERIC_READ, FILE_SHARE_READ, 0, OPEN_EXISTING, 0, 0); //в дискрипторе окна вызывается ф-ия создания файла с параметрами чтения и открытия 
                                                                                                  //параметры открытия 
            if (handle != System.IntPtr.Zero) //если файл существует, озвращаем T, нет - F 
                                              //IntPtr - нулевое поле 
            {
                return true; //устройство найдено 
            }
            else
            {
                return false; //устройство не надено 
            }
        }
        public bool OpenWrite(string FileName)
        {
            // открытие существующего файла для записи 
            handle = CreateFile(FileName, GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0); //параметры открытия 
                                                                                                    //handle - дескриптор окна 
            if (handle != System.IntPtr.Zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        unsafe public int Read(byte[] buffer, int count) //чтение ячеек flash
                                                         //сборщик мусора среды CLR может 
                                                         //Во избежание этого блок fixed используется для получения указателя на память и его пометки таким образом, 
                                                         //что его не смог переместить сборщик мусора. В конце блока fixed память снова становится доступной для перемещения путем сборки мусора. 
                                                         //Эта способность называется декларативным закреплением.
        {
            int n = 0;
            fixed (byte* p = buffer)
            {
                if (!ReadFile(handle, p, count, &n, 0)) //параметры чтения 
                {
                    return 0;
                }
            }
            return n;
        }
        unsafe public int Write(byte[] buffer, int index, int count) //запись ячеек flash 
        {
            int n = 0;
            fixed (byte* p = buffer)
                if (!WriteFile(handle, p + index, 512, &n, 0)) //параметры записи 
                {
                    return 0;
                }
            return n;
        }
        public bool Close()
        //bool - используется для объявления переменных для хранения логических значений, true и false. 
        {
            return CloseHandle(handle); //закрыть дискриптор откна 
        }
    }
}


