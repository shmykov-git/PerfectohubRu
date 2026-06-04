using System;
using System.Net.NetworkInformation;

namespace PerfectohubRu.Tools
{
    public static class IdHelper
    {
        public static string GetMacAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    byte[] mac = ni.GetPhysicalAddress().GetAddressBytes();
                    if (mac.Length > 0)
                        return BitConverter.ToString(mac);
                }
            }

            return "UnknownMAC";
        }
    }
}
