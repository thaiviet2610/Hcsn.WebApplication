

using MySqlConnector;
using System.Data;
using static Dapper.SqlMapper;

namespace Hcsn.WebApplication.DL.DBConfig
{
	public class RepositoryDB : IRepositoryDB
	{
		/// <summary>
		/// Hàm kết nối tới MySQL database
		/// </summary>
		/// <returns>Đối tượng kết nối tới MySQL database</returns>
		/// Create by: LTVIET (20/03/2023)
		public IDbConnection GetOpenConnection()
		{
			var mySqlConnection = new MySqlConnection(DatabaseContext.ConnectionString);
			mySqlConnection.Open();
			return mySqlConnection;
		}

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
		/// Create by: LTVIET (20/03/2023)
		public int Execute(IDbConnection cnn, string sql, object? param = null,
			IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return cnn.Execute(sql, param, transaction, commandTimeout, commandType);
		}

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
		/// Create by: LTVIET (20/03/2023)
		public GridReader QueryMultiple(IDbConnection cnn, string sql, object? param = null,
			IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			var result = cnn.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
			return result;
		}

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
		/// Create by: LTVIET (20/03/2023)
		public Object QueryFirstOrDefault<Object>(IDbConnection cnn, string sql, object? param = null,
			IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			return cnn.QueryFirstOrDefault<Object>(sql, param, transaction, commandTimeout, commandType);
		}
	}
}
