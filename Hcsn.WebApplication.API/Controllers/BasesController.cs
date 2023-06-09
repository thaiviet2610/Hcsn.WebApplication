﻿using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.Common.Entities.DTO.result.error;
using Hcsn.WebApplication.Common.Entities.DTO.result.service;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasesController<T> : ControllerBase
    {
        #region Field
        private IBaseBL<T> _baseBL;
        #endregion

        #region Constructor
        public BasesController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }
		#endregion

		#region Method
		/// <summary>
		/// API Lấy ra danh sách tất cả các bản ghi
		/// </summary>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTVIET (20/03/2023)
		[HttpGet]
        public IActionResult GetAllRecord()
        {
            try
            {
                var result = _baseBL.GetAllRecord();
                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK, result.Data);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = ErrorCode.NotFound,
                        DevMsg = ErrorResource.DevMsg_NotFound,
                        UserMsg = ErrorResource.UserMsg_NotFound,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception)
            {
				return HandleErrorException();
			}
        }

		/// <summary>
		/// API Lấy thông tin chi tiết 1 bản ghi theo id
		/// </summary>
		/// <param name="recordId">ID bản ghi muốn lấy</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTVIET (20/03/2023)
		[HttpGet("{recordId}")]
        public IActionResult GetRecordById([FromRoute]Guid recordId)
        {
            try
            {
                var result = _baseBL.GetRecordById(recordId);
                if (result.IsSuccess)
                {
					return StatusCode(StatusCodes.Status200OK, result.Data);
                }
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                {
                    ErrorCode = ErrorCode.NotFound,
                    DevMsg = ErrorResource.DevMsg_NotFound,
                    UserMsg = ErrorResource.UserMsg_NotFound,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = result.Message,
                });
            }
            catch (Exception)
            {
				return HandleErrorException();
			}
        }

		/// <summary>
		/// Hàm thêm mới 1 bản ghi
		/// </summary>
		/// <param name="record">Bản ghi muốn thêm</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		[HttpPost]
        public IActionResult InsertRecord([FromBody]T record)
        {
            try
            {
                var result = _baseBL.InsertRecord(record);
                if (result.IsSuccess)
                {
                    return StatusCode(201);
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.InvalidateData)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InvalidateData,
                        DevMsg = ErrorResource.DevMsg_InvalidData,
                        UserMsg = ErrorResource.UserMsg_InvalideData,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else
                {
					return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InsertFailed,
                        DevMsg = ErrorResource.DevMsg_InsertFailed,
                        UserMsg = ErrorResource.UserMsg_InsertFailed,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception)
            {
				return HandleErrorException();
			}

        }

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi
		/// </summary>
		/// <param name="recordId">Id bản ghi muốn sửa đổi</param>
		/// <param name="record">Bản ghi muốn sửa đổi</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		[HttpPut("{recordId}")]
        public IActionResult UpdateRecordById([FromRoute]Guid recordId, [FromBody]T record)
        {
            try
            {
                var result = _baseBL.UpdateRecord(recordId, record);
                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else if (!result.IsSuccess && result.ErrorCode == ErrorCode.InvalidateData)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = ErrorCode.InvalidateData,
                        DevMsg = ErrorResource.DevMsg_InvalidData,
                        UserMsg = ErrorResource.UserMsg_InvalideData,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = ErrorCode.UpdateFailed,
                        DevMsg = ErrorResource.DevMsg_UpdateFailed,
                        UserMsg = ErrorResource.UserMsg_UpdateFailed,
                        TraceId = HttpContext.TraceIdentifier,
                        MoreInfo = result.Data,
                    });
                }
            }
            catch (Exception)
            {
				return HandleErrorException();

			}
        }

		/// <summary>
		/// API xóa 1 bản ghi
		/// </summary>
		/// <param name="recordId">Id bản ghi muốn xóa</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		[HttpDelete("{recordId}")]
        public ActionResult DeleteRecordById([FromRoute]Guid recordId)
        {
            try
            {
                var result = _baseBL.DeleteRecord(recordId);
                if (result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                {
                    ErrorCode = ErrorCode.DeleteFailed,
                    DevMsg = ErrorResource.DevMsg_DeleteFailed,
                    UserMsg = ErrorResource.UserMsg_DeleteFailed,
                    TraceId = HttpContext.TraceIdentifier,
                    MoreInfo = result,
                });
                
            }
            catch (Exception)   
            {
				return HandleErrorException();
			}
        }

		/// <summary>
		/// API xóa 1 bản ghi
		/// </summary>
		/// <param name="entitiesId">Danh sách id bản ghi muốn xóa</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/03/2023)
		[HttpDelete]
		public ActionResult DeleteMultipleRecord([FromBody] List<Guid> entitiesId)
		{
			try
			{
				var result = _baseBL.DeleteMultipleRecord(entitiesId);
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK, result.Data);
				}
				return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
				{
					ErrorCode = ErrorCode.DeleteMultipleFailed,
					DevMsg = ErrorResource.DevMsg_DeleteMultipleFailed,
					UserMsg = ErrorResource.UserMsg_DeleteMultipleFailed,
					TraceId = HttpContext.TraceIdentifier,
					MoreInfo = result,
				});

			}
			catch (Exception)
			{
				return HandleErrorException();
			}
		}

		/// <summary>
		/// Hàm xử lý lỗi khi xảy ra excepcion
		/// </summary>
		/// <returns>Đối tượng chứa thông tin lỗi</returns>
		/// Created by: LTVIET (15/04/2023)
		private ActionResult HandleErrorException()
		{
			return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
			{
				ErrorCode = ErrorCode.Exception,
				DevMsg = ErrorResource.DevMsg_Exception,
				UserMsg = ErrorResource.UserMsg_Exception,
				TraceId = HttpContext.TraceIdentifier,
				MoreInfo = ErrorResource.Exception_MoreInfo,
			});
		}
		#endregion
	}
}
