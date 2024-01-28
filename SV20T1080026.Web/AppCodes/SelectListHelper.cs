using SV20T1080026.BusinessLayers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SV20T1080026.Web
{
    public class SelectListHelper
    {
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem(){
                Value = "",
                Text = "-- Chọn tỉnh/thành --"
            });

            foreach(var item in CommonDataService.ListOfProvinces())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }

            return list;
        }

        public static List<SelectListItem> Categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn loại hàng --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfCategories(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }

            return list;
        }

        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn nhà cung cấp --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfSuppliers(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }

            return list;
        }
    }
}
