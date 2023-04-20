using Hcsn.WebApplication.BL;
using Hcsn.WebApplication.BL.AssetIncrementBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hcsn.WebApplication.API.Controllers
{

	public class AssetIncrementsController : BasesController<FixedAssetIncrement>
	{
		#region Field
		private IAssetIncrementBL _assetIncrementBL;
		#endregion

		public AssetIncrementsController(IAssetIncrementBL assetIncrementBL) : base(assetIncrementBL)
		{
			_assetIncrementBL = assetIncrementBL;
		}

		[HttpPost("Insert")]
		public IActionResult Insert([FromBody]AssetIncrementInsertDTO entity)
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
	}
}
