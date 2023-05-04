using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Constants.ProcedureName
{
    public static class ProcedureName
    {
        /// <summary>
        /// Thêm 1 bản ghi mới
        /// </summary>
        public static string Insert = "Proc_{0}_Insert";


        /// <summary>
        /// Lấy thông tin tất cả các bản ghi
        /// </summary>
        public static string GetAll = "Proc_{0}_GetAll";

        /// <summary>
        /// Lấy thông tin một bản ghi theo id
        /// </summary>
        public static string GetById = "Proc_{0}_GetById";

        /// <summary>
        /// Sửa thông tin một bản ghi
        /// </summary>
        public static string Update = "Proc_{0}_Update";

        /// <summary>
        /// Xóa tất cả các bản ghi
        /// </summary>
        public static string DeleteAll = "Proc_{0}_DeleteAll";

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        public static string DeleteMultiple = "Proc_{0}_DeleteMultiple";

        /// <summary>
        /// Xóa một bản ghi
        /// </summary>
        public static string DeleteById = "Proc_{0}_DeleteById";

        /// <summary>
        /// Thực hiện lọc, phân trang các bản ghi
        /// </summary>
        public static string Filter = "Proc_{0}_Filter";

        /// <summary>
        /// thực hiện kiểm tra xem có bị trùng code không ?
        /// </summary>
        public static string CheckDuplicate = "Proc_{0}_GetNumberRecordOfDuplicate{1}";

        /// <summary>
        /// Lấy ra mã bản ghi của lần nhập gần nhất
        /// </summary>
        public static string GetNewCode = "Proc_{0}_GetNewCode";

        /// <summary>
        /// Lấy ra tất cả các bản ghi có id không nằm trong danh sách truyền vào
        /// </summary>
        public static string FilterRecordNotIn = "Proc_{0}_FilterRecordNotIn";
    }
}
