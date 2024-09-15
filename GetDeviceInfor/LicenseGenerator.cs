using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace GetDeviceInfor
{
    class LicenseGenerator
    {
        static void Main(string[] args)
        {
            try
            {
                string filePath = @"D:\Project\CSharp\GetDeviceInfor\GetDeviceInfor\licenseKey.txt";
                string licenseKey = getLicenceKey(filePath);
                //Console.WriteLine("License Key: " + licenseKey);

                string licenseKeyGenerate = generateLicenceKey();

                if (string.IsNullOrWhiteSpace(licenseKey))
                {
                    Console.WriteLine("No key found.");
                }
                else if (licenseKeyGenerate.Equals(licenseKey, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Key is correct.");
                }
                else
                {
                    Console.WriteLine("Key not available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        static string hashToUUID(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }

                string hex = hashString.ToString();
                return $"{hex.Substring(0, 8)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}-{hex.Substring(16, 4)}-{hex.Substring(20, 12)}";
            }
        }

        static void writeKeyToFile(string filePath, string content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing to file: " + ex.Message);
            }
        }

        static string generateLicenceKey()
        {
            string macAddress = getMacAddress();
            string motherboardId = getMotherboardId();
            string hardDriveId = getHardDriveId();
            string cpuId = getCpuId();
            string fileCreationTime = GetFileCreationTime(@"D:\Project\CSharp\GetDeviceInfor\GetDeviceInfor\licenseKey.txt");
            string combinedString = macAddress + motherboardId + hardDriveId + cpuId + fileCreationTime;
            string licenseKeyGenerate = hashToUUID(generateSHA256Hash(combinedString));
            Console.WriteLine("Generated License Key: " + licenseKeyGenerate);
            return licenseKeyGenerate;
        }

        static string getLicenceKey(string filePath)
        {
            string licenceKey = string.Empty;
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        licenceKey = reader.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while reading the file: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
            return licenceKey ?? string.Empty;
        }

        static string getMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return string.Empty;
        }

        static string getMotherboardId()
        {
            string motherboardId = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                foreach (ManagementObject mo in searcher.Get())
                {
                   return mo["SerialNumber"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while getting motherboard ID: " + ex.Message);
            }
            return motherboardId;
        }

        static string getHardDriveId()
        {
            string hardDriveId = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive");
                foreach (ManagementObject mo in searcher.Get())
                {
                    return mo["SerialNumber"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while getting hard drive ID: " + ex.Message);
            }
            return hardDriveId;
        }

        static string getCpuId()
        {
            string cpuId = string.Empty;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                foreach (ManagementObject mo in searcher.Get())
                {
                    return mo["ProcessorId"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while getting CPU ID: " + ex.Message);
            }
            return cpuId;
        }

        static string GetFileCreationTime(string filePath)
        {
            if (File.Exists(filePath))
            {
                DateTime creationTime = File.GetCreationTime(filePath);
                return creationTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return string.Empty;
        }

        static string generateSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
