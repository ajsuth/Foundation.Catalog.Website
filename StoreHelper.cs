using System;
using System.Web;
using Foundation.BackEnd.Catalog.Catalog.Model;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Foundation.BackEnd.Catalog
{
    public class StoreHelper
    {
        public StoreHelper()
        {
            StorefrontContext = ServiceLocator.ServiceProvider.GetService<IStorefrontContext>();
        }

        private IStorefrontContext StorefrontContext { get; set; }

        public string GetDefaultStoreID()
        {
            return ((Models.CommerceStorefront)StorefrontContext.CurrentStorefront).DefaultStoreID;
        }

        public void SetCookie(string storeID)
        {
            var cookie = new HttpCookie("STORE_ID", storeID);
            // set the cookie's expiration date
            cookie.Expires = DateTime.Now.AddDays(365);
            // set the cookie on client's browser
            HttpContext.Current.Response.Cookies.Remove("STORE_ID");
            HttpContext.Current.Response.SetCookie(cookie);
        }

        public StoresCookie GetCookie()
        {
            var cookieValue = string.Empty;
            if (HttpContext.Current.Request.Cookies["STORE_ID"] != null)
            {
                cookieValue = HttpContext.Current.Request.Cookies.Get("STORE_ID").Value;
            }

            var cookie = cookieValue.Split('|');


            if (cookie.Length < 2)
            {
                return null;
            }

            var storeId = cookie[0].ToString();
            var initialStore = Convert.ToBoolean(cookie[1].ToString());

            return new StoresCookie()
            {
                StoreId = storeId,
                IsInitialStore = initialStore
            };
        }
    }
}