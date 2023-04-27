using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class FixedAssetIncrementDTO : FixedAssetIncrement
	{
        public int index { get; set; }

		public List<FixedAssetDTO> assets { get; set; }
    }
}
