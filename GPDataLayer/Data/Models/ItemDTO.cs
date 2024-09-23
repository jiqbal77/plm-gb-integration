using GPDataLayer.Data.Enums;
using GPDataLayer.Data.Messages;
using GPDataLayer.Data.Models.Factory;
using GPDataLayer.Data.ViewModels;
using GPUtils;
using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GPDataLayer.Data.Models
{
    public class ItemDTO
    {
        #region private members
        private eConnectType EConnect;

        private IVItemMasterType ItemType;

        private taUpdateCreateItemRcd Item;

        private taItemSite_ItemsTaItemSite[] Item2;

        private taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine Item4;

        private taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine Item5;

        private AppLogger _logger = AppLogger.GetInstance();

        private string DatabaseConnString;

        private string DatabaseConnStringMiddleware;
        #endregion

        #region public methods
        public ItemDTO()
        {
            DatabaseConnString = ConfigurationManager.AppSettings["GPDatabaseConnectionString"].ToString();
            DatabaseConnStringMiddleware = ConfigurationManager.AppSettings["GPMiddlewareDbConnectionString"].ToString();
        }

        public GenericResponse<dynamic> CreateItems(ItemDAO itemDAO)
        {
            try
            {
                _logger.Debug("GPDataLayer.ItemDTO.createTransaction", itemDAO.name);
                //var modifyWarantyDaysResponse = ModifyWarrantyDaysOfItem(itemDAO);// treatment for -1 warranty days
                var createItemMasterResponse = CreateItemInIVMaster(itemDAO);
                if (createItemMasterResponse.success)
                {
                    var createExtraItemResponse = InsertInExtraWindows(itemDAO);
                    if (!createExtraItemResponse.success)
                    {
                        return new GenericResponse<dynamic>("", false, createExtraItemResponse.message);
                    }
                    var createRemainingItemRespose = CreateUpdateItemRemainingAttribute(itemDAO);
                    if (!createRemainingItemRespose.success)
                    {
                        return new GenericResponse<dynamic>("", false, "Failure while inserting Remaining Attributes for Item:" + itemDAO.name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GPDataLayer.ItemDTO@createTransaction Exception Occured:" + ex.ToString(), itemDAO.name);
                return new GenericResponse<dynamic>("", false, "Insertion failed" + "GPDataLayer.ItemDTO@createTransaction Exception Occured: " + ex.ToString());
            }
            return new GenericResponse<dynamic>("", true, "Inserted successfully");
        }
        public GenericResponse<dynamic> CreateTransaction(ItemDAO itemDAO)
        {
            try
            {
                _logger.Debug("GPDataLayer.ItemDTO.createTransaction", itemDAO.name);
                //var modifyWarantyDaysResponse = ModifyWarrantyDaysOfItem(itemDAO);// treatment for -1 warranty days
                // var createItemMasterResponse = CreateItemInIVMaster(itemDAO);
                //if (createItemMasterResponse.success)
                //{
                /*var createExtraItemResponse = InsertInExtraWindows(itemDAO);
                if (!createExtraItemResponse.success)
                {
                    return new GenericResponse<dynamic>("", false, createExtraItemResponse.message);
                }
                var createRemainingItemRespose = CreateUpdateItemRemainingAttribute(itemDAO);
                if (!createRemainingItemRespose.success)
                {
                    return new GenericResponse<dynamic>("", false, "Failure while inserting Remaining Attributes for Item:" + itemDAO.name);
                }*/
                if (itemDAO.lifeCycle_Phase!= "Obsolete")
                {
                    dynamic previousBomItems = GetPreviousBomOfItem(itemDAO.name);

                    if (previousBomItems.success && previousBomItems.data.Count > 0)
                    {

                        List<dynamic> list = new List<dynamic>(previousBomItems.data);
                        List<Bom> bomFilteredList = new List<Bom>();
                        List<Bom> parentList = new List<Bom>(itemDAO.bom);


                        bool flag = false;
                        int i = 0;

                        while (parentList.Count > 0)
                        {
                            flag = false;
                            foreach (var obj in previousBomItems.data)
                            {
                                if (parentList[i].item_name.Equals(obj.cpn_i.Trim()))
                                {
                                    parentList[i].effectiveoutdate_i = obj.effective_in_date;
                                    bomFilteredList.Add(parentList[i]);
                                    parentList.Remove(parentList[i]);
                                    previousBomItems.data.Remove(obj);
                                    flag = true;
                                    break;
                                }
                            }

                            if (flag == false)
                            {
                                bomFilteredList.Add(parentList[i]);
                                parentList.Remove(parentList[i]);
                            }

                        }
                        if (previousBomItems.data.Count > 0)
                        {
                            var deleteBomCpnGenerate = GenerateDeleteBomCpn(previousBomItems.data);
                            if (deleteBomCpnGenerate.success)
                            {
                                var deletBomSpResponse = DeleteBomItems(itemDAO.name, deleteBomCpnGenerate.data);
                                if (!deletBomSpResponse.success)
                                {
                                    return new GenericResponse<dynamic>("", false, "Failure while deleting Previous Bom of Item:" + itemDAO.name + "CPN_I List:" + deleteBomCpnGenerate.data);
                                }
                            }
                        }
                        var insertUpdateBomListResponse = CreateUpdateBomFromList(itemDAO.name, itemDAO.rev_level, bomFilteredList);
                        if (!insertUpdateBomListResponse.success)
                        {
                            return new GenericResponse<dynamic>("", false, insertUpdateBomListResponse.message);
                        }
                    }
                    else
                    {
                        if (itemDAO.bom.Count > 0)
                        {

                            var insertUpdateBomListResponse = CreateUpdateBomFromList(itemDAO.name, itemDAO.rev_level, itemDAO.bom);
                            if (!insertUpdateBomListResponse.success)
                            {
                                return new GenericResponse<dynamic>("", false, insertUpdateBomListResponse.message);
                            }

                            /*   foreach (Bom bom in itemDAO.bom)
                               {
                                   var bomResponse = CreateUpdateItemBom(itemDAO.name, bom);
                                   if (!bomResponse.success)
                                   {
                                       return new GenericResponse<dynamic>("", false, "Failure while inserting Bom:" + bom.cpn + "for Item:" + itemDAO.name);
                                   }
                               }*/
                        }
                    }
                }

                //}
                /*else
                {
                    return new GenericResponse<dynamic>("", false, createItemMasterResponse.message);
                }*/
            }
            catch (Exception ex)
            {
                _logger.Error("GPDataLayer.ItemDTO@createTransaction Exception Occured:" + ex.ToString(), itemDAO.name);
                return new GenericResponse<dynamic>("", false, "Insertion failed" + "GPDataLayer.ItemDTO@createTransaction Exception Occured: " + ex.ToString());
            }
            return new GenericResponse<dynamic>("", true, "Inserted successfully");
        }
        #endregion

        #region private methods
        private GenericResponse<string> CreateUpdateBomFromList(string itemNumber, string rev, List<Bom> boms)
        {
            try
            {
                foreach (Bom bom in boms)
                {
                    var bomResponse = CreateUpdateItemBom(itemNumber, rev, bom);
                    if (!bomResponse.success)
                    {
                        return new GenericResponse<string>("", false, "Failure while inserting Bom:" + bom.cpn + "for Item:" + itemNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GPDataLayer.ItemDTO@createUpdateBomFromList Exception Occured:" + ex.ToString(), itemNumber);
                return new GenericResponse<string>("", false, "Failure while processing Boms: for Item:" + itemNumber);
            }
            return new GenericResponse<string>("", true, Message.GetMessage(MessageType.BOM_UPSERT_SUCCESS));
        }

        private GenericResponse<string> GenerateDeleteBomCpn(List<dynamic> list)
        {
            string cpn_i = ""; bool status = false;
            try
            {
                if (list.Count > 0)
                {
                    foreach (var bom in list)
                    {
                        cpn_i = bom.cpn_i.Trim() + "," + cpn_i;
                    }
                    status = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GPDataLayer.ItemDTO@generateDeleteBomCpn Exception Occured:" + ex.ToString());
                return new GenericResponse<string>(ex.ToString(), false, "Failure while generating Delete Bom Cpn list for Item:");
            }
            return new GenericResponse<string>(cpn_i, status, status ? Message.GetMessage(MessageType.CPN_GENERATE_SUCCESS) : Message.GetMessage(MessageType.CPN_GENERATE_FAILURE)); ;
        }

        private GenericResponse<dynamic> CreateItemInIVMaster(ItemDAO itemDAO)
        {

            string msg = "";
            bool res = false;
            string sConnectionString = "";
            string sItemDocument = "";
            #region econnect
            using (eConnectMethods e = new eConnectMethods())
            {
                try
                {
                    _logger.Debug("GPDataLayer.ItemDTO@createTransaction", itemDAO.name);
                    // Create the ItemMaster data file
                    SerializeItemObject("Item_" + itemDAO.name + ".xml", itemDAO);

                    // Use an XML document to create a string representation of the customer
                    XmlDocument xmldoc = new XmlDocument();

                    xmldoc.Load(ConfigurationManager.AppSettings["xmlDirectory"].ToString() + "Item_" + itemDAO.name + ".xml");

                    // Specify the Microsoft Dynamics GP server and database in the connection string
                    sConnectionString = ConfigurationManager.AppSettings["GPeConnectConnectionString"].ToString();

                    _logger.Debug("Connection String:" + sConnectionString);
                    // Create an XML Document object for the schema

                    sItemDocument = xmldoc.OuterXml;
                    _logger.Debug("Connection String:" + sConnectionString);

                    _logger.Debug("XML Schema:" + sItemDocument);

                    _logger.Debug("Before Transaction");
                    // Pass in xsdSchema to validate against.

                    bool status = e.CreateEntity(sConnectionString, sItemDocument);
                    _logger.Debug("After Transaction :" + itemDAO.name);
                    res = status;
                    msg = "Record Inserted Successfully";
                }
                // The eConnectException class will catch eConnect business logic errors. display the error message on the console
                catch (eConnectException exc)
                {
                    _logger.Error("GPDataLayer.ItemDTO@createTransaction Econnect exception:" + exc.ToString(), itemDAO.name);
                    res = false;
                    msg = exc.ToString();
                }
                // Catch any system error that might occurr.
                catch (Exception ex)
                {
                    _logger.Error("GPDataLayer.ItemDTO@createTransaction exception:" + ex.ToString(), itemDAO.name);
                    res = false;
                    msg = ex.ToString();
                }
                finally
                {
                    // Call the Dispose method to release the resources
                    // of the eConnectMethds object
                    _logger.Debug("GPDataLayer.ItemDTO@createTransaction Closing Connection", itemDAO.name);
                    e.Dispose();
                }
            }
            #endregion

            return new GenericResponse<dynamic>("No Data", res, msg);
        }

        private void SerializeItemObject(string filename, ItemDAO itemDAO)
        {
            try
            {
                // Instantiate an eConnectType schema object
                EConnect = new eConnectType();

                // Instantiate a RMCustomerMasterType schema object
                ItemType = new IVItemMasterType();


                // Instantiate a taUpdateCreateCustomerRcd XML node object
                Item = new taUpdateCreateItemRcd();
                Item2 = new taItemSite_ItemsTaItemSite[12];
                Item4 = new taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine();
                Item5 = new taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine();
                taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine[] Items = new taIVCreateItemPriceListLine_ItemsTaIVCreateItemPriceListLine[2];

                // Create an XML serializer object
                XmlSerializer serializer = new XmlSerializer(EConnect.GetType());

                // Populate elements of the taUpdateCreateCustomerRcd XML node object

                List<string> locations = new List<string>();
                locations.Add("CA-WHS");
                locations.Add("PL-WHS");
                locations.Add("GL-WHS");
                locations.Add("PW-WHS");
                locations.Add("CA-CON-BH");
                locations.Add("PL-CON-BH");
                locations.Add("GL-CON-BH");
                locations.Add("PW-CON-BH");
                locations.Add("CA-CON-NEW");
                locations.Add("PL-CON-NEW");
                locations.Add("GL-CON-NEW");
                locations.Add("PW-CON-NEW");

                for (int j = 0; j < 12; j++)
                {
                    taItemSite_ItemsTaItemSite tempItem = new taItemSite_ItemsTaItemSite();
                    tempItem.ITEMNMBR = itemDAO.name;
                    tempItem.LOCNCODE = locations[j];
                    Item2[j] = tempItem;
                }


                Item4.ITEMNMBR = itemDAO.name;
                Item4.CURNCYID = "Z-US$";
                Item4.PRCLEVEL = "ZEROP";
                Item4.UOFM = "EA";



                Item5.ITEMNMBR = itemDAO.name;
                Item5.CURNCYID = "Z-US$";
                Item5.PRCLEVEL = "LIST";
                Item5.UOFM = "EA";

                Item.ITEMNMBR = itemDAO.name;
                Item.ITEMDESC = itemDAO.description;
                Item.ITMSHNAM = itemDAO.short_description;
                Item.ITMGEDSC = itemDAO.description;
                Item.ITMCLSCD = itemDAO.item_class_code;
                Item.ITEMTYPE = 3;//KIT
                Item.UOMSCHDL = "EACH";
                Item.ABCCODE = 2;
                Item.ITEMSHWT = itemDAO.shipping_weight_in_lbs.Equals("") ? 0 : Decimal.Parse(itemDAO.shipping_weight_in_lbs);
                //Item.NOTETEXT = itemDAO.notes_comments;
                Item.STNDCOST = itemDAO.initial_cost.Equals("") ? 0 : Decimal.Parse(itemDAO.initial_cost);
                //Item.LOCNCODE = itemDAO.locn_code;
                Item.UpdateIfExists = 1;
                //  Item.WRNTYDYS = (short)(itemDAO.warranty_days.Equals("") ? 0 : Convert.ToInt16(itemDAO.warranty_days));//passed -1 in GP

                Item.VCTNMTHD = 1;//fifo
                //Item.USCATVLS_4 = itemDAO.uplifted_support;//uplifted support
                Item.UseItemClass = 1; //Flag to have class setting roll down to elements that are not passed in; uses the ITMCLSCD class
                                       // Populate the RMCustomerMasterType schema with the taUpdateCreateCustomerRcd XML node

               // Items[0] = Item4;
               // Items[1] = Item5;

                ItemType.taUpdateCreateItemRcd = Item;
                //ItemType.taIVCreateItemPriceListLine_Items = Items;
                ItemType.taItemSite_Items = Item2;



                IVItemMasterType[] myIVItemMaster = { ItemType };

                // Populate the eConnectType object with the RMCustomerMasterType schema object
                EConnect.IVItemMasterType = myIVItemMaster;

                // Create objects to create file and write the customer XML to the file
                FileStream fs = new FileStream(ConfigurationManager.AppSettings["xmlDirectory"].ToString() + filename, FileMode.Create);
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());

                // Serialize the eConnectType object to a file using the XmlTextWriter.
                serializer.Serialize(writer, EConnect);
                writer.Close();
            }
            // catch any errors that occur and display them to the console
            catch (System.Exception ex)
            {
                _logger.Error("GPDataLayer.ItemDTO@serializeItemObject exception occured:" + ex.ToString(), itemDAO.name);

            }
        }


        private GenericResponse<string> InsertInExtraWindows(ItemDAO itemDAO)
        {
            _logger.Debug("GPDataLayer.ItemDTO@insertInExtraWindows", itemDAO.name);

            IExtenderTable extenderTableInventory = new ExtenderFactory().GetExtraListObject(ExtrasType.INVENTORY);
            List<dynamic> listExtInventory = extenderTableInventory.GetExtraWindowList(itemDAO);
            var responseInventory = CreateExtenderWindowData(itemDAO, listExtInventory);
            if (!responseInventory.success)
            {
                return new GenericResponse<string>("", false, responseInventory.message);
            }
            IExtenderTable extenderTableInventory2 = new ExtenderFactory().GetExtraListObject(ExtrasType.INVENTORY2);
            List<dynamic> listExtInventory2 = extenderTableInventory2.GetExtraWindowList(itemDAO);
            var responseInventory2 = CreateExtenderWindowData(itemDAO, listExtInventory2);
            if (!responseInventory2.success)
            {
                return new GenericResponse<string>("", false, responseInventory2.message);
            }
            IExtenderTable extenderTableITEMSHIPDIMS = new ExtenderFactory().GetExtraListObject(ExtrasType.ITEMSHIPDIMS);
            List<dynamic> listExtShippingDimension = extenderTableITEMSHIPDIMS.GetExtraWindowList(itemDAO);
            var responseInventory3 = CreateExtenderWindowData(itemDAO, listExtShippingDimension);
            if (!responseInventory3.success)
            {
                return new GenericResponse<string>("", false, responseInventory3.message);
            }
            IExtenderTable extenderTableITEMWTYATTIBRU = new ExtenderFactory().GetExtraListObject(ExtrasType.ITEMWTYATTIBRU);
            List<dynamic> listExtWarranty = extenderTableITEMWTYATTIBRU.GetExtraWindowList(itemDAO);
            var responseInventory4 = CreateExtenderWindowData(itemDAO, listExtWarranty);
            if (!responseInventory4.success)
            {
                return new GenericResponse<string>("No Data", false, responseInventory4.message);
            }
            return new GenericResponse<string>("No Data", responseInventory4.success, responseInventory4.success ? Message.GetMessage(MessageType.UPSERT_EXTRA_ATTR_SUCCESS) :
                Message.GetMessage(MessageType.UPSERT_EXTRA_ATTR_FAILURE));
        }

        private GenericResponse<dynamic> CreateExtenderWindowData(ItemDAO itemDAO, List<dynamic> list)
        {
            _logger.Debug("GPDataLayer.ItemDTO@createExtenderWindowData", itemDAO.name);
            int res = 0;

            using (SqlConnection conn = new SqlConnection(DatabaseConnString))
            {
                try
                {

                    int O_iErrorState = 0;
                    string oErrString = "";

                    conn.Open();

                    SqlCommand sql_cmnd = new SqlCommand("taExtenderWindowAddUpdate", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    foreach (dynamic obj in list)
                    {
                        sql_cmnd.Parameters.AddWithValue(obj.key, SqlDbType.NVarChar).Value = obj.map;

                    }

                    sql_cmnd.Parameters.AddWithValue("@O_iErrorState", SqlDbType.Int).Value = O_iErrorState;
                    sql_cmnd.Parameters.AddWithValue("@oErrString", SqlDbType.NVarChar).Value = oErrString;

                    res = sql_cmnd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error("GPDataLayer.ItemDTO@createExtenderWindowData exception occured: " + ex.ToString(), itemDAO.name);
                    return new GenericResponse<dynamic>(res, false, "GPDataLayer.ItemDTO@createExtenderWindowData Exception occured:" + ex.ToString());
                }
            }
            return new GenericResponse<dynamic>(res, true, Message.GetMessage(MessageType.UPSERT_EXTRA_ATTR_SUCCESS));
        }

        private GenericResponse<string> CreateUpdateItemRemainingAttribute(ItemDAO itemDAO)
        {
            _logger.Debug("GPDataLayer.ItemDTO@createUpdateItemRemainingAttribute", itemDAO.name);
            int res = 0;
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DatabaseConnStringMiddleware))
            {

                try
                {
                    conn.Open();

                    SqlCommand sql_cmnd = new SqlCommand("taUpdateCreateItemRemainingAttributes", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@ITEMNUMBER", SqlDbType.VarChar).Value = itemDAO.name;
                    sql_cmnd.Parameters.AddWithValue("@BuyerId", SqlDbType.VarChar).Value = itemDAO.buyer_Id;
                    sql_cmnd.Parameters.AddWithValue("@PlannerID", SqlDbType.VarChar).Value = itemDAO.planner_code;
                    sql_cmnd.Parameters.AddWithValue("@PRIMVNDR", SqlDbType.VarChar).Value = itemDAO.primary_vendor;
                    sql_cmnd.Parameters.AddWithValue("@MNFCTRNGFXDLDTM", SqlDbType.Int).Value = itemDAO.mfg_fixed_lead_time.Equals("") ? 0 : Int16.Parse(itemDAO.mfg_fixed_lead_time);
                    sql_cmnd.Parameters.AddWithValue("@ORDERPOLICY", SqlDbType.Int).Value = itemDAO.order_policy.Equals("") ? 0 : Int16.Parse(itemDAO.order_policy);
                    sql_cmnd.Parameters.AddWithValue("@NMBROFDYS", SqlDbType.Int).Value = itemDAO.number_of_days_period_qty.Equals("") ? 0 : Int16.Parse(itemDAO.number_of_days_period_qty);
                    sql_cmnd.Parameters.AddWithValue("@MNMMORDRQTY", SqlDbType.Int).Value = itemDAO.min_order_qty.Equals("") ? 0 : Int16.Parse(itemDAO.min_order_qty);
                    sql_cmnd.Parameters.AddWithValue("@MXMMORDRQTY", SqlDbType.Int).Value = itemDAO.max_order_qty.Equals("") ? 0 : Int16.Parse(itemDAO.max_order_qty);
                    sql_cmnd.Parameters.AddWithValue("@MAKEBUYCODE_I", SqlDbType.Int).Value = itemDAO.make_buy_code.Equals("") ? 0 : Int16.Parse(itemDAO.make_buy_code);
                    sql_cmnd.Parameters.AddWithValue("@FLOORSTOCK_I", SqlDbType.Int).Value = itemDAO.floor_stock.Equals("") ? 0 : Int16.Parse(itemDAO.floor_stock);
                    sql_cmnd.Parameters.AddWithValue("@ITEMFULFILLMETHOD", SqlDbType.Int).Value = itemDAO.item_fullfilment_method.Equals("") ? 0 : Int16.Parse(itemDAO.item_fullfilment_method);
                    sql_cmnd.Parameters.AddWithValue("@REPLENISHMENTMETHOD", SqlDbType.Int).Value = itemDAO.replenishment.Equals("") ? 0 : Int16.Parse(itemDAO.replenishment);
                    sql_cmnd.Parameters.AddWithValue("@WRNTYDYS", SqlDbType.Int).Value = itemDAO.warranty_days.Equals("") ? 0 : Int16.Parse(itemDAO.warranty_days);
                    sql_cmnd.Parameters.AddWithValue("@INITIALCOST", SqlDbType.Decimal).Value = itemDAO.initial_cost.Equals("") ? 0 : Decimal.Parse(itemDAO.initial_cost);
                    sql_cmnd.Parameters.AddWithValue("@ROUTINGNAME", SqlDbType.VarChar).Value = itemDAO.default_route;
                    sql_cmnd.Parameters.AddWithValue("@REVISIONLEVEL_I", SqlDbType.VarChar).Value = itemDAO.rev_level.Equals("") ? " " : itemDAO.rev_level;
                    sql_cmnd.Parameters.AddWithValue("@CALCMRP_I", SqlDbType.Int).Value = itemDAO.calculate_mrp.Equals("") ? null : itemDAO.calculate_mrp;
                    sql_cmnd.Parameters.AddWithValue("@LaborTime", SqlDbType.Decimal).Value = itemDAO.labor_time.Equals("") ? 0 : Decimal.Parse(itemDAO.labor_time); 
                    sql_cmnd.Parameters.AddWithValue("@QueueTime", SqlDbType.Decimal).Value = itemDAO.queue_time.Equals("") ? 0 : Decimal.Parse(itemDAO.queue_time);
                    sql_cmnd.Parameters.AddWithValue("@Lifecycle", SqlDbType.VarChar).Value = itemDAO.lifeCycle_Phase;
                    sql_cmnd.Parameters.AddWithValue("@ShipWeight", SqlDbType.Decimal).Value = itemDAO.shipping_weight_in_lbs.Equals("") ? 0 : Decimal.Parse(itemDAO.shipping_weight_in_lbs);
                    res = sql_cmnd.ExecuteNonQuery();
                    conn.Close();
                    status = res > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    _logger.Error($"GPDataLayer.ItemDTO@createUpdateItemRemainingAttribute exception occured: {ex.ToString()}", itemDAO.name);
                    return new GenericResponse<string>("", false, "GPDataLayer.ItemDTO@createUpdateItemRemainingAttribute " +
                        "Exception occured:" + ex.ToString());
                }
            }
            _logger.Debug("GPDataLayer.ItemDTO@createUpdateItemRemainingAttribute Inserted successfully for: " + itemDAO.name, itemDAO.name);
            return new GenericResponse<string>("No data", status, status ? Message.GetMessage(MessageType.UPSERT_REMAINING_ATTR_SUCCESS) : Message.GetMessage(MessageType.UPSERT_REMAINING_ATTR_FAILURE));
        }

        //incase u get expection while updaing item record warranty days with -1
        private GenericResponse<string> ModifyWarrantyDaysOfItem(ItemDAO itemDAO)
        {
            _logger.Debug("GPDataLayer.ItemDTO@ModifyWarrantyDaysOfItem", itemDAO.name);
            int res = 0;
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DatabaseConnStringMiddleware))
            {

                try
                {
                    conn.Open();

                    SqlCommand sql_cmnd = new SqlCommand("modifyWarantyDays", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@ITEMNUMBER", SqlDbType.VarChar).Value = itemDAO.name;
                    res = sql_cmnd.ExecuteNonQuery();
                    conn.Close();
                    status = res > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    _logger.Error($"GPDataLayer.ItemDTO@ModifyWarrantyDaysOfItem exception occured: {ex.ToString()}", itemDAO.name);
                    return new GenericResponse<string>("", false, "GPDataLayer.ItemDTO@ModifyWarrantyDaysOfItem " +
                        "Exception occured:" + ex.ToString());
                }
            }
            _logger.Debug("GPDataLayer.ItemDTO@ModifyWarrantyDaysOfItem Inserted successfully for: " + itemDAO.name, itemDAO.name);
            return new GenericResponse<string>("No data", status, status ? Message.GetMessage(MessageType.MODIFY_WARANTY_DAY_SUCCESS) : Message.GetMessage(MessageType.MODIFY_WARANTY_DAY_FAILURE));
        }

        private GenericResponse<string> CreateUpdateItemBom(string itemNumber, string rev, Bom bom)
        {
            _logger.Debug("GPDataLayer.ItemDTO@createUpdateItemBom", itemNumber);
            int res = 0;
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DatabaseConnStringMiddleware))
            {
                try
                {
                    conn.Open();

                    var effIn = bom.effectiveindate_i.Equals("") ? DateTime.Parse("1900-01-01") : DateTime.Parse(bom.effectiveindate_i);
                    var effOut = DateTime.Parse(bom.effectiveoutdate_i);
                    SqlCommand sql_cmnd = new SqlCommand("createOrUpdateBOM", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@ITEMNUMBER", SqlDbType.VarChar).Value = itemNumber;
                    sql_cmnd.Parameters.AddWithValue("@Rev", SqlDbType.VarChar).Value = rev;
                    sql_cmnd.Parameters.AddWithValue("@BOMCAT_I", SqlDbType.Int).Value = Int16.Parse(bom.bom_cat_i);
                    sql_cmnd.Parameters.AddWithValue("@BOMNAME_I", SqlDbType.VarChar).Value = bom.bom_name;
                    sql_cmnd.Parameters.AddWithValue("@REVISIONLEVEL_I", SqlDbType.VarChar).Value = bom.revesion;
                    sql_cmnd.Parameters.AddWithValue("@EFFECTIVEDATE_I", SqlDbType.DateTime).Value = DateTime.Parse("1900-01-01");
                    sql_cmnd.Parameters.AddWithValue("@BACKFLUSHTIME_I", SqlDbType.Int).Value = bom.backflush_item_i.Equals("") ? 0 : Int16.Parse(bom.backflush_item_i);
                    sql_cmnd.Parameters.AddWithValue("@BOMTYPE_I", SqlDbType.Int).Value = bom.bom_type_i.Equals("") ? 0 : Int16.Parse(bom.bom_type_i);
                    sql_cmnd.Parameters.AddWithValue("@LOCNCODE", SqlDbType.VarChar).Value = bom.locn_code;
                    sql_cmnd.Parameters.AddWithValue("@WCID_I", SqlDbType.VarChar).Value = bom.wcid_i;
                    sql_cmnd.Parameters.AddWithValue("@CPN_I", SqlDbType.VarChar).Value = bom.cpn;
                    sql_cmnd.Parameters.AddWithValue("@BOMSEQ_I", SqlDbType.Int).Value = bom.bom_seq_i.Equals("") ? 1 : Int16.Parse(bom.bom_seq_i);
                    sql_cmnd.Parameters.AddWithValue("@QUANTITY_I", SqlDbType.Int).Value = bom.quantity.Equals("") ? 0 : Int16.Parse(bom.quantity);
                    sql_cmnd.Parameters.AddWithValue("@SUBCAT_I", SqlDbType.Int).Value = bom.sub_cat_i.Equals("") ? 0 : Int16.Parse(bom.sub_cat_i);
                    sql_cmnd.Parameters.AddWithValue("@EFFECTIVEINDATE_I", SqlDbType.DateTime).Value = effIn;
                    sql_cmnd.Parameters.AddWithValue("@EFFECTIVEOUTDATE_I", SqlDbType.DateTime).Value = DateTime.Parse("1900-01-01");
                    sql_cmnd.Parameters.AddWithValue("@UOFM", SqlDbType.VarChar).Value = bom.uofm;
                    res = sql_cmnd.ExecuteNonQuery();
                    conn.Close();

                    status = res > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    _logger.Error("GPDataLayer.ItemDTO@createUpdateItemBom Item NO:" + bom.cpn + " exception occured" + ex.ToString(), itemNumber);
                    return new GenericResponse<string>("", false, "GPDataLayer.ItemDTO@createUpdateItemBom " +
                        "Exception occured:" + ex.ToString());
                }
            }
            _logger.Debug("GPDataLayer.ItemDTO@createUpdateItemBom Bom Insertion for Item Number" + bom.cpn + " successfully for: " + itemNumber, itemNumber);
            return new GenericResponse<string>("No data", status, status ? Message.GetMessage(MessageType.BOM_UPSERT_SUCCESS) : Message.GetMessage(MessageType.BOM_UPSERT_FAILURE));
        }

        private GenericResponse<dynamic> GetPreviousBomOfItem(string itemNumber)
        {
            _logger.Debug("GPDataLayer.ItemDTO@getPreviousBomOfItem", itemNumber);
            int res = 0;
            bool status = false;
            List<dynamic> resultList = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(DatabaseConnStringMiddleware))
            {

                try
                {
                    conn.Open();
                    SqlDataReader rd;
                    SqlCommand sql_cmnd = new SqlCommand("getPreviousBomOfItem", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;
                    sql_cmnd.Parameters.AddWithValue("@ITEMNUMBER", SqlDbType.VarChar).Value = itemNumber;

                    rd = sql_cmnd.ExecuteReader();
                    while (rd.Read())
                    {
                        var obj = new
                        {
                            ppn_i = rd["PPN_I"].ToString(),
                            cpn_i = rd["CPN_I"].ToString(),
                            effective_in_date = rd["EFFECTIVEINDATE_I"].ToString(),
                            effective_out_date = rd["EFFECTIVEOUTDATE_I"].ToString()
                        };
                        resultList.Add(obj);
                    }
                    rd.Close();

                    conn.Close();


                }
                catch (Exception ex)
                {
                    _logger.Error("GPDataLayer.ItemDTO@getPreviousBomOfItem Item NO:" + itemNumber + " exception occured" + ex.ToString(), itemNumber);
                    return new GenericResponse<dynamic>(resultList, false, "GPDataLayer.ItemDTO@createUpdateItemBom " +
                        "Exception occured:" + ex.ToString());
                }
            }
            _logger.Debug("GPDataLayer.ItemDTO@getPreviousBomOfItem Bom Insertion for Item Number" + itemNumber, itemNumber);
            return new GenericResponse<dynamic>(resultList, true, Message.GetMessage(MessageType.BOM_FETCH_SUCCESS));
        }

        private GenericResponse<string> DeleteBomItems(string itemNumber, string cpn_i)
        {
            _logger.Debug("GPDataLayer.ItemDTO@deleteBomItems", itemNumber);
            int res = 0;
            bool status = false;
            using (SqlConnection conn = new SqlConnection(DatabaseConnStringMiddleware))
            {

                try
                {
                    conn.Open();

                    SqlCommand sql_cmnd = new SqlCommand("deleteBOMItems", conn);
                    sql_cmnd.CommandType = CommandType.StoredProcedure;

                    sql_cmnd.Parameters.AddWithValue("@ITEMNUMBER", SqlDbType.VarChar).Value = itemNumber;
                    sql_cmnd.Parameters.AddWithValue("@CPN_I", SqlDbType.NVarChar).Value = cpn_i;
                    res = sql_cmnd.ExecuteNonQuery();
                    conn.Close();
                    status = res > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    _logger.Error("GPDataLayer.ItemDTO@deleteBomItems CPN_I:" + cpn_i + " exception occured" + ex.ToString(), itemNumber);
                    return new GenericResponse<string>("", false, "GPDataLayer.ItemDTO@deleteBomItems " +
                        "Exception occured:" + ex.ToString());
                }
            }
            _logger.Debug("GPDataLayer.ItemDTO@deleteBomItems Bom Insertion for Item Number" + cpn_i + " successfully for: " + itemNumber, itemNumber);
            return new GenericResponse<string>("No data", status, status ? Message.GetMessage(MessageType.BOM_DElETE_SUCCESS) : Message.GetMessage(MessageType.BOM_DElETE_FAILURE));
        }

        #endregion
    }
}