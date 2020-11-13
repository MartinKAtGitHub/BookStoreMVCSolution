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

        public const string Role_User_Individual = "Individual Customer";
        public const string Role_User_Company = "Company Customer";
        public const string Role_User_Administrator = "Admin";
        public const string Role_User_Employee = "Employee";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_InProcess = "Processing";
        public const string Status_Shipped = "Shipped";
        public const string Status_Canceled = "Canceled";
        public const string Status_Refunded = "Refunded";

        public const string PaymentStatus_Pending = "Pending";
        public const string PaymentStatus_Approved = "Approved";
        public const string PaymentStatus_DelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatus_Rejected = "Rejected";

        public const string SessionNameShoppingCart = "Shopping cart session";
        public static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if(quantity < 50)
            {
                return price;
            }
            else
            {
                if(quantity < 100)
                {
                    return price50;
                }
                else
                {
                    return price100;
                }
            }
        }

        public static string ConvertToRawHtml(string source)
        {  
            if(source == null)
            {
                return "No Description";
            }

            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

    }
}
