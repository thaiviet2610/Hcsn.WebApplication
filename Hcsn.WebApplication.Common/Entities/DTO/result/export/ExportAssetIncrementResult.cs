using Hcsn.WebApplication.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.result.export
{
    public class ExportAssetIncrementResult
    {
        #region Field
        public static List<string> Headers = new()
                {
                    Resources.TableExcelHeaderColumnIndex,Resources.TableExcelHeaderColumnVoucherCode,Resources.TableExcelHeaderColumnVoucherDate,
                    Resources.TableExcelHeaderColumnIncrementDate,Resources.TableExcelHeaderColumnPrice,Resources.TableExcelHeaderColumnDescription,
                };

        public static string Title = Resources.TableExcelAssetIncrementTitle;

        public static string SheetName = Resources.TableExcelAssetIncrementSheetName;

        public static string NoData = Resources.TableExcelNoData;

        public static string FooterTotal = Resources.FooterTableExcelTotal;
        #endregion

        /// <summary>
        /// Tên file excel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dữ liệu file excel
        /// </summary>
        public object Data { get; set; }
    }
}
