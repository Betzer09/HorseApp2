﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HorseApp2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HorseDatabaseEntities : DbContext
    {
        public HorseDatabaseEntities()
            : base("name=HorseDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblActiveListingPhoto> tblActiveListingPhotos { get; set; }
        public virtual DbSet<tblActiveListing> tblActiveListings { get; set; }
        public virtual DbSet<tblName> tblNames { get; set; }
        public virtual DbSet<tblSire> tblSires { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
    
        public virtual int usp_DeleteActiveListing()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_DeleteActiveListing");
        }
    
        public virtual int usp_DeleteSire(Nullable<long> sireServerId, string name)
        {
            var sireServerIdParameter = sireServerId.HasValue ?
                new ObjectParameter("SireServerId", sireServerId) :
                new ObjectParameter("SireServerId", typeof(long));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_DeleteSire", sireServerIdParameter, nameParameter);
        }
    
        public virtual int usp_InsertActiveListing(string activeListingId, string age, string color, string dam, string sire, string damSire, string description, string firebaseId, string gender, string horseName, Nullable<bool> inFoal, Nullable<decimal> lte, Nullable<System.DateTime> originalDateListed, Nullable<decimal> price, string purchaseListingType, string ranchPhoto, string sellerId, string horseType, Nullable<bool> isSold)
        {
            var activeListingIdParameter = activeListingId != null ?
                new ObjectParameter("ActiveListingId", activeListingId) :
                new ObjectParameter("ActiveListingId", typeof(string));
    
            var ageParameter = age != null ?
                new ObjectParameter("Age", age) :
                new ObjectParameter("Age", typeof(string));
    
            var colorParameter = color != null ?
                new ObjectParameter("Color", color) :
                new ObjectParameter("Color", typeof(string));
    
            var damParameter = dam != null ?
                new ObjectParameter("Dam", dam) :
                new ObjectParameter("Dam", typeof(string));
    
            var sireParameter = sire != null ?
                new ObjectParameter("Sire", sire) :
                new ObjectParameter("Sire", typeof(string));
    
            var damSireParameter = damSire != null ?
                new ObjectParameter("DamSire", damSire) :
                new ObjectParameter("DamSire", typeof(string));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            var firebaseIdParameter = firebaseId != null ?
                new ObjectParameter("FirebaseId", firebaseId) :
                new ObjectParameter("FirebaseId", typeof(string));
    
            var genderParameter = gender != null ?
                new ObjectParameter("Gender", gender) :
                new ObjectParameter("Gender", typeof(string));
    
            var horseNameParameter = horseName != null ?
                new ObjectParameter("HorseName", horseName) :
                new ObjectParameter("HorseName", typeof(string));
    
            var inFoalParameter = inFoal.HasValue ?
                new ObjectParameter("InFoal", inFoal) :
                new ObjectParameter("InFoal", typeof(bool));
    
            var lteParameter = lte.HasValue ?
                new ObjectParameter("Lte", lte) :
                new ObjectParameter("Lte", typeof(decimal));
    
            var originalDateListedParameter = originalDateListed.HasValue ?
                new ObjectParameter("OriginalDateListed", originalDateListed) :
                new ObjectParameter("OriginalDateListed", typeof(System.DateTime));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(decimal));
    
            var purchaseListingTypeParameter = purchaseListingType != null ?
                new ObjectParameter("PurchaseListingType", purchaseListingType) :
                new ObjectParameter("PurchaseListingType", typeof(string));
    
            var ranchPhotoParameter = ranchPhoto != null ?
                new ObjectParameter("RanchPhoto", ranchPhoto) :
                new ObjectParameter("RanchPhoto", typeof(string));
    
            var sellerIdParameter = sellerId != null ?
                new ObjectParameter("SellerId", sellerId) :
                new ObjectParameter("SellerId", typeof(string));
    
            var horseTypeParameter = horseType != null ?
                new ObjectParameter("HorseType", horseType) :
                new ObjectParameter("HorseType", typeof(string));
    
            var isSoldParameter = isSold.HasValue ?
                new ObjectParameter("IsSold", isSold) :
                new ObjectParameter("IsSold", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_InsertActiveListing", activeListingIdParameter, ageParameter, colorParameter, damParameter, sireParameter, damSireParameter, descriptionParameter, firebaseIdParameter, genderParameter, horseNameParameter, inFoalParameter, lteParameter, originalDateListedParameter, priceParameter, purchaseListingTypeParameter, ranchPhotoParameter, sellerIdParameter, horseTypeParameter, isSoldParameter);
        }
    
        public virtual int usp_InsertSire(string name)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_InsertSire", nameParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> usp_RowExists(Nullable<long> row)
        {
            var rowParameter = row.HasValue ?
                new ObjectParameter("row", row) :
                new ObjectParameter("row", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("usp_RowExists", rowParameter);
        }
    
        public virtual int usp_SearchActiveListings(Nullable<bool> typeSearch, Nullable<bool> priceSearch, Nullable<decimal> priceLow, Nullable<decimal> priceHigh, Nullable<bool> sireSearch, Nullable<bool> damSearch, Nullable<bool> damSireSearch, Nullable<bool> genderSearch, Nullable<bool> ageSearch, Nullable<bool> colorSearch, Nullable<bool> lteSearch, Nullable<decimal> lteHigh, Nullable<decimal> lteLow, Nullable<bool> inFoalSearch, Nullable<bool> inFoal, Nullable<int> itemsPerPage, Nullable<int> page, Nullable<bool> orderBy, Nullable<int> orderByType, Nullable<bool> orderByAscOrDesc)
        {
            var typeSearchParameter = typeSearch.HasValue ?
                new ObjectParameter("TypeSearch", typeSearch) :
                new ObjectParameter("TypeSearch", typeof(bool));
    
            var priceSearchParameter = priceSearch.HasValue ?
                new ObjectParameter("PriceSearch", priceSearch) :
                new ObjectParameter("PriceSearch", typeof(bool));
    
            var priceLowParameter = priceLow.HasValue ?
                new ObjectParameter("PriceLow", priceLow) :
                new ObjectParameter("PriceLow", typeof(decimal));
    
            var priceHighParameter = priceHigh.HasValue ?
                new ObjectParameter("PriceHigh", priceHigh) :
                new ObjectParameter("PriceHigh", typeof(decimal));
    
            var sireSearchParameter = sireSearch.HasValue ?
                new ObjectParameter("SireSearch", sireSearch) :
                new ObjectParameter("SireSearch", typeof(bool));
    
            var damSearchParameter = damSearch.HasValue ?
                new ObjectParameter("DamSearch", damSearch) :
                new ObjectParameter("DamSearch", typeof(bool));
    
            var damSireSearchParameter = damSireSearch.HasValue ?
                new ObjectParameter("DamSireSearch", damSireSearch) :
                new ObjectParameter("DamSireSearch", typeof(bool));
    
            var genderSearchParameter = genderSearch.HasValue ?
                new ObjectParameter("GenderSearch", genderSearch) :
                new ObjectParameter("GenderSearch", typeof(bool));
    
            var ageSearchParameter = ageSearch.HasValue ?
                new ObjectParameter("AgeSearch", ageSearch) :
                new ObjectParameter("AgeSearch", typeof(bool));
    
            var colorSearchParameter = colorSearch.HasValue ?
                new ObjectParameter("ColorSearch", colorSearch) :
                new ObjectParameter("ColorSearch", typeof(bool));
    
            var lteSearchParameter = lteSearch.HasValue ?
                new ObjectParameter("LteSearch", lteSearch) :
                new ObjectParameter("LteSearch", typeof(bool));
    
            var lteHighParameter = lteHigh.HasValue ?
                new ObjectParameter("LteHigh", lteHigh) :
                new ObjectParameter("LteHigh", typeof(decimal));
    
            var lteLowParameter = lteLow.HasValue ?
                new ObjectParameter("LteLow", lteLow) :
                new ObjectParameter("LteLow", typeof(decimal));
    
            var inFoalSearchParameter = inFoalSearch.HasValue ?
                new ObjectParameter("InFoalSearch", inFoalSearch) :
                new ObjectParameter("InFoalSearch", typeof(bool));
    
            var inFoalParameter = inFoal.HasValue ?
                new ObjectParameter("InFoal", inFoal) :
                new ObjectParameter("InFoal", typeof(bool));
    
            var itemsPerPageParameter = itemsPerPage.HasValue ?
                new ObjectParameter("ItemsPerPage", itemsPerPage) :
                new ObjectParameter("ItemsPerPage", typeof(int));
    
            var pageParameter = page.HasValue ?
                new ObjectParameter("Page", page) :
                new ObjectParameter("Page", typeof(int));
    
            var orderByParameter = orderBy.HasValue ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(bool));
    
            var orderByTypeParameter = orderByType.HasValue ?
                new ObjectParameter("OrderByType", orderByType) :
                new ObjectParameter("OrderByType", typeof(int));
    
            var orderByAscOrDescParameter = orderByAscOrDesc.HasValue ?
                new ObjectParameter("OrderByAscOrDesc", orderByAscOrDesc) :
                new ObjectParameter("OrderByAscOrDesc", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SearchActiveListings", typeSearchParameter, priceSearchParameter, priceLowParameter, priceHighParameter, sireSearchParameter, damSearchParameter, damSireSearchParameter, genderSearchParameter, ageSearchParameter, colorSearchParameter, lteSearchParameter, lteHighParameter, lteLowParameter, inFoalSearchParameter, inFoalParameter, itemsPerPageParameter, pageParameter, orderByParameter, orderByTypeParameter, orderByAscOrDescParameter);
        }
    
        public virtual ObjectResult<usp_SearchAllSires_Result> usp_SearchAllSires(string name)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_SearchAllSires_Result>("usp_SearchAllSires", nameParameter);
        }
    
        public virtual ObjectResult<usp_SearchAllSiresElastically_Result> usp_SearchAllSiresElastically(string name)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_SearchAllSiresElastically_Result>("usp_SearchAllSiresElastically", nameParameter);
        }
    
        public virtual int usp_SearchByAge()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SearchByAge");
        }
    
        public virtual int usp_SearchByGender(string gender)
        {
            var genderParameter = gender != null ?
                new ObjectParameter("Gender", gender) :
                new ObjectParameter("Gender", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SearchByGender", genderParameter);
        }
    
        public virtual int usp_SearchByHorseType(string type)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SearchByHorseType", typeParameter);
        }
    
        public virtual int usp_searchByPrice(Nullable<decimal> low, Nullable<decimal> high)
        {
            var lowParameter = low.HasValue ?
                new ObjectParameter("Low", low) :
                new ObjectParameter("Low", typeof(decimal));
    
            var highParameter = high.HasValue ?
                new ObjectParameter("High", high) :
                new ObjectParameter("High", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_searchByPrice", lowParameter, highParameter);
        }
    
        public virtual int usp_SearchBySire(string sire)
        {
            var sireParameter = sire != null ?
                new ObjectParameter("Sire", sire) :
                new ObjectParameter("Sire", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SearchBySire", sireParameter);
        }
    
        public virtual int usp_UpdateActiveListing(Nullable<long> activeListingId, Nullable<int> age, string color, string dam, string sire, string damSire, string description, string gender, string horseName, Nullable<bool> inFoal, Nullable<decimal> lte, Nullable<System.DateTime> originalDateListed, Nullable<decimal> price, string purchaseListingType, string ranchPhoto, string sellerId, string horseType)
        {
            var activeListingIdParameter = activeListingId.HasValue ?
                new ObjectParameter("ActiveListingId", activeListingId) :
                new ObjectParameter("ActiveListingId", typeof(long));
    
            var ageParameter = age.HasValue ?
                new ObjectParameter("Age", age) :
                new ObjectParameter("Age", typeof(int));
    
            var colorParameter = color != null ?
                new ObjectParameter("Color", color) :
                new ObjectParameter("Color", typeof(string));
    
            var damParameter = dam != null ?
                new ObjectParameter("Dam", dam) :
                new ObjectParameter("Dam", typeof(string));
    
            var sireParameter = sire != null ?
                new ObjectParameter("Sire", sire) :
                new ObjectParameter("Sire", typeof(string));
    
            var damSireParameter = damSire != null ?
                new ObjectParameter("DamSire", damSire) :
                new ObjectParameter("DamSire", typeof(string));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            var genderParameter = gender != null ?
                new ObjectParameter("Gender", gender) :
                new ObjectParameter("Gender", typeof(string));
    
            var horseNameParameter = horseName != null ?
                new ObjectParameter("HorseName", horseName) :
                new ObjectParameter("HorseName", typeof(string));
    
            var inFoalParameter = inFoal.HasValue ?
                new ObjectParameter("InFoal", inFoal) :
                new ObjectParameter("InFoal", typeof(bool));
    
            var lteParameter = lte.HasValue ?
                new ObjectParameter("Lte", lte) :
                new ObjectParameter("Lte", typeof(decimal));
    
            var originalDateListedParameter = originalDateListed.HasValue ?
                new ObjectParameter("OriginalDateListed", originalDateListed) :
                new ObjectParameter("OriginalDateListed", typeof(System.DateTime));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(decimal));
    
            var purchaseListingTypeParameter = purchaseListingType != null ?
                new ObjectParameter("PurchaseListingType", purchaseListingType) :
                new ObjectParameter("PurchaseListingType", typeof(string));
    
            var ranchPhotoParameter = ranchPhoto != null ?
                new ObjectParameter("RanchPhoto", ranchPhoto) :
                new ObjectParameter("RanchPhoto", typeof(string));
    
            var sellerIdParameter = sellerId != null ?
                new ObjectParameter("SellerId", sellerId) :
                new ObjectParameter("SellerId", typeof(string));
    
            var horseTypeParameter = horseType != null ?
                new ObjectParameter("HorseType", horseType) :
                new ObjectParameter("HorseType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_UpdateActiveListing", activeListingIdParameter, ageParameter, colorParameter, damParameter, sireParameter, damSireParameter, descriptionParameter, genderParameter, horseNameParameter, inFoalParameter, lteParameter, originalDateListedParameter, priceParameter, purchaseListingTypeParameter, ranchPhotoParameter, sellerIdParameter, horseTypeParameter);
        }
    }
}
