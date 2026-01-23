using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Product : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("categoryId")]
        public string CategoryId { get; set; } = string.Empty;

        [BsonElement("imei")]
        public string Imei { get; set; } = string.Empty;

        [BsonElement("modelId")]
        public string ModelId { get; set; } = string.Empty;

        [BsonElement("moveStock")]
        public string MoveStock { get; set; } = string.Empty;

        [BsonElement("quantityStock")]
        public int QuantityStock { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("priceDiscount")]
        public decimal PriceDiscount { get; set; }

        [BsonElement("priceTotal")]
        public decimal PriceTotal { get; set; }

        [BsonElement("costPrice")]
        public decimal CostPrice { get; set; }

        [BsonElement("expenseCostPrice")]
        public decimal ExpenseCostPrice { get; set; }
        
        [BsonElement("variations")]
        public List<VariationProduct> Variations {get;set;} = [];
        
        // [BsonElement("serials")]
        // public List<SerialControl> Serials {get;set;} = [];

        [BsonElement("sku")]
        public string Sku { get; set; } = string.Empty;

        [BsonElement("barcode")]
        public string Barcode { get; set; } = string.Empty;

        [BsonElement("unitOfMeasure")]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [BsonElement("descriptionComplet")]
        public string DescriptionComplet { get; set; } = string.Empty;
        
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("subCategory")]
        public string SubCategory { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("ean")]
        public string Ean { get; set; } = string.Empty;

        [BsonElement("minimumStock")]
        public decimal MinimumStock { get; set; }

        [BsonElement("maximumStock")]
        public decimal MaximumStock { get; set; }
        
        [BsonElement("saleWithoutStock")]
        public string SaleWithoutStock { get; set; } = string.Empty;
        
        [BsonElement("hasVariations")]
        public string HasVariations { get; set; } = string.Empty;
        
        [BsonElement("hasSerial")]
        public string HasSerial { get; set; } = string.Empty;
        
        [BsonElement("physicalLocation")]
        public string PhysicalLocation { get; set; } = string.Empty;

        [BsonElement("cost")]
        public decimal Cost { get; set; }
        
        [BsonElement("averageCost")]
        public decimal AverageCost { get; set; }
        
        [BsonElement("minimumPrice")]
        public decimal MinimumPrice { get; set; }
        
        [BsonElement("margin")]
        public decimal Margin { get; set; }
        
        [BsonElement("hasDiscount")]
        public string HasDiscount { get; set; } = string.Empty;

        [BsonElement("ncm")]
        public string Ncm { get; set; } = string.Empty;

        [BsonElement("cest")]
        public decimal Cest { get; set; }

        [BsonElement("cfopIn")]
        public decimal CfopIn { get; set; }

        [BsonElement("cfopOut")]
        public decimal CfopOut { get; set; }
        
        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty;

        [BsonElement("cst")]
        public decimal Cst { get; set; }

        // [BsonElement("cstIcms")]
        // public string CstIcms { get; set; } = string.Empty;

        [BsonElement("icms")]
        public decimal Icms { get; set; }
        
        [BsonElement("pis")]
        public decimal Pis { get; set; }

        // [BsonElement("cstPis")]
        // public string CstPis { get; set; } = string.Empty;

        // [BsonElement("cstCofins")]
        // public decimal CstCofins { get; set; }

        [BsonElement("cofins")]
        public decimal Cofins { get; set; }

        // [BsonElement("cstIpi")]
        // public string CstIpi { get; set; } = string.Empty;

        [BsonElement("ipi")]
        public decimal Ipi { get; set; }

        [BsonElement("ibpt")]
        public decimal Ibpt { get; set; }

        [BsonElement("taxGroup")]
        public string TaxGroup { get; set; } = string.Empty;
    }

    public class VariationProduct
    {
        [BsonElement("barcode")]
        public string Barcode {get;set;} = string.Empty; 

        [BsonElement("variationId")]
        public string VariationId {get;set;} = string.Empty; 

        [BsonElement("variationItemId")]
        public string VariationItemId {get;set;} = string.Empty;

        [BsonElement("stock")]
        public decimal Stock {get;set;} 
    }

    public class SerialControl 
    {
        [BsonElement("serialNumber")]
        public string SerialNumber {get;set;} = string.Empty; 

        [BsonElement("imei1")]
        public string Imei1 {get;set;} = string.Empty; 

        [BsonElement("imei2")]
        public string Imei2 {get;set;} = string.Empty;  
        
        [BsonElement("status")]
        public string Status {get;set;} = string.Empty;  
        
        [BsonElement("individualCost")]
        public decimal IndividualCost {get;set;}  
        
        [BsonElement("individualPrice")]
        public decimal IndividualPrice {get;set;}  
        
        [BsonElement("origin")]
        public string Origin {get;set;} = string.Empty;  
        
        [BsonElement("originDoc")]
        public string OriginDoc {get;set;} = string.Empty;  
        
        [BsonElement("warrantyExpiration")]
        public DateTime? WarrantyExpiration {get;set;}  
        
        [BsonElement("observations")]
        public string Observations {get;set;} = string.Empty;  
    }
}