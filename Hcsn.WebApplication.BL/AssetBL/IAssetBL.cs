using Hcsn.WebApplication.BL.BaseBL;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.BL.AssetBL
{
    public interface IAssetBL : IBaseBL<Asset>
    {
        ServiceResult GetPaging(string? keyword, Guid? departmentId, Guid? fixedAssetCatagortId, int pageSize, int pageNumber);
    }
}
