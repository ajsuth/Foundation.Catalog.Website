using Foundation.BackEnd.Catalog.Catalog;
using Foundation.BackEnd.Catalog.Models;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Interfaces;
using Sitecore.Commerce.Engine.Connect.Search;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.ContentSearch.Exceptions;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundation.BackEnd.Catalog.Managers
{
	public class SearchManager : Sitecore.Commerce.XA.Foundation.CommerceEngine.Managers.SearchManager
	{
		public SearchManager(IStorefrontContext storefrontContext)
		  : base(storefrontContext)
		{
		}
		
		public override List<Item> GetNavigationCategories()
		{
			var objList = new List<Item>();

			try
			{
				var index = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>().GetIndex();
				var storeHelper = new StoreHelper();
				var storeId = storeHelper.GetCookie()?.StoreId ?? ((Models.CommerceStorefront)StorefrontContext.CurrentStorefront).DefaultStoreID;
				var idString = ((Models.CommerceStorefront)StorefrontContext.CurrentStorefront).GetStartNavigationCategoryID(storeId).Guid.ToString();
				using (var searchContext = index.CreateSearchContext(SearchSecurityOptions.Default))
				{
					var list = searchContext.GetQueryable<CommerceCategorySearchResultItem>()
											.Where(item => item.SitecoreId == idString)
											.Where(item => item.Language == CurrentLanguageName)
											.Select(p => p).ToList();
					if (list.Count > 0)
					{
						objList = ExtractChildrenCategories(list[0]);
					}
				}
			}
			catch (DirectoryNotFoundException ex)
			{
				Helpers.LogErrorMessage(StorefrontContext.GetSystemMessage("Search Index Directory Not Found Error", true), ex, ex);
			}
			catch (IndexNotFoundException ex)
			{
				Helpers.LogErrorMessage(StorefrontContext.GetSystemMessage("Search Index Not Found Error", true), ex, ex);
			}
			catch
			{
				throw;
			}

			return objList;
		}
	}
}