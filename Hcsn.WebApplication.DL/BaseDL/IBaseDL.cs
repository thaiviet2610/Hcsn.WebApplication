using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Hcsn.WebApplication.DL.BaseDL
{
    public interface IBaseDL<T>
    {
        #region Method
        /// <summary>
        /// Hàm truy cập database lấy ra danh sách tất cả các bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả các bản ghi</returns>
        /// Created by: LTVIET (20/03/2023)
        List<T> GetAllRecord();

        /// <summary>
        /// Hàm truy cập database lấy thông tin chi tiết 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Bản ghi muốn lấy</returns>
        /// Created by: LTVIET (20/03/2023)
        T GetRecordById(Guid recordId);

		/// <summary>
		/// Hàm truy cập database thêm mới 1 bản ghi
		/// </summary>
		/// <param name="record">Bản ghi muốn thêm</param>
		/// <returns>
		/// true: thêm mới thành công
		/// false: thêm mới thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool InsertRecord(T record);

		/// <summary>
		/// Hàm truy cập database sửa đổi 1 bản ghi
		/// </summary>
		/// <param name="record">Bản ghi muốn sửa đổi</param>
		/// <returns>
		/// true: sửa thành công
		/// false: sửa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool UpdateRecord(Guid recordId, T record);

		/// <summary>
		/// Hàm truy cập database xóa 1 bản ghi
		/// </summary>
		/// <param name="recordId">Id bản ghi muốn xóa</param>
		/// <returns>
		/// true: Xóa thành công
		/// false: Xóa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool DeleteRecord(Guid recordId);

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="entitiesId">Danh sách bản ghi cần xóa</param>
		/// <returns>
		/// true: Xóa thành công
		/// false: Xóa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		bool DeleteMultipleRecord(List<Guid> entitiesId);
        #endregion
    }
}
