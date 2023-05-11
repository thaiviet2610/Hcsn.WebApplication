using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class FixedAssetIncrementDTO : FixedAssetIncrement
	{
		/// <summary>
		/// Số thứ tự
		/// </summary>
        public int index { get; set; }

		/// <summary>
		/// Danh sách tài sản của chứng từ
		/// </summary>
		public List<FixedAssetDTO> assets { get; set; }
    }
}
