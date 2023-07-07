using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export
{
    public static class GlobalConfig
    {
        public static string AppId { get; set; }

        public static string AppSecret { get; set; }

        public static string ExportPath { get; set; }

        public static string WikiSpaceId { get; set; }

        public static string DocSaveType { get; set; }

        /// <summary>
        /// 飞书支持导出的文件类型和导出格式
        /// </summary>
        static Dictionary<string, string> fileExtensionDict = new Dictionary<string, string>()
        {
            {"doc","docx" },
            {"docx","docx" },
            {"sheet","xlsx" },
            {"bitable","xlsx" },
        };

        /// <summary>
        /// 获取飞书支持导出的文件格式
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        public static bool GetFileExtension(string objType, out string fileExt)
        {
            return fileExtensionDict.TryGetValue(objType, out fileExt);
        }

        public static void InitAsposeLicense()
        {
            string licenseText = @"<License>
  <Data>
    <LicensedTo>Shanghai Hudun Information Technology Co., Ltd</LicensedTo>
    <EmailTo>317701809@qq.com</EmailTo>
    <LicenseType>Developer OEM</LicenseType>
    <LicenseNote>Limited to 1 developer, unlimited physical locations</LicenseNote>
    <OrderID>200615215909</OrderID>
    <UserID>266166</UserID>
    <OEM>This is a redistributable license</OEM>
    <Products>
      <Product>Aspose.Total for .NET</Product>
    </Products>
    <EditionType>Enterprise</EditionType>
    <SerialNumber>dec1c2e1-d58b-451c-8390-928e7c3dec37</SerialNumber>
    <SubscriptionExpiry>20210617</SubscriptionExpiry>
    <LicenseVersion>3.0</LicenseVersion>
    <LicenseInstructions>https://purchase.aspose.com/policies/use-license</LicenseInstructions>
  </Data>
  <Signature>eeV78yvFXn1N5syK8PGZQTphgkh7uh1m8zws8ihQbPoTPaN3P9/zfQFV9+hINlXSitO9CuoYyHpYMbutyNZcq6854jGteFFAOPlLUWWKurAQ+8Ada4aNkjESkgUN3BrUpfvU4mI7tYDQ4T/anbDYSx/vdb4UWLwqGWftbEplVpk=</Signature>
</License>";

            byte[] licenseData = Encoding.UTF8.GetBytes(licenseText);

            using (MemoryStream stream = new MemoryStream(licenseData))
            {
                License license = new License();
                license.SetLicense(stream);
            }
        }
    }
}
