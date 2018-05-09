using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Data.Items;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Foundation.BackEnd.Catalog.Catalog;

namespace Foundation.BackEnd.Catalog.Models
{
	public class CommerceStorefront : Sitecore.Commerce.XA.Foundation.Common.Models.CommerceStorefront
	{
		public CommerceStorefront(IDomainProvider domainProvider, ICurrencyProvider currencyProvider)
			:base(domainProvider, currencyProvider)
		{
		}
		
		public Item GetStoreCatalog(string storeId)
		{
			var storeCatalogItem = Catalogs.Axes.GetChild(storeId);
			Assert.IsNotNull(storeCatalogItem, "Unable to locate the catalog node for the current store catalog.");

			return storeCatalogItem;
		}
		
		public string DefaultStoreID
		{
			get
			{
				var defaultStoreId = CatalogConfiguration.Fields[CatalogConstants.DataTemplates.CatalogConfiguration.Fields.DefaultStoreID]?.ToString();
				Assert.IsNotNull(defaultStoreId, "Default Store ID Item of the Storefront catalog Configuration cannot be found.");

				return defaultStoreId;
			}
		}

		public string StartNavigationCategoryPath
		{
			get
			{
				var path = CatalogConfiguration[CatalogConstants.DataTemplates.CatalogConfiguration.Fields.StartNavigationCategoryPath]?.ToString();
				Assert.IsNotNull(path, "Storefront Catalog start navigation category path field must be populated.");

				return path;
			}
		}

		public Item Catalogs
		{
			get
			{
				var catalogsItem = Context.Database.GetItem(Sitecore.Commerce.Engine.Connect.CommerceConstants.KnownItemIds.CatalogsItem);
				Assert.IsNotNull(catalogsItem, "Catalogs");

				return catalogsItem;
			}
		}

		public ID GetStartNavigationCategoryID(string storeId)
		{
			if (string.IsNullOrWhiteSpace(storeId))
			{
				storeId = DefaultStoreID;
			}

			var storeCatalogItem = GetStoreCatalog(storeId);
			var navigationPath = $"{storeCatalogItem.Paths.FullPath}/{storeId}-{StartNavigationCategoryPath}";
			var navigationPathItem = Context.Database.GetItem(navigationPath);
			Assert.IsNotNull(navigationPathItem, $"Item not found at {navigationPath}.");

			return navigationPathItem.ID;
		}

		public override Item CatalogItem
		{
			get
			{
				var storeHelper = new StoreHelper();
				var storeId = storeHelper.GetCookie()?.StoreId ?? DefaultStoreID;
				var storeCatalogItem = Catalogs.Axes.GetChild(storeId);
				Assert.IsNotNull(storeCatalogItem, "Unable to locate the catalog node for the current store catalog.");

				return storeCatalogItem;
			}
		}

		public override string Catalog
		{
			get
			{
				// TODO: Remove
				return this.CatalogItem.Name;
			}
		}

	}
}