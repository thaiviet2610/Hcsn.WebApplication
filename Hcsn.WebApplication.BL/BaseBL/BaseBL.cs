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
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện thêm mới:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult DeleteRecord(Guid recordId)
        {
			var validate = ValidateCustomDelete(new List<Guid>() { recordId });
			if(!validate.IsSuccess)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
					Data = validate
				};
			}
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
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện xóa:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult DeleteMultipleRecord(List<Guid> entitiesId)
		{
			var validate = ValidateCustomDelete(entitiesId);
			if (!validate.IsSuccess)
			{
				return new ServiceResult
				{
					IsSuccess = false,
					ErrorCode = ErrorCode.InvalidateData,
					Message = ServiceResource.InvalidData,
					Data = validate
				};
			}
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
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
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
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
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
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult InsertRecord(T record)
        {
            var validateResult = ValidateRequesData(record);
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
		/// <param name="recordId">Id bản ghi muốn sửa đổi</param>
		/// <param name="record">Bản ghi muốn sửa đổi</param>
		/// <returns>
		/// Đối tượng ServiceResult thể hiện kết quả việc thực hiện logic:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		public ServiceResult UpdateRecord(Guid recordId, T record)
        {
            // Validate dữ liệu đầu vào
            var validateResult = ValidateRequesData(record);
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
		/// <returns>
		/// Đối tượng ValidateResult thể hiện kết quả việc thực hiện validate:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual ValidateResult ValidateRequesData(T record)
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
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// true: không có giá trị rỗng
		/// false: có giá trị rỗng
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		private bool ValidateEmpty(T record)
		{
            bool check = true;
			var validateEmpty = new List<String>();
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
		/// Hàm validate riêng dữ liệu cho việc thêm mới hoặc sửa bản ghi
		/// </summary>
		/// <param name="record">bản ghi cần validate</param>
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// true: thành công
		/// false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual bool ValidateCustom(T record)
        {
            return true;
        }

		/// <summary>
		/// Hàm validate riêng dữ liệu cho việc xóa bản ghi
		/// </summary>
		/// <param name="recordsId">Danh sách Id các bản ghi cần validate</param>
		/// <returns>
		/// Kết quả validate dữ liệu:
		/// IsSuccess == true: thành công
		/// IsSuccess == false: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		protected virtual ValidateResult ValidateCustomDelete(List<Guid> recordsId)
		{
			return new ValidateResult
			{
				IsSuccess = true
			};
		}




		#endregion
	}
}
