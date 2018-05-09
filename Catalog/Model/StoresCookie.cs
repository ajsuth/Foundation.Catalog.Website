using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foundation.BackEnd.Catalog.Catalog.Model
{
    public class StoresCookie
    {
        public string StoreId { get; set; }
        public bool IsInitialStore { get; internal set; }
    }
}