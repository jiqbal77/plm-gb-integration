using GPDataLayer.Data.Enums;
using GPDataLayer.Data.Models.ExtraTableImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPDataLayer.Data.Models.Factory
{
    public class ExtenderFactory
    {
        public dynamic getExtraListObject(string type)
        {
           
            if (type.Equals("Inventory"))
            {
                return new ExtInventory();

            }
            else if (type.Equals("Inventory2"))
            {
                return new ExtInventory2();

            }
            else if (type.Equals("ITEMSHIPDIMS"))
            {
                return new ExtShippingDimension();

            }
            else if (type.Equals("ITEMWTYATTIBRU"))
            {
                return new ExtWaranty();

            }
            return null;
        }
        public dynamic GetExtraListObject(ExtrasType type)
        {

            if (type==ExtrasType.INVENTORY)
            {
                return new ExtInventory();

            }
            else if (type == ExtrasType.INVENTORY2)
            {
                return new ExtInventory2();

            }
            else if (type == ExtrasType.ITEMSHIPDIMS)
            {
                return new ExtShippingDimension();

            }
            else if (type == ExtrasType.ITEMWTYATTIBRU)
            {
                return new ExtWaranty();

            }
            return null;
        }

    }
}
