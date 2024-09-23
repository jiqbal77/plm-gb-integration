using GPDataLayer.Data.Models.Factory;
using GPDataLayer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.ExtraTableImpl
{
    public class ExtInventory : IExtenderTable
    {
        private List<dynamic> list = new List<dynamic>();
        public List<dynamic> GetExtraWindowList(ItemDAO itemDAO)
        {

            list.Add(new { windowId = "INVENTORY", key = "@I_vWindowID", map = "INVENTORY" });
            list.Add(new { windowId = "INVENTORY", key = "@I_vKey1", map = itemDAO.name });
         //   list.Add(new { windowId = "Inventory", key = "@I_vFieldValue1", map = itemDAO.name });// Item ID
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue2", map = itemDAO.customer_gp_id });//customer GP ID
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue3", map = itemDAO.part_category });//Part Category (Update Category)
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue4", map = itemDAO.eccn }); //ECCN
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue5", map = itemDAO.schb });// SCHB
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue6", map = itemDAO.liscence_exception_symbol });
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue7", map = itemDAO.ccats });//ccats
            // list.Add(new {windowId="Inventory",key= "@I_vFieldValue8", map=itemDAO.name});//DSL
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue9", map = itemDAO.customer_group });//Customer Group ID
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue10", map = itemDAO.customer_platform });//Customer Platform
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue11", map = itemDAO.pafr });// Pafr
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue12", map = itemDAO.main_region });// Main Region
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue13", map = itemDAO.wty_in_months });// wty in months
            list.Add(new { windowId = "INVENTORY", key = "@I_vFieldValue14", map = itemDAO.contract_type });//conntract type
            //list.Add(new {windowId="Inventory",key= "@I_vFieldValue15", map=itemDAO.offset_date_attr});
            return list;
        }
    }
}
