using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetIncrementBL
{
    public interface IAssetIncrementBL : IBaseBL<FixedAssetIncrement>
    {
		public ServiceResult InsertAssetIncrement(AssetIncrementInsertDTO entity);

	}
}
