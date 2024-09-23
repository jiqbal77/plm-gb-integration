using GPDataLayer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.Factory
{
    interface IExtenderTable
    {
        List<dynamic> GetExtraWindowList(ItemDAO itemDAO);
    }
}
