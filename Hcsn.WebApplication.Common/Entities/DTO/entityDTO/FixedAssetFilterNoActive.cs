using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO.entityDTO
{
    public class FixedAssetFilterNoActive
    {
        public List<Guid>? AssetsNotIn { get; set; }

        public List<Guid>? AssetsActive { get; set; }

    }
}
