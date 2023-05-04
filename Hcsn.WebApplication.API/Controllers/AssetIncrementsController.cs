using Hcsn.WebApplication.BL.AssetIncrementBL;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class AssetIncrementsController : ControllerBase
	{
		#region Field
		private IAssetIncrementBL _assetIncrementBL;
		#endregion

		#region Constructor
		public AssetIncrementsController(IAssetIncrementBL assetIncrementBL) 
		{
			_assetIncrementBL = assetIncrementBL;
		}
		#endregion

		#region Method
		/// <summary>
		/// API Lấy thông tin chi tiết 1 chứng từ theo id
		/// </summary>
		/// <param name="assetIncrementId">Id chứng từ muốn lấy</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
		[HttpGet("{assetIncrementId}")]
		public IActionResult GetById([FromRoute] Guid assetIncrementId)
		{
			try
			{
				var result = _assetIncrementBL.GetById(assetIncrementId);
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
		/// API phân trang, lọc danh sách tài sản chứng từ
		/// </summary>
		/// <param name="keyword">Từ khóa tìm kiếm</param>
		/// <param name="pageSize">Số bản ghi trong 1 trang</param>
		/// <param name="pageNumber">Vị trí trang hiện tại</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTVIET (20/04/2023)
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

		/// <summary>
		/// Hàm thêm mới 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementDTO">Bản ghi chứng từ muốn thêm</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		[HttpPost]
		public IActionResult InsertAssetIncrement([FromBody] FixedAssetIncrementDTO assetIncrementDTO)
		{
			try
			{
				var result = _assetIncrementBL.InsertAssetIncrement(assetIncrementDTO);

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
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
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

		/// <summary>
		/// Hàm sửa đổi 1 bản ghi chứng từ
		/// </summary>
		/// <param name="assetIncrementUpdate">Đối tượng chứng từ chứa thông tin để update</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		[HttpPut]
		public IActionResult UpdateAssetIncrement([FromBody] FixedAssetIncrementUpdate assetIncrementUpdate)
		{
			try
			{
				var assetIncrement = assetIncrementUpdate.AssetIncrement;
				var assetsAdd = assetIncrementUpdate.AssetsAdd;
				var assetsDelete = assetIncrementUpdate.AssetsDelete;
				var result = _assetIncrementBL.UpdateAssetIncrement(assetIncrement, assetsAdd,assetsDelete);
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
					return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
					{
						ErrorCode = ErrorCode.UpdateFailed,
						DevMsg = ErrorResource.DevMsg_UpdateFailed,
						UserMsg = ErrorResource.UserMsg_UpdateFailed,
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
		/// Hàm sửa đổi tổng nguyên giá của bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id của chứng từ cần sửa</param>
		/// <param name="price">Giá trị của tổng nguyên giá</param>
		/// <returns>
		/// StatusCode == 200: thành công
		/// StatusCode != 200: thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		[HttpPut("Price")]
		public IActionResult UpdateAssetIncrementPrice([FromQuery] Guid voucherId, [FromQuery] Decimal price )
		{
			try
			{
				var result = _assetIncrementBL.UpdateAssetIncrementPrice(voucherId, price);
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
					return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
					{
						ErrorCode = ErrorCode.UpdateFailed,
						DevMsg = ErrorResource.DevMsg_UpdateFailed,
						UserMsg = ErrorResource.UserMsg_UpdateFailed,
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
		/// API xóa 1 bản ghi chứng từ
		/// </summary>
		/// <param name="voucherId">Id bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// StatusCode == 200: Nếu delete thành công
		/// StatusCode != 200: Nếu delete thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		[HttpDelete("{voucherId}")]
		public ActionResult DeleteRecordById([FromRoute] Guid voucherId)
		{
			try
			{
				var result = _assetIncrementBL.DeleteAssetIncrementById(voucherId);
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.DeleteFailed,
					DevMsg = ErrorResource.DevMsg_DeleteFailed,
					UserMsg = ErrorResource.UserMsg_DeleteFailed,
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
		/// API xóa nhiều bản ghi chứng từ
		/// </summary>
		/// <param name="ids">Danh sách id các bản ghi chứng từ muốn xóa</param>
		/// <returns>
		/// StatusCode == 200: Nếu delete thành công
		/// StatusCode != 200: Nếu delete thất bại
		/// </returns>
		/// Created by: LTViet (20/04/2023)
		[HttpDelete]
		public ActionResult DeleteMultipleRecord([FromBody]List<Guid> ids)
		{
			try
			{
				var result = _assetIncrementBL.DeleteMultipleAssetIncrement(ids);
				if (result.IsSuccess)
				{
					return StatusCode(StatusCodes.Status200OK, result.Data);
				}
				return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
				{
					ErrorCode = ErrorCode.DeleteMultipleFailed,
					DevMsg = ErrorResource.DevMsg_DeleteMultipleFailed,
					UserMsg = ErrorResource.UserMsg_DeleteMultipleFailed,
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
		#endregion
	}
}
