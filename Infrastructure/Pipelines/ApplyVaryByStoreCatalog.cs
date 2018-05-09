using Foundation.BackEnd.Catalog.Catalog;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using System.Globalization;
using System.Web;
using Foundation.BackEnd.Catalog;

namespace Foundation.Backend.Catalog.Infrastructure.Pipelines
{
	public class ApplyVaryByStoreCatalog : RenderRenderingProcessor
	{
		public ApplyVaryByStoreCatalog()
		{
			StorefrontContext = ServiceLocator.ServiceProvider.GetService<IStorefrontContext>();
		}

		private IStorefrontContext StorefrontContext { get; set; }

		public override void Process(RenderRenderingArgs args)
		{
			if (args.Rendered || HttpContext.Current == null || (!args.Cacheable || args.Rendering.RenderingItem == null))
			{
				return;
			}

			var obj = args.PageContext.Database.GetItem(args.Rendering.RenderingItem.ID);
			if (obj == null || obj["VaryByStoreCatalog"] != "1")
			{
				return;
			}

			var storeHelper = new StoreHelper();
			var storeId = storeHelper.GetCookie()?.StoreId ?? ((BackEnd.Catalog.Models.CommerceStorefront)StorefrontContext.CurrentStorefront).DefaultStoreID;
			if (string.IsNullOrWhiteSpace(args.CacheKey))
			{
				args.CacheKey = string.Format(CultureInfo.InvariantCulture, "_#varyByStoreCatalog_{0}_{1}_{2}_{3}_{4}", Context.Site.Name, Context.Language.Name, HttpContext.Current.Request.Url.AbsoluteUri, args.Rendering.RenderingItem.ID.ToString(), storeId);
			}
			else
			{
				args.CacheKey = string.Format(CultureInfo.InvariantCulture, "_#varybyStoreCatalog_{0}_{1}_{2}", args.CacheKey, args.Rendering.RenderingItem.ID.ToString(), storeId);
			}
		}
	}
}