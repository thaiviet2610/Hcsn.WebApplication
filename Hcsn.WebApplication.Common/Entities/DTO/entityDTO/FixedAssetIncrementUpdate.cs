using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.entityDTO
{
    public class FixedAssetIncrementUpdate
    {
        public FixedAssetIncrementDTO AssetIncrement { get; set; }
        public List<Guid> AssetsAdd { get; set; }
        public List<Guid> AssetsDelete { get; set; }
    }
}
