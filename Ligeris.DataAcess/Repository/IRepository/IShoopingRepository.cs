using Ligeris.Modelss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.DataAcess.Repository.IRepository
{
	public interface IShoopingRepository :IRepository<ShoopingCard>
	{
		void Update(ShoopingCard obj);
	}
}
