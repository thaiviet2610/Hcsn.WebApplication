﻿using Dapper;
using Hcsn.WebApplication.Common.Entities.DTO.result.service;
using Hcsn.WebApplication.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Data.Common;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Constants;
using System.Reflection;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.BL.AssetBL;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Hcsn.WebApplication.Common.Resource;
using System.Collections;
using OfficeOpenXml.DataValidation;
using System.IO;
using Hcsn.WebApplication.Common.Entities.DTO.entityDTO;
using Hcsn.WebApplication.Common.Entities.DTO.result.error;

namespace Hcsn.WebApplication.API.Controllers
{

    public class AssetsController : BasesController<FixedAsset>
    {
        #region Field
        private IAssetBL _assetBL;
		#endregion
		#region Construction
		public AssetsController(IAssetBL assetBL) : base(assetBL)
		{
			_assetBL = assetBL;
		} 
		#endregion




		/// <summary>
		/// API phân trang, lọc danh sách tài sản
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <param name="departmentId">Id phòng ban tìm kiếm</param>
		/// <param name="fixedAssetCatagortId">Id loại tài sản tìm kiếm</param>
		/// <param name="pageSize">Số bản ghi trong 1 trang</param>
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns>Danh sách các tài sản phù hợp</returns>
		/// Created by: LTVIET (09/03/2023)
		[HttpGet("Filter")]
        public IActionResult GetPaging(
            [FromQuery] string? keyword,
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? fixedAssetCatagortId,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var result = _assetBL.GetPaging(keyword, departmentId, fixedAssetCatagortId, pageSize, pageNumber);
                if (!result.IsSuccess)
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
				return StatusCode(StatusCodes.Status200OK, result.Data);
			}
            catch (Exception)
            {
				return HandleErrorException();
			}
        }

		/// <summary>
		/// API lấy ra code mới dựa theo code ở lần nhập gần nhất + 1
		/// </summary>
		/// <returns>Mã code mới</returns>
		/// Created by: LTViet (20/03/2023)
		[HttpGet("NewCode")]
		public IActionResult GetNewCode()
		{
			try
			{
				var result = _assetBL.GetNewCode();
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK, result.Data);
				}

				return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
				{
					ErrorCode = ErrorCode.GenerateNewCodefailed,
					DevMsg = ErrorResource.DevMsg_GetNewCodeFailed,
					UserMsg = ErrorResource.UserMsg_GetNewCodeFailed,
					MoreInfo = result.Data,
					TraceId = HttpContext.TraceIdentifier
				});
			}
			catch (Exception)
			{
				return HandleErrorException();
			}

		}

		/// <summary>
		/// API xuất danh sách tài sản theo phân trang, bộ lọc ra file excel 
		/// </summary>
		/// <param name="keyword"> Từ khóa tìm kiếm</param>
		/// <param name="departmentId"> Id phòng ban tìm kiếm</param>
		/// <param name="fixedAssetCatagortId"> Id loại tìa sản tìm kiếm</param>
		/// <returns> Kết quả việc thực hiện xuất file excel</returns>
		/// Created by: LTVIET (29/03/2023)
		[HttpGet("Export")]
		public IActionResult GetExcelFiles(
			[FromQuery] string? keyword,
			[FromQuery] Guid? departmentId,
			[FromQuery] Guid? fixedAssetCatagortId)
		{
			try
			{
				var stream = _assetBL.ExportExcel(keyword, departmentId, fixedAssetCatagortId);
				if(stream != null)
				{
					return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
				}
				return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
				{
					ErrorCode = ErrorCode.ExportExcelFailed,
					DevMsg = ErrorResource.DevMsg_ExportExcelFailed,
					UserMsg = ErrorResource.UserMsg_ExportExcelFailed,
					TraceId = HttpContext.TraceIdentifier,
				});
				
			}
			catch (Exception)
			{
				return HandleErrorException();
			}
		}

		/// <summary>
		/// API phân trang, lọc danh sách tài sản
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <param name="pageSize">Số bản ghi trong 1 trang</param>
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <param name="ids">Danh sách các id của các tài sản không cần lấy ra</param>
		/// <returns>Danh sách các tài sản phù hợp</returns>
		/// Created by: LTVIET (09/03/2023)
		[HttpPost("FilterNotIn")]
		public IActionResult GetRecordNotIn(
			[FromBody] FixedAssetFilterNoActive assetFilterNoActive,
			[FromQuery] string? keyword,
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageNumber = 1)
		{
			try
			{
				var assetsNotIn = assetFilterNoActive.AssetsNotIn;
				var assetsActive = assetFilterNoActive.AssetsActive;
				var result = _assetBL.GetAllAssetNoActive(keyword, pageSize, pageNumber, assetsNotIn, assetsActive);
				if (!result.IsSuccess)
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
				return StatusCode(StatusCodes.Status200OK, result.Data);
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

	}
}
