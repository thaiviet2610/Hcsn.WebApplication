using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.entityDTO
{
    public class FixedAssetFilterNotIn
    {
        public List<Guid>? NotInAssets { get; set; }

        public List<Guid>? ActiveAssets { get; set; }

    }
}
