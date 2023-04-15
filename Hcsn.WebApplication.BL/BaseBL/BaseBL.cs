using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Constants;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hcsn.WebApplication.Common.Resource;
using System.Reflection;

namespace Hcsn.WebApplication.BL.BaseBL
{
    public class BaseBL<T> : IBaseBL<T>
    {
        #region Field
        private IBaseDL<T> _baseDL;

        protected List<ValidateResult> inValidList = new();
        #endregion

        #region Constructor
        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }
        #endregion

        #region Method

        /// <summary>
        /// Hàm xử lý logic khi xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">Id bản ghi muốn xóa</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult DeleteRecord(Guid recordId)
        {
            var numberOfAffectedRows = _baseDL.DeleteRecord(recordId);
            if (numberOfAffectedRows == 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.DeleteFailed,
                    Message = ServiceResource.DeleteFailed
                };
            }
            return new ServiceResult
            {
                IsSuccess = true
            };
        }

		/// <summary>
		/// Hàm xử lý logic khi xóa nhiều bản ghi
		/// </summary>
		/// <param name="entitiesId">Danh sách Id các bản ghi muốn xóa</param>
		/// <returns>
		/// Kết quả việc xóa nhiều bản ghi
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult DeleteMultipleRecord(List<Guid> entitiesId)
		{
			var numberOfAffectedRows = _baseDL.DeleteMultipleRecord(entitiesId);
			if (numberOfAffectedRows == 0)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.DeleteMultipleFailed,
					Message = ServiceResource.DeleteMultipleFailed,
				};
			}
			return new ServiceResult
			{
				IsSuccess = true,
                Data = numberOfAffectedRows
			};
		}

		/// <summary>
		/// Hàm xử lý logic khi lấy ra danh sách tất cả các bản ghi
		/// </summary>
		/// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
		/// Created by: LTVIET (20/03/2023)
		public ServiceResult GetAllRecord()
        {
            var records = _baseDL.GetAllRecord();
            if(records == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.NotFound,
                    Message = ServiceResource.NotFound
                };
            }
            return new ServiceResult
            {
                IsSuccess = true,
                Data = records
            };
        }

        /// <summary>
        /// Hàm xử lý logic khi lấy thông tin chi tiết 1 bản ghi theo id từ tầng DL 
        /// </summary>
        /// <param name="recordId">ID bản ghi muốn lấy</param>
        /// <returns>Đối tượng ServiceResult thể hiện kết quả xử lý logic</returns>
        /// Created by: LTVIET (20/03/2023)
        public ServiceResult GetRecordById(Guid recordId)
        {
            var record = _baseDL.GetRecordById(recordId);
            if (record == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.NotFound,
                    Message = ServiceResource.NotFound
                };
            }
            return new ServiceResult
            {
                IsSuccess = true,
                Data = record
            };
        }

        /// <summary>
        /// Hàm xử lý logic khi thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn thêm</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult InsertRecord(T record)
        {
            var validateResult = ValidateRequesData(record,Guid.Empty);
            if(!validateResult.IsSuccess)
            {
                return new ServiceResult
                {
                    // Thất bại
                    IsSuccess = false,
                    ErrorCode = ErrorCode.InvalidateData,
                    Message = ServiceResource.InvalidData,
                    Data = validateResult.Data
                };
            }
            // Thành công
            var numberOfAffectedRows = _baseDL.InsertRecord(record);
            if(numberOfAffectedRows > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.InsertFailed,
                    Message = ServiceResource.InsertFailed,
                };
            }
        }


        /// <summary>
        /// Hàm xử lý logic khi sửa đổi 1 bản ghi
        /// </summary>
        /// <param name="record">Bản ghi muốn sửa đổi</param>
        /// <returns>
        /// Đối tượng ServiceResult thể hiện kết quả xử lý logic
        /// </returns>
        /// Created by: LTViet (20/03/2023)
        public ServiceResult UpdateRecord(Guid recordId, T record)
        {
            // Validate dữ liệu đầu vào
            var validateResult = ValidateRequesData(record,recordId);
            if (!validateResult.IsSuccess)
            {
                return new ServiceResult
                {
                    // Thất bại
                    IsSuccess = false,
                    ErrorCode = ErrorCode.InvalidateData,
                    Message = ServiceResource.InvalidData,
                    Data = validateResult.Data
                };
            }

            // Thành công

            var numberOfAffectedRows = _baseDL.UpdateRecord(recordId, record);
            if (numberOfAffectedRows > 0)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorCode = ErrorCode.UpdateFailed,
                    Message = ServiceResource.UpdateFailed,
                };
            }
        }

		

		

		/// <summary>
		/// Hàm validate chung dữ liệu 
		/// </summary>
		/// <param name="record">bản ghi cần validate</param>
		/// <param name="recordId">Id của bản ghi</param>
		/// <returns>Kết quả validate dữ liệu</returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual ValidateResult ValidateRequesData(T record,Guid recordId)
		{
			var validateEmptyResult =  ValidateEmpty(record);
			var validateCustomResult = ValidateCustom(record);
			if (!validateEmptyResult | !validateCustomResult)
			{
				return new ValidateResult
				{
					IsSuccess = false,
					Data = inValidList
				};
			}
			return new ValidateResult { IsSuccess = true };
		}

		/// <summary>
		/// Hàm validate giá trị các thuộc tính không được để trống
		/// </summary>
		/// <param name="record">Đối tượng chứa các thuộc tính</param>
		/// <returns>Kết quả validate</returns>
		/// Created by: LTViet (20/03/2023)
		private bool ValidateEmpty(T record)
		{
            bool check = true;
			List<String> validateEmpty = new();
			var properties = typeof(T).GetProperties();
            
			foreach (var property in properties)
			{
				var propValue = property.GetValue(record);
				var propName = property.Name;
				
				var requiredAttribute = (HcsnRequiredAttribute?)property.GetCustomAttributes(typeof(HcsnRequiredAttribute)).FirstOrDefault();
				if (requiredAttribute != null && (propValue == null || String.IsNullOrEmpty(propValue.ToString().Trim())))
				{
                    validateEmpty.Add(propName);
                    check = false;
                }
			}
			if(!check)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.Empty,
                    Message = ValidateResource.Empty,
					Data = validateEmpty
				});
			}
			return check;
		}

		/// <summary>
		/// Hàm validate giá trị các thuộc tính kiểu số
		/// </summary>
		/// <param name="record">Đối tượng chứa các thuộc tính</param>
		/// <returns>Kết quả validate</returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual ValidateResult ValidatePropertyNumber(T record)
        {
			List<String> validateNumber = new();
			var properties = typeof(T).GetProperties();
			foreach (var property in properties)
            {
				var propValue = property.GetValue(record);
				string propName = property.Name;
				if (property.IsDefined(typeof(HcsnNumberAttribute), false))
				{
					var attHcsnNumber = property.GetCustomAttributes(typeof(HcsnNumberAttribute), false).FirstOrDefault();
					var propType = (attHcsnNumber as HcsnNumberAttribute).PropType;
					if (propType == (object)TypeValue.Rate && ((double)propValue < 0 || (double)propValue > 100))
					{
						validateNumber.Add(propName);
					}
				}
			}
            if (validateNumber.Count > 0)
            {
				return new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.OutOfRate,
					Data = validateNumber
				};
			}
            return new ValidateResult { IsSuccess = true };
		}

		/// <summary>
		/// Hàm validate riêng dữ liệu 
		/// </summary>
		/// <param name="record">bản ghi cần validate</param>
		/// <returns>Kết quả validate dữ liệu</returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual bool ValidateCustom(T record)
        {
            return true;
        }

		


		#endregion
	}
}
