using GPDataLayer.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Messages
{
    public static class Message
    {
        private static Dictionary<MessageType, string> MessageList = new Dictionary<MessageType, string>() 
        {
            [MessageType.BOM_DElETE_SUCCESS]= "Bom Deleted successfully.",
            [MessageType.BOM_DElETE_FAILURE]= "Bom Deletion failed.",
            [MessageType.BOM_FETCH_SUCCESS]= "Fetched Previous Bom of item successfully.",
            [MessageType.BOM_UPSERT_SUCCESS]= "Bom upsert successfull.",
            [MessageType.BOM_UPSERT_FAILURE]= "Bom upsert failed.",
            [MessageType.UPSERT_REMAINING_ATTR_SUCCESS] = "Remaining Attributes upsert successfull.",
            [MessageType.UPSERT_REMAINING_ATTR_FAILURE] = "Remaining Attributes upsert failed.",
            [MessageType.UPSERT_EXTRA_ATTR_FAILURE] = "Extra Attributes of Item Upsert Successfull.",
            [MessageType.UPSERT_EXTRA_ATTR_SUCCESS] = "Extra Attributes of Item Upsert failed.",
            [MessageType.UPSERT_EXTRA_INVENTORY_FAILURE] = "Extra Attributes of Inventory UI Upsert failed.",
            [MessageType.UPSERT_EXTRA_INVENTORY2_FAILURE] = "Extra Attributes of Item Inventory2 UI failed.",
            [MessageType.UPSERT_EXTRA_SHIPPING_FAILURE] = "Extra Attributes of Item Shipping UI failed.",
            [MessageType.UPSERT_EXTRA_WARANTY_FAILURE] = "Extra Attributes of Item Waranty UI failed.",
            [MessageType.CPN_GENERATE_SUCCESS] = "CPN_I string for deletion of bom generated successfully.",
            [MessageType.CPN_GENERATE_FAILURE] = "CPN_I string for deletion of bom failed.",
            [MessageType.MODIFY_WARANTY_DAY_SUCCESS] = "Warranty Days for Item successful.",
            [MessageType.MODIFY_WARANTY_DAY_FAILURE] = "Warranty Days for Item failed.",
        };

        public static string GetMessage(MessageType key)
        {
            if (MessageList.ContainsKey(key))
            {
                return MessageList[key] ;
            }
            return "";
        }


    }
}
