using Allure.Net.Commons;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Capstone_Project.Support
{
    public static class ReportUtility
    {
        public static void AttachListToAllure(string attachmentName, string listHeader, List<string> items)
        {
            if (items == null || !items.Any())
            {
                return; 
            }

            string formattedList = $"{listHeader}:\n\n" +
                                   string.Join("\n", items.Select(item => $"• {item}"));

            AllureApi.AddAttachment(
                attachmentName,
                "text/plain",
                Encoding.UTF8.GetBytes(formattedList),
                ".txt");
        }
        public static void AttachScreenshot(string attachmentName, byte[] screenshotBytes)
        {
            AllureApi.AddAttachment(
                attachmentName,
                "image/png",
                screenshotBytes,
                ".png");
        }
    }
}