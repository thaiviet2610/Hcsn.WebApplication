﻿using Hcsn.WebApplication.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class ExcelResult
	{
        #region Field
        public static List<string> Headers = new()
				{
					Resources.TableExcelHeaderColumnIndex,Resources.TableExcelHeaderColumnAssetCode,Resources.TableExcelHeaderColumnAssetName,
					Resources.TableExcelHeaderColumnAssetCategory,Resources.TableExcelHeaderColumnDepartment,Resources.TableExcelHeaderColumnQuantity,
					Resources.TableExcelHeaderColumnCost,Resources.TableExcelHeaderColumnDepreciationValue,Resources.TableExcelHeaderColumnResidualValue
				};

		public static string Title = Resources.TableExcelTitle;

		public static string SheetName = Resources.TableExcelSheetName;

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
        public Object Data { get; set; }

        



    }
}
