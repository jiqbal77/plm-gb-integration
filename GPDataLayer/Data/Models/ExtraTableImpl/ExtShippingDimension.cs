using GPDataLayer.Data.Models.Factory;
using GPDataLayer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.ExtraTableImpl
{
    class ExtShippingDimension : IExtenderTable
    {
        private List<dynamic> list = new List<dynamic>();
        public List<dynamic> GetExtraWindowList(ItemDAO itemDAO)
        {

            list.Add(new { windowId = "ITEMSHIPDIMS", key = "@I_vWindowID", map = "ITEMSHIPDIMS" });
            list.Add(new { windowId = "ITEMSHIPDIMS", key = "@I_vKey1", map = itemDAO.name });
            list.Add(new { windowId = "ITEMSHIPDIMS", key = "@I_vFieldValue1", map = itemDAO.shipping_length_in_inches });//length
            list.Add(new { windowId = "ITEMSHIPDIMS", key = "@I_vFieldValue2", map = itemDAO.shipping_width_in_inches });//width
            list.Add(new { windowId = "ITEMSHIPDIMS", key = "@I_vFieldValue3", map = itemDAO.shipping_height_in_inches });//height
        
            return list;
        }
    }
}
