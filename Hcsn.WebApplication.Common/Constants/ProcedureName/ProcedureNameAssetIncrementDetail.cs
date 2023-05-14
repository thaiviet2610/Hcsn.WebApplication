using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Constants.ProcedureName
{
	public class ProcedureNameAssetIncrementDetail
	{
		/// <summary>
		/// Xóa nhiều chứng từ chi tiết theo danh sách id tài sản
		/// </summary>
		//public static string DeleteMultipleByIdAssets = "Proc_FixedAssetIncrementDetail_DeleteMultipleByIdAssets";
		public static string DeleteMultipleByIdAssets = $"DELETE FROM fixed_asset_increment_detail WHERE fixed_asset_id IN @p_ids";


		/// <summary>
		/// Xóa nhiều chứng từ chi tiết theo danh sách id chứng từ
		/// </summary>
		public static string DeleteMultipleByIdVouchers = "Proc_FixedAssetIncrementDetail_DeleteMultipleByIdVouchers";
	}
}
