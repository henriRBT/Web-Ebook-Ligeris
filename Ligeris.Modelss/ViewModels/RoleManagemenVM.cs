using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.Modelss.ViewModels
{
    public class RoleManagemenVM
    {
        public AplikasiUser AplikasiUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; } 
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}
