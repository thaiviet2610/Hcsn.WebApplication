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
        /// 1: Nếu insert thành công
        /// 2: Nếu insert thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int InsertRecord(T record);

        /// <summary>
        /// Hàm truy cập database sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int UpdateRecord(Guid recordId, T record);

        /// <summary>
        /// Hàm truy cập database xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        int DeleteRecord(Guid recordId);

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="entitiesId">Danh sách bản ghi cần xóa</param>
		/// <returns>
		/// Kết quả việc thực hiện xóa nhiều bản ghi
		/// 1: Nếu update thành công
		/// 0: Nếu update thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		int DeleteMultipleRecord(List<Guid> entitiesId);

        /// <summary>
        /// Hàm truy cập database lấy ra mã code ở lần nhập gần nhất
        /// </summary>
        /// <returns>Mã code của đối tượng</returns>
        /// Created by: LTViet (20/03/2023)
        string? GetNewCode();

        /// <summary>
        /// Hàm truy cập database lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
        /// </summary>
        /// <param name="recordCode">Code cần tìm</param>
        /// <param name="recordId">Id cần tìm </param>
        /// <returns>Số bản ghi cần tìm</returns>
        /// Created by: LTViet (20/03/2023)
        int GetRecordByCode(string recordCode, Guid recordId);

        /// <summary>
        /// Hàm kết nối tới MySQL database
        /// </summary>
        /// <returns>Đối tượng kết nối tới MySQL database</returns>
        /// Created by: LTViet (20/03/2023)
        IDbConnection GetOpenConnection();

        /// <summary>
        /// Thực hiện truy vấn một hàng, trả về dữ liệu đã nhập là <typeparamref name="department"/>.
        /// </summary>
        /// <param name="cnn"> Đối tượng kết nối tới database</param>
        /// <param name="sql"> Câu lệnh SQL để thực thi cho truy vấn này</param>
        /// <param name="param"> Các tham số để sử dụng cho truy vấn này</param>
        /// <param name="transaction"> Giao dịch để sử dụng cho truy vấn này.</param>
        /// <param name="commandTimeout"> Số giây trước khi hết thời gian thực thi lệnh.</param>
        /// <param name="commandType"> Nó có phải là một proc được lưu trữ hoặc một batch không?</param>
        /// <returns>
        /// Một chuỗi dữ liệu của loại được cung cấp; 
        /// nếu một loại cơ bản (int, chuỗi, v.v.) được truy vấn thì dữ liệu từ cột đầu tiên được giả định, 
        /// nếu không, một phiên bản được tạo trên mỗi hàng và ánh xạ trực tiếp column-name===member-name được giả định (không phân biệt chữ hoa chữ thường)
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        Object QueryFirstOrDefault<Object>(IDbConnection cnn, string sql, object? param = null, 
            IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Thực thi một lệnh trả về nhiều tập hợp kết quả và truy cập lần lượt từng tập hợp
        /// </summary>
        /// <param name="cnn"> Đối tượng kết nối tới database</param>
        /// <param name="sql"> Câu lệnh SQL để thực thi cho truy vấn này</param>
        /// <param name="param"> Các tham số để sử dụng cho truy vấn này</param>
        /// <param name="transaction"> Giao dịch để sử dụng cho truy vấn này.</param>
        /// <param name="commandTimeout"> Số giây trước khi hết thời gian thực thi lệnh.</param>
        /// <param name="commandType"> Nó có phải là một proc được lưu trữ hoặc một batch không?</param>
        /// <returns>một multiple result sets kiểu GridReader</returns>
        /// Created by: LTViet (20/03/2023)
        GridReader QueryMultiple(IDbConnection cnn, string sql, object? param = null, 
            IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Thực thi SQL được tham số hóa
        /// </summary>
        /// <param name="cnn"> Đối tượng kết nối tới database</param>
        /// <param name="sql"> Câu lệnh SQL để thực thi cho truy vấn này</param>
        /// <param name="param"> Các tham số để sử dụng cho truy vấn này</param>
        /// <param name="transaction"> Giao dịch để sử dụng cho truy vấn này.</param>
        /// <param name="commandTimeout"> Số giây trước khi hết thời gian thực thi lệnh.</param>
        /// <param name="commandType"> Nó có phải là một proc được lưu trữ hoặc một batch không?</param>
        /// <returns> Số bản ghi bị ảnh hưởng</returns>
        /// Created by: LTViet (20/03/2023)
        int Execute(IDbConnection cnn, string sql, object? param = null, 
            IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        #endregion
    }
}
