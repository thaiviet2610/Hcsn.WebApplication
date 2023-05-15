using Hcsn.WebApplication.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.result.export
{
    public class ExportAssetIncrementDetailResult
    {
        #region Field
        public static List<string> HeaderAssetIncrements = new()
                {
                    Resources.TableExcelHeaderColumnIndex,Resources.TableExcelHeaderColumnVoucherCode,Resources.TableExcelHeaderColumnVoucherDate,
                    Resources.TableExcelHeaderColumnIncrementDate,Resources.TableExcelHeaderColumnPrice,Resources.TableExcelHeaderColumnDescription,
                };
        public static List<string> HeaderAssets = new()
                {
                    Resources.TableExcelHeaderColumnIndex,Resources.TableExcelHeaderColumnAssetCode,Resources.TableExcelHeaderColumnAssetName,
                    Resources.TableExcelHeaderColumnDepartment,Resources.TableExcelHeaderColumnCost,
                    Resources.TableExcelHeaderColumnDepreciationValue,Resources.TableExcelHeaderColumnResidualValue
                };

        public static string TitleAssetIncrementTable = Resources.TableExcelAssetIncrementDetailTitleAssetIncrement;

        public static string TitleAssetTable = Resources.TableExcelAssetIncrementDetailTitleAsset;


        public static string SheetName = Resources.TableExcelAssetIncrementDetailSheetName;

        public static string NoData = Resources.TableExcelNoData;

        #endregion

        /// <summary>
        /// Dữ liệu file excel
        /// </summary>
        public object Data { get; set; }
    }
}
