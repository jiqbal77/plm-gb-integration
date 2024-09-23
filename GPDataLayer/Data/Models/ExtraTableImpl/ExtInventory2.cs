using GPDataLayer.Data.Models.Factory;
using GPDataLayer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.ExtraTableImpl
{
    class ExtInventory2 : IExtenderTable
    {
        private List<dynamic> list = new List<dynamic>();
        public List<dynamic> GetExtraWindowList(ItemDAO itemDAO)
        {
            list.Add(new { windowId = "INVENTORY2", key = "@I_vWindowID", map = "INVENTORY2" });
            list.Add(new { windowId = "INVENTORY2", key = "@I_vKey1", map = itemDAO.name });
            list.Add(new { windowId = "INVENTORY2", key = "@I_vFieldValue9", map = itemDAO.eccn_eu });// eccn_eu
            list.Add(new { windowId = "INVENTORY2", key = "@I_vFieldValue10", map = itemDAO.schb_eu });//schb eu
            list.Add(new { windowId = "INVENTORY2", key = "@I_vFieldValue3", map = itemDAO.regmodel });//region model

            return list;
        }
    }
    
}
