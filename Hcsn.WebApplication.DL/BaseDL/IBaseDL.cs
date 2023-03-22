using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.DL.BaseDL
{
    public interface IBaseDL<T>
    {
        #region Method
        /// <summary>
        /// API Lấy ra danh sách tất cả các bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả các bản ghi</returns>
        /// Created by: LTVIET (20/03/2023)
        List<T> GetAllRecord();

        /// <summary>
        /// API Lấy thông tin chi tiết 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Bản ghi muốn lấy</returns>
        /// Created by: LTVIET (20/03/2023)
        T GetRecordById(Guid recordId);

        /// <summary>
        /// Hàm thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 2: Nếu insert thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int InsertRecord(T record);

        /// <summary>
        /// Hàm sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int UpdateRecord(Guid recordId, T record);

        /// <summary>
        /// Hàm xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int DeleteRecord(Guid recordId);

        string GetNewCode();

        int GetRecordByCode(string recordCode, Guid recordId);
        #endregion
    }
}
