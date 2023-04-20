using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Hcsn.WebApplication.Common.Resource;
using Hcsn.WebApplication.DL.AssetIncrementDL;
using Hcsn.WebApplication.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetIncrementBL
{
	public class AssetIncrementBL : BaseBL<FixedAssetIncrement>, IAssetIncrementBL
	{
		#region Field
        private IAssetIncrementDL _assetIncrementDL;
		#endregion
		public AssetIncrementBL(IAssetIncrementDL assetIncrementDL) : base(assetIncrementDL)
		{
			_assetIncrementDL = assetIncrementDL;
		}

		public ServiceResult InsertAssetIncrement(AssetIncrementInsertDTO entity)
		{
			var assetIncrement = entity.fixedAssetIncrement;
			var assets = entity.assets;
            var validateResult = ValidateRequesData(assetIncrement);
			if(assets.Count == 0 || assets == null)
			{
				inValidList.Add(new ValidateResult
				{
					IsSuccess = false,
					ValidateCode = ValidateCode.NoAssetIncrements,
					Message = ValidateResource.NoAssetIncrements,
				});
				validateResult.IsSuccess = false;
				validateResult.Data = inValidList;
			}

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

			int numberOfAffectedRows = _assetIncrementDL.InsertAssetIncrement(entity);
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
					ErrorCode = ErrorCode.InsertFailed,
					Message = ServiceResource.InsertFailed,
				};
			}


		}

		protected override ValidateResult ValidateRequesData(FixedAssetIncrement record)
		{
			return base.ValidateRequesData(record);
		}

	}
}
