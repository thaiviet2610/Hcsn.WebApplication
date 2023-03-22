using Dapper;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.Database;
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

namespace Hcsn.WebApplication.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T>
    {
        #region Field
        private IBaseRepository<T> _baseRepository;
        #endregion

        #region Constructor
        public BaseDL(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }
        #endregion

        #region Method

        public int DeleteRecord(Guid recordId)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.DeleteById, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            GeneratePrimaryKeyValue(properties, parameters, recordId);
            // Khởi tạo kết nối tới Database
            var dbConnection = _baseRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = _baseRepository.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows;
        }

        public List<T> GetAllRecord()
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetAll, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored

            // Khởi tạo kết nối tới Database
            var dbConnection = _baseRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var entites = _baseRepository.QueryMultiple(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
            // Xử lý kết quả trả về 
            return entites.Read<T>().ToList();
        }

        public T GetRecordById(Guid recordId)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetById, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            GeneratePrimaryKeyValue(properties, parameters, recordId);

            // Khởi tạo kết nối tới Database
            var dbConnection = _baseRepository.GetOpenConnection();

            // Thực hiện gọi vào Database để chạy stored procedure
            var entity = _baseRepository.QueryFirstOrDefault<T>(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

            // Xử lý kết quả trả về 
            return entity;
        }
        

        public int InsertRecord(T record)
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.Insert, typeof(T).Name);
            // Chuẩn bị tham số đầu vào cho stored
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            AddParametersValue(properties, parameters, record);
            GeneratePrimaryKeyValue(properties, parameters, Guid.Empty );
            // Khởi tạo kết nối tới Database
            var dbConnection = _baseRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRows = _baseRepository.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            // Xử lý kết quả trả về
            return numberOfAffectedRows;
            
        }

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

            var dbConnection = _baseRepository.GetOpenConnection();
            int numberOfAffectedRows = _baseRepository.Execute(dbConnection, storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
            dbConnection.Close();
            return numberOfAffectedRows;

        }

        public string GetNewCode()
        {
            // Chuẩn bị tên stored procedure
            string storedProcedureName = String.Format(ProcedureName.GetNewCode, typeof(Asset).Name);
            // Chuẩn bị tham số đầu vào cho stored
            // Khởi tạo kết nối tới Database
            var dbConnection = _baseRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            var asset = _baseRepository.QueryFirstOrDefault<Asset>(dbConnection, storedProcedureName, commandType: CommandType.StoredProcedure);
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

        public int GetRecordByCode(string recordCode, Guid recordId)
        {
            string storedProcedureNameCheckSameCode = String.Format(ProcedureName.CheckSameCode, typeof(Asset).Name);
            var parametersCheckSameCode = new DynamicParameters();
            parametersCheckSameCode.Add("p_code", recordCode);
            parametersCheckSameCode.Add("p_id", recordId);
            var dbConnection = _baseRepository.GetOpenConnection();
            // Thực hiện gọi vào Database để chạy stored procedure
            int numberOfAffectedRowsCheckSameCode = _baseRepository.QueryFirstOrDefault<int>(dbConnection, storedProcedureNameCheckSameCode, parametersCheckSameCode, commandType: CommandType.StoredProcedure);
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
        protected virtual void GeneratePrimaryKeyValue(PropertyInfo[] properties, DynamicParameters parameters, Guid? entityId)
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

        protected virtual void AddParametersValue(PropertyInfo[] properties, DynamicParameters parameters, Object entity)
        {
            foreach (var property in properties)
            {
                parameters.Add($"p_{property.Name}", property.GetValue(entity));
            }
        }
        #endregion
    }
}
