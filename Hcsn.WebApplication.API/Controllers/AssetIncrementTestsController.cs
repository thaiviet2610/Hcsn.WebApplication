using Hcsn.WebApplication.BL;
using Hcsn.WebApplication.BL.AssetIncrementTest1BL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{

	public class AssetIncrementTestsController : BasesController<FixedAssetIncrement>
	{
		#region Field
		private IAssetIncrementTestBL _assetIncrementBL;
		#endregion

		#region Constructor
		public AssetIncrementTestsController(IAssetIncrementTestBL assetIncrementBL) : base(assetIncrementBL)
		{
			_assetIncrementBL = assetIncrementBL;
		}
		#endregion

		#region Method
		[HttpGet("assetIncrementDTO")]
		public IActionResult GetAssetIncrementDTOById([FromQuery]Guid assetIncrementId)
		{
			try
			{

				var result = _assetIncrementBL.GetAssetIncrementDTOById(assetIncrementId);
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK, result.Data);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.NotFound,
					DevMsg = ErrorResource.DevMsg_NotFound,
					UserMsg = ErrorResource.UserMsg_NotFound,
					TraceId = HttpContext.TraceIdentifier,
					MoreInfo = result.Message,
				});
			}
			catch (Exception ex)
			{
				string traceId = HttpContext.TraceIdentifier;
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(traceId);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.Exception,
					DevMsg = ErrorResource.DevMsg_Exception,
					UserMsg = ErrorResource.UserMsg_Exception,
					TraceId = traceId,
					MoreInfo = ErrorResource.Exception_MoreInfo,
				});
			}
		}

		/// <summary>
		/// API phân trang, lọc danh sách tài sản
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <param name="pageSize">Số bản ghi trong 1 trang</param>
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns>Danh sách các tài sản phù hợp</returns>
		/// Created by: LTVIET (09/03/2023)
		[HttpGet("Filter")]
		public IActionResult GetPaging(
			[FromQuery] string? keyword,
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageNumber = 1)
		{
			try
			{
				var result = _assetIncrementBL.GetPaging(keyword, pageSize, pageNumber);
				if (!result.IsSuccess)
				{
					return StatusCode(500, new ErrorResult
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
			catch (Exception ex)
			{
				string traceId = HttpContext.TraceIdentifier;
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(traceId);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.Exception,
					DevMsg = ErrorResource.DevMsg_Exception,
					UserMsg = ErrorResource.UserMsg_Exception,
					TraceId = traceId,
					MoreInfo = ErrorResource.Exception_MoreInfo,
				});
			}
		}

		[HttpPost("Insert")]
		public IActionResult Insert([FromBody] FixedAssetIncrementDTO entity)
		{
			try
			{
				var result = _assetIncrementBL.InsertAssetIncrement(entity);

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
					return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
					{
						ErrorCode = ErrorCode.InsertFailed,
						DevMsg = ErrorResource.DevMsg_InsertFailed,
						UserMsg = ErrorResource.UserMsg_InsertFailed,
						TraceId = HttpContext.TraceIdentifier,
						MoreInfo = result.Data,
					});
				}
			}
			catch (Exception ex)
			{
				string traceId = HttpContext.TraceIdentifier;
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{
					sws.WriteLine(traceId);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.Exception,
					DevMsg = ErrorResource.DevMsg_Exception,
					UserMsg = ErrorResource.UserMsg_Exception,
					TraceId = traceId,
					MoreInfo = ErrorResource.Exception_MoreInfo,
				});
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
				var result = _assetIncrementBL.GetNewCode();
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK, result.Data);
				}

				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.GenerateNewCodefailed,
					DevMsg = ErrorResource.DevMsg_GetNewCodeFailed,
					UserMsg = ErrorResource.UserMsg_GetNewCodeFailed,
					MoreInfo = result.Data,
					TraceId = HttpContext.TraceIdentifier
				});
			}
			catch (Exception ex)
			{
				string traceId = HttpContext.TraceIdentifier;
				using (StreamWriter sws = new(ErrorResult.LogError, true))
				{

					sws.WriteLine(traceId);
					sws.WriteLine(ex.Message);
					sws.WriteLine(ex.StackTrace);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.Exception,
					DevMsg = ErrorResource.DevMsg_Exception,
					UserMsg = ErrorResource.UserMsg_Exception,
					TraceId = traceId,
					MoreInfo = ErrorResource.Exception_MoreInfo,
				});
			}

		} 
		#endregion
	}
}
