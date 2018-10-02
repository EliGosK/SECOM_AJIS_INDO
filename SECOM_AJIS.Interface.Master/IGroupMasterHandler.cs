using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IGroupMasterHandler
    {
        List<doGroup> GetGroup(doGroup inputDo);
        List<tbm_Group> InsertGroup(doGroup insertDo);
        bool IsUsedGroupData(string GroupCode);
        List<tbm_Group> UpdateGroup(doGroup updateDo);
        bool CheckDuplicateGroupData(string GroupNameLC, string GroupCode);
        string GenerateGroupCode();
    }
}
