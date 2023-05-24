using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Constants.ProcedureName
{
	public class ProcedureNameAsset
	{
		/// <summary>
		/// Sửa trạng thái của tài sản theo id tài sản
		/// </summary>
		public static string UpdateActiveByIdAsset = $"UPDATE fixed_asset fa SET fa.active = @p_active , fa.modified_date = @p_modified_date WHERE fa.fixed_asset_id IN @p_idAssets";

		/// <summary>
		/// Sửa trạng thái của tài sản theo id chứng từ
		/// </summary>
		public static string UpdateActiveByIdVouchers = $"UPDATE fixed_asset fa SET fa.active = @p_active , fa.modified_date = @p_modified_date WHERE fa.fixed_asset_id IN (SELECT faid.fixed_asset_id FROM fixed_asset_increment_detail faid WHERE faid.voucher_id IN @p_idVouchers)";

		/// <summary>
		/// Lấy danh sách tài sản theo mã chứng từ
		/// </summary>
		public static string GetByVoucherId = $"SELECT fa.fixed_asset_id FROM fixed_asset fa LEFT JOIN fixed_asset_increment_detail faid ON fa.fixed_asset_id = faid.fixed_asset_id WHERE faid.voucher_id IN @p_ids";

		/// <summary>
		/// Kiểm tra tài sản đã chứng từ chưa
		/// </summary>
		public static string CheckIncrement = $"SELECT COUNT(*) FROM fixed_asset_increment_detail faid LEFT JOIN fixed_asset_increment fai ON faid.voucher_id = fai.voucher_id WHERE faid.fixed_asset_id IN @ids";

	}
}
