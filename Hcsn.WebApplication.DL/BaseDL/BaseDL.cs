using Dapper;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data.Common;
using static Dapper.SqlMapper;
using MySqlConnector;

namespace Hcsn.WebApplication.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T>
    {

        #region Method

        /// <summary>
        /// Hàm gọi daatabase thực hiện việc xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 0: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public int DeleteRecord(Guid recordId)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.DeleteById, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            GeneratePrimaryKeyValue(properties, parameters, recordId);
            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows;
        }

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
		public int DeleteMultipleRecord(List<Guid> entitiesId)
		{
			// Chuẩn bị tham số đầu vào
			string entityName = GetEntityName();
			string primaryKey = "";
			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
			{
				if (property.IsDefined(typeof(HcsnPrimaryKeyAttribute), false))
				{
					primaryKey = property.Name;
					break;
				}
			}
			string sql = string.Format(ProcedureName.DeleteMultiple, entityName, primaryKey, string.Join("','", entitiesId));
			// Khởi tạo kết nối tới Database
			int numberOfAffectedRows = 0;
			var dbConnection = GetOpenConnection();
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					numberOfAffectedRows = Execute(dbConnection, sql, transaction: transaction);
                    if(numberOfAffectedRows != entitiesId.Count)
                    {
                        throw new Exception();
                    }
					transaction.Commit();
				}
				catch (Exception)
				{
                    numberOfAffectedRows = 0;
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return numberOfAffectedRows;
		}

		/// <summary>
		/// Hàm lấy ra tên của đối tượng được lưu trong database
		/// </summary>
		/// <returns>Tên của đối tượng được lưu trong database</returns>
		/// Created by: LTViet (20/03/2023)
		private static string GetEntityName()
		{
			string nameEntity = "";
			int j = 0;
			for (int i = 0; i < typeof(T).Name.Length; i++)
			{
				if (Char.IsUpper(typeof(T).Name[i]))
				{
					if (j != 0)
					{
						nameEntity += $"_{typeof(T).Name[i]}";
					}
					else
					{
						j += 1;
						nameEntity += typeof(T).Name[i];
					}
				}
				else
				{
					nameEntity += typeof(T).Name[i];
				}
			}

			return nameEntity;
		}

		/// <summary>
		/// API Lấy ra danh sách tất cả các bản ghi
		/// </summary>
		/// <returns>Danh sách tất cả các bản ghi</returns>
		/// Created by: LTVIET (20/03/2023)
		public List<T> GetAllRecord()
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetAll, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored

            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var result = QueryMultiple(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
            var entites = result.Read<T>().ToList();
			dbConnection.Close();
            // Xử lý kết quả trả về 
            return entites;
        }

        /// <summary>
        /// API Lấy thông tin chi tiết 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Bản ghi muốn lấy</returns>
        /// Created by: LTVIET (20/03/2023)
        public T GetRecordById(Guid recordId)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetById, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            GeneratePrimaryKeyValue(properties, parameters, recordId);

            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();

            // Thực hiện gọi vào Database để chạy stored procedure
            var entity = QueryFirstOrDefault<T>(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về 
            return entity;
        }

        /// <summary>
        /// Hàm thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// 1: Nếu insert thành công
        /// 2: Nếu insert thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public int InsertRecord(T record)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Insert, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            AddParametersValue(properties, parameters, record);
            GeneratePrimaryKeyValue(properties, parameters, Guid.Empty);
            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows;
            
        }

        /// <summary>
        /// Hàm sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// 1: Nếu update thành công
        /// 2: Nếu update thất bại
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public int UpdateRecord(Guid recordId,T record)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Update, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            AddParametersValue(properties, parameters, record);
            GeneratePrimaryKeyValue(properties, parameters, recordId);
            // Khởi tạo kết nối tới Database
            // Thực hiện gọi vào Database để chạy stored procedure

            var dbConnection = GetOpenConnection();
            int numberOfAffectedRows = Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            return numberOfAffectedRows;

        }

		/// <summary>
		/// Hàm lấy ra mã code ở lần nhập gần nhất
		/// </summary>
		/// <returns>Mã code của đối tượng</returns>
		/// Created by: LTViet (20/03/2023)
		public string? GetNewCode()
		{
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetNewCode, typeof(FixedAsset).Name);
            // Chuẩn bị tham số đầu vào cho stored
            // Khởi tạo kết nối tới Database
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var asset = QueryFirstOrDefault<FixedAsset>(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
            dbConnection.Close();

            // Xử lý kết quả trả về 
            if (asset == null)
            {
                // Nếu không có đối tượng nào trong database thì trả về kết quả null
                return null;
            }
            else
            {
                return asset.fixed_asset_code;
            }
        }

        /// <summary>
        /// Hàm lấy ra số bản ghi có cùng code nhưng khác id được truyền vào
        /// </summary>
        /// <param name="recordCode">Code cần tìm</param>
        /// <param name="recordId">Id cần tìm </param>
        /// <returns>Số bản ghi cần tìm</returns>
        /// Created by: LTViet (20/03/2023)
        public int GetRecordByCode(string recordCode, Guid recordId)
        {
            string storedProcedureNameCheckSameCode = String.Format(ProcedureName.CheckSameCode, typeof(FixedAsset).Name);
            var parametersCheckSameCode = new DynamicParameters();
            parametersCheckSameCode.Add("p_code", recordCode);
            parametersCheckSameCode.Add("p_id", recordId);
            var dbConnection = GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRowsCheckSameCode = 
                QueryFirstOrDefault<int>(dbConnection, storedProcedureNameCheckSameCode, 
                                         parametersCheckSameCode, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            return numberOfAffectedRowsCheckSameCode;
        }

        /// <summary>
        /// Sinh dữ liệu khóa chính
        /// </summary>
        /// <param name="properties">Các thuộc tính của đối tượng</param>
        /// <param name="parameters">Các tham số đầu vào</param>
        /// <param name="entityId">Id của đối tượng</param>
        /// Create by: LTVIET (20/03/2023)
        protected virtual void GeneratePrimaryKeyValue
            (PropertyInfo[] properties, DynamicParameters parameters, Guid? entityId)
        {
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(HcsnPrimaryKeyAttribute), false))
                {
                    if (entityId != Guid.Empty)
                    {
                        parameters.Add($"p_{property.Name}", entityId);
                    }
                    else
                    {
                        parameters.Add($"p_{property.Name}", Guid.NewGuid());
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Hàm thêm các giá trị vào parameters để truyền vào storeProcedure
        /// </summary>
        /// <param name="properties">Các thuộc tính của đối tượng</param>
        /// <param name="parameters">Các tham số truyền vào</param>
        /// <param name="entity">Đối tượng truyền vào</param>
        /// Create by: LTVIET (20/03/2023)
        protected virtual void AddParametersValue(PropertyInfo[] properties, DynamicParameters parameters, T entity)
        {
            foreach (var property in properties)
            {
                parameters.Add($"p_{property.Name}", property.GetValue(entity));
            }
        }

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

		
		#endregion
	}
}
