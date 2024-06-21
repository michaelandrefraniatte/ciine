using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace ciine
{
    internal static class Program
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        private const string alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (GetMac() != "5C80B6FAB3A1")
                {
                    return;
                }
                if (GetInfo() != "80158F43BFEBFBFF000906EA3A7A691C-5826-2020-0118-164738000000")
                {
                    return;
                }
                if (!GetChecksum())
                {
                    return;
                }
                if (!GetUser("micha", ""))
                {
                    return;
                }
                if (AlreadyRunning())
                {
                    return;
                }
                if (!hasAdminRights())
                {
                    RunElevated();
                    return;
                }
            }
            catch
            {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static string GetMac()
        {
            try
            {
                String firstMacAddress = System.Net.NetworkInformation.NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
                return firstMacAddress;
            }
            catch
            {
                return "";
            }
        }
        public static string GetInfo()
        {
            try
            {
                string cpuInfo = string.Empty;
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
                string drive = "C";
                ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
                dsk.Get();
                string volumeSerial = dsk["VolumeSerialNumber"].ToString();
                string uuidInfo = string.Empty;
                ManagementClass mcu = new ManagementClass("Win32_ComputerSystemProduct");
                ManagementObjectCollection mocu = mcu.GetInstances();
                foreach (ManagementObject mou in mocu)
                {
                    uuidInfo = mou.Properties["UUID"].Value.ToString();
                    break;
                }
                if (volumeSerial != null & volumeSerial != "" & cpuInfo != null & cpuInfo != "" & uuidInfo != null & uuidInfo != "")
                    return volumeSerial + cpuInfo + uuidInfo;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        private static bool GetChecksum()
        {
            try
            {
                return GetChecksum("ciine.exe");
            }
            catch 
            { 
                return false; 
            }
        }
        private static bool GetChecksum(string exePath)
        {
            SHA1 sha1 = SHA1.Create();
            FileStream fs = new FileStream(exePath, FileMode.Open, FileAccess.Read);
            string checksum = BitConverter.ToString(sha1.ComputeHash(fs)).Replace("-", "");
            fs.Close();
            string salt = GetSalt(10);
            string hashedPass = HashPassword(salt, checksum);
            using (System.IO.StreamReader file = new System.IO.StreamReader("ciine"))
            {
                if (file.ReadLine() == hashedPass)
                {
                    return true;
                }
            }
            return false;
        }
        private static string GetSalt(int saltSize)
        {
            float key = 0.6f;
            StringBuilder strB = new StringBuilder("");
            while ((saltSize--) > 0)
                strB.Append(alphanumeric[(int)(key * alphanumeric.Length)]);
            return strB.ToString();
        }
        private static string HashPassword(string salt, string password)
        {
            string mergedPass = string.Concat(salt, password);
            return EncryptUsingMD5(mergedPass);
        }
        private static string EncryptUsingMD5(string inputStr)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                return sBuilder.ToString();
            }
        }
        static bool GetUser(string username, string password)
        {
            try
            {
                IntPtr token;
                bool result = LogonUser(username, null, password, 3, 0, out token);
                int error = Marshal.GetLastWin32Error();
                return result | error == 1327;
            }
            catch
            {
                return false;
            }
        }
        private static bool AlreadyRunning()
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(thisprocessname);
            if (processes.Length > 1)
                return true;
            else
                return false;
        }
        public static bool hasAdminRights()
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public static void RunElevated()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = Application.ExecutablePath;
                Process.Start(processInfo);
            }
            catch { }
        }
    }
}