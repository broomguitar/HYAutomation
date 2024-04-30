using HYAutomation.BLL;
using HYAutomation.Core.Views.Models;
using HYAutomation.Core.Views.ViewModels;
using HYAutomation.Entity_Base;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYAutomation.Core.Views.Utils
{
    internal class ExportToExcel : BaseProductRecordViewModel<ProductInfoModel, ProductInfo>
    {
        protected override IProductRecordBLL<ProductInfo> BLL => throw new NotImplementedException();

        /// <summary>
        /// 在查询结果页面导出Excel,productLogs为导出对象
        /// </summary>
        /// <param name="HeaderName">列名</param>
        /// <param name="Path">路径</param>
        /// <returns>ture:导出成功；false:导出失败</returns>
        public bool DataExport(List<string> HeaderName,string Path)
        {
            bool re = true;
            try
            {

                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                ISheet sheet1 = book.CreateSheet("Sheet1");
                IRow row1 = sheet1.CreateRow(0);
                for (int i = 0; i < HeaderName.Count; i++)
                {
                    row1.CreateCell(i).SetCellValue(HeaderName[i]);
                }

                for (int i = 0; i < ProductLogs.Count; i++)
                {
                    IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(ProductLogs[i].CreateTime);
                    rowtemp.CreateCell(1).SetCellValue(ProductLogs[i].IsOK.ToString());
                    rowtemp.CreateCell(2).SetCellValue(ProductLogs[i].CameraDatas[0].CameraImagePath);
                    rowtemp.CreateCell(3).SetCellValue(ProductLogs[i].WorkLog);
                    rowtemp.CreateCell(4).SetCellValue(ProductLogs[i].Barcode);
                    rowtemp.CreateCell(5).SetCellValue(ProductLogs[i].TypeName);
                }
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);

                book.Write(ms);
                FileStream dumpFile = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite);
                ms.WriteTo(dumpFile);
                ms.Flush();
                ms.Position = 0;
                dumpFile.Close();
                dumpFile.Dispose();
            }
            catch (Exception ex)
            {
                re = false;
                LogHelper.Instance.AddLog(ex.Message);
            }
            return re;
        }
    }
}
