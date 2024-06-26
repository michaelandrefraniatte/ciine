﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Device.Net;
using Hid.Net.Windows;

namespace DualSenseAPI.Util
{
    /// <summary>
    /// Utilities to scann for DualSense controllers on HID.
    /// </summary>
    internal class HidScanner
    {
        private static IDeviceFactory hidFactory = new FilterDeviceDefinition((uint)int.Parse("54C", System.Globalization.NumberStyles.HexNumber), (uint)int.Parse("CE6", System.Globalization.NumberStyles.HexNumber), label: "DualSense").CreateWindowsHidDeviceFactory();

        /// <summary>
        /// Lists connected devices.
        /// </summary>
        /// <returns>An enumerable of connected devices.</returns>
        public static IEnumerable<ConnectedDeviceDefinition> ListDevices()
        {
            Task<IEnumerable<ConnectedDeviceDefinition>> scannerTask = hidFactory.GetConnectedDeviceDefinitionsAsync();
            scannerTask.Wait();
            return scannerTask.Result;
        }

        /// <summary>
        /// Gets a device from its information.
        /// </summary>
        /// <param name="deviceDefinition">The information for the connected device.</param>
        /// <returns>The actual device.</returns>
        public static IDevice GetConnectedDevice(ConnectedDeviceDefinition deviceDefinition)
        {
            Task<IDevice> connectTask = hidFactory.GetDeviceAsync(deviceDefinition);
            connectTask.Wait();
            return connectTask.Result;
        }
    }
}