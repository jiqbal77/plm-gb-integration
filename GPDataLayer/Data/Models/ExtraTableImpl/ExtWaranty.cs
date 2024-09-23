using GPDataLayer.Data.Models.Factory;
using GPDataLayer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.ExtraTableImpl
{
    class ExtWaranty : IExtenderTable
    {
        private List<dynamic> list = new List<dynamic>();
        public List<dynamic> GetExtraWindowList(ItemDAO itemDAO)
        {
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vWindowID", map = "ITEMWTYATTIBRU" });
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vKey1", map = itemDAO.name });
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue1", map = itemDAO.warranty_name });// warranty name
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue2", map = itemDAO.terms });////term
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue3", map = itemDAO.parts });//parts
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue4", map = itemDAO.labor });//labour
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue5", map = itemDAO.onsite });//onsite
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue6", map = itemDAO.coverage });//coverage
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue7", map = itemDAO.labor_vendor });//labour vendor
            list.Add(new { windowId = "ITEMWTYATTIBRU", key = "@I_vFieldValue8", map = itemDAO.parts_vendor });//PartVendor
           
            return list;
        }
    }
}
