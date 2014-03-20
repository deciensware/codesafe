﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;

namespace KeePass.Utils
{
    internal static class ErrorReport
    {
        public static void Report(Exception ex)
        {
            var sb = new StringBuilder();

            AddAppInfo(sb);
            sb.AppendLine();

            AddErrorDetails(sb, ex);
            AddAddress(sb);
            sb.AppendLine();

            AddDeviceInfo(sb);

            new EmailComposeTask
            {
                To = "codesafe-crash@hotmail.com",
                Subject = "Report: Open database error",
                Body = sb.ToString()
            }.Show();
        }

        private static void AddAppInfo(StringBuilder sb)
        {
            sb.AppendLine("CodeSafe info:");

            sb.Append("Version: ");
            sb.AppendLine(PhoneHelper.GetAppAttribute("Version"));
        }

        private static void AddDeviceInfo(StringBuilder sb)
        {
            sb.AppendLine("Device info:");

            sb.Append("Manufacturer: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceManufacturer"));

            sb.Append("Device: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceName"));

            sb.Append("Instance ID: ");
            sb.AppendLine(AppSettings.Instance.InstanceID);

            sb.Append("Firmware: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceFirmwareVersion"));

            sb.Append("Hardware: ");
            sb.AppendLine((string)DeviceExtendedProperties
                .GetValue("DeviceHardwareVersion"));

            sb.Append("Memory: ");
            sb.Append(DeviceExtendedProperties
                .GetValue("ApplicationCurrentMemoryUsage"));
            sb.Append(" (Max: ");

            sb.Append(DeviceExtendedProperties
                .GetValue("ApplicationPeakMemoryUsage"));

            sb.Append(")/");
            sb.Append(DeviceExtendedProperties
                .GetValue("DeviceTotalMemory"));
            sb.AppendLine();
        }

        private static void AddErrorDetails(
            StringBuilder sb, Exception ex)
        {
            sb.AppendLine("Error details:");

            sb.AppendLine(ex.GetType().FullName);

            sb.AppendLine(ex.Message);
            sb.AppendLine();

            sb.AppendLine("Stack Trace:");
            sb.AppendLine(ex.StackTrace);
        }

        private static void AddAddress(StringBuilder sb)
        {
            var page = App.Current.RootFrame.Content
                as PhoneApplicationPage;

            if (page == null)
                return;

            sb.Append("Uri: ");

            var uri = page.NavigationService
                .CurrentSource.ToString();
            
            var regexs = Properties.Resources.UriCensors.Split(
                new[] {Environment.NewLine}, StringSplitOptions.None);
            foreach (var regex in regexs)
                uri = Regex.Replace(uri, regex, "«Censored»");

            sb.AppendLine(uri);
        }
    }
}