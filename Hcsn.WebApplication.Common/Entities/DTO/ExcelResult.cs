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
        public static List<string> headers = new()
				{
					"STT","Mã tài sản","Tên tài sản","Loại tài sản","Bộ phận sử dụng","Số lượng","Nguyên giá","HM/KH lũy kế","Giá trị còn lại"
				};

		public static string title = "Danh sách tài sản";
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
