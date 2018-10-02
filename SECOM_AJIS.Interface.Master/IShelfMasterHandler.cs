using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IShelfMasterHandler
    {
        /// <summary>
        /// Getting shelf information (sp_MA_GetTbm_Shelf)
        /// </summary>
        /// <param name="strShelfNo"></param>
        /// <returns></returns>
        List<tbm_Shelf> GetTbm_Shelf(string strShelfNo);

        /// <summary>
        /// Retrieve shelf data according to search condition (sp_MA_GetShelf)
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <param name="ShelfName"></param>
        /// <param name="ShelfType"></param>
        /// <param name="AreaCode"></param>
        /// <returns></returns>
        List<doShelf> GetShelf(string ShelfNo, string ShelfName, string ShelfType, string AreaCode);

        /// <summary>
        /// Check duplicate shelf no. (sp_MA_CheckDuplicateShelf)
        /// </summary>
        /// <param name="ShelfNo"></param>
        /// <returns></returns>
        bool CheckDuplicateShelf(string ShelfNo);

        /// <summary>
        /// Insert Shelf information (sp_MA_InsertShelf)
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        List<tbm_Shelf> InsertShelf(tbm_Shelf shelf);

        /// <summary>
        /// Update shelf information (sp_MA_UpdateShelf)
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        List<tbm_Shelf> UpdateShelf(tbm_Shelf shelf);

    }
}
