using GPDataLayer.Data.Models;
using GPDataLayer.Data.ViewModels;
using GPUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPBusinessLayer.Business.GPTransaction
{
    public class GPTransactionBL
    {
        private AppLogger _logger = AppLogger.GetInstance();

        #region public methods
        public GPTransactionBL()
        {
        }
        
       public GenericResponse<string> InsertDataToGp(GpTransactionDAO gpTransactionDAO)
        {
            try
            {
                _logger.Debug("GPBusinessLayer.GPTransactionBL@InsertDataToGp",gpTransactionDAO.CODetail.ChangeId);

                if (gpTransactionDAO.CODetail.items.Count > 0)
                {
                    foreach (ItemDAO item in gpTransactionDAO.CODetail.items)
                    {
                        ItemDTO itemDTO = new ItemDTO();
                        var response = itemDTO.CreateItems(item);

                        if (!response.success)
                        {
                            return new GenericResponse<string>("", false, response.message + " Item Number: " + item.name);
                        }

                    }
                    foreach (ItemDAO item in gpTransactionDAO.CODetail.items)
                    {
                        ItemDTO itemDTO = new ItemDTO();
                        var response = itemDTO.CreateTransaction(item);

                        if (!response.success)
                        {
                            return new GenericResponse<string>("", false, response.message + " Item Number: " + item.name);
                        }

                    }
                }
                else
                {
                    return new GenericResponse<string>("", false, "Request cannot be processed.No Items found in request.");
                }
            }catch(Exception ex)
            {
                _logger.Error("GPBusinessLayer.GPTransactionBL@insertDataToGp exception occured:" + ex.ToString(),gpTransactionDAO.CODetail.ChangeId);
                return new GenericResponse<string>("", false, "Exception occured"+ex.ToString()); ;
            }
            return new GenericResponse<string>("", true,"Transaction went successful."); ;
        }
       #endregion

    }
}
