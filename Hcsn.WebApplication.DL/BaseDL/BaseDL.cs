﻿using Dapper;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using Hcsn.WebApplication.DL.DBConfig;
using Hcsn.WebApplication.Common.Constants.ProcedureName;

namespace Hcsn.WebApplication.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T>
    {
		#region Field
		private IRepositoryDB _repositoryDB;
		#endregion

		#region Constructor
		public BaseDL(IRepositoryDB repositoryDB)
		{
			_repositoryDB = repositoryDB;
		}
		#endregion

		#region Method

		/// <summary>
		/// Hàm gọi daatabase thực hiện việc xóa 1 bản ghi
		/// </summary>
		/// <param name="recordId">Id bản ghi muốn xóa</param>
		/// <returns>
		/// true: Xóa thành công
        /// false: Xóa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public bool DeleteRecord(Guid recordId)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.DeleteById, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            GeneratePrimaryKeyValue(properties, parameters, recordId);
            // Khởi tạo kết nối tới Database
            var dbConnection = _repositoryDB.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = _repositoryDB.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows == 1;
        }

		/// <summary>
		/// Hàm gọi database để thực hiện việc xóa nhiều bản ghi
		/// </summary>
		/// <param name="entitiesId">Danh sách bản ghi cần xóa</param>
		/// <returns>
		/// true: Xóa thành công
		/// false: Xóa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public bool DeleteMultipleRecord(List<Guid> entitiesId)
		{
            // Chuẩn bị tham số đầu vào
            string sql = string.Format(ProcedureName.DeleteMultiple, typeof(T).Name);
			// Khởi tạo kết nối tới Database
			int numberOfAffectedRows = 0;
            var parameters = new DynamicParameters();
			var listIdToString = $"('{string.Join("','", entitiesId)}')";

			parameters.Add("p_ids", listIdToString);
			var dbConnection = _repositoryDB.GetOpenConnection();
            bool checkMultiple = true;
			using (var transaction = dbConnection.BeginTransaction())
			{
				try
				{
					// Thực hiện gọi vào Database để chạy stored procedure
					numberOfAffectedRows = _repositoryDB.Execute(dbConnection, sql,parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                    if(numberOfAffectedRows != entitiesId.Count)
                    {
						transaction.Rollback();
                        checkMultiple = false;
					}
                    else
                    {
						transaction.Commit();
					}
					
				}
				catch (Exception)
				{
					checkMultiple = false;
					transaction.Rollback();
				}
			}
			dbConnection.Close();
			// Xử lý kết quả trả về
			return checkMultiple;
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
            var dbConnection = _repositoryDB.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var result = _repositoryDB.QueryMultiple(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
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
            var dbConnection = _repositoryDB.GetOpenConnection();

            // Thực hiện gọi vào Database để chạy stored procedure
            var entity = _repositoryDB.QueryFirstOrDefault<T>(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về 
            return entity;
        }

		/// <summary>
		/// Hàm thêm mới 1 bản ghi
		/// </summary>
		/// <param name="record">Bản ghi muốn thêm</param>
		/// <returns>
		/// true: thêm mới thành công
		/// false: thêm mới thất bại
		/// </returns>
		/// Created by: LTVIET (20/03/2023)
		public bool InsertRecord(T record)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Insert, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            AddParametersValue(properties, parameters, record);
            GeneratePrimaryKeyValue(properties, parameters, Guid.Empty);
            // Khởi tạo kết nối tới Database
            var dbConnection = _repositoryDB.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = _repositoryDB.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows == 1;
            
        }

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi
		/// </summary>
		/// <param name="record">Bản ghi muốn sửa đổi</param>
		/// <returns>
		/// true: sửa thành công
		/// false: sửa thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public bool UpdateRecord(Guid recordId,T record)
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

            var dbConnection = _repositoryDB.GetOpenConnection();
            int numberOfAffectedRows = _repositoryDB.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            return numberOfAffectedRows == 1;

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
		
		#endregion
	}
}
