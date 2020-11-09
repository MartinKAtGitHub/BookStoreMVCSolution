using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.Utility
{
    public static class SD
    {
        // The names of the stored procedures, you can check if the names match in the migration file CreateStoredPorcForCoverType
        public const string Proc_CoverType_Create = "usp_CreateCoverType";
        public const string Proc_CoverType_Get = "usp_GetCoverType";
        public const string Proc_CoverType_GetAll = "usp_GetCoverTypes";
        public const string Proc_CoverType_Update = "usp_UpdateCoverType";
        public const string Proc_CoverType_Delete = "usp_DeleteCoverType";
    }
}
