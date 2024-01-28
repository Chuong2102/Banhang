using Dapper;
using Microsoft.IdentityModel.Tokens;
using SV20T1080026.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080026.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                        select -1
                    else
                        begin
                            insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                            values(@productName,@productDescription,@supplierID,@categoryID,@unit,@price,@photo,@isSelling);
                            select @@identity;
                        end";
                var parameters = new
                {
                    productName = data.ProductName ?? "",
                    productDescription = data.ProductDescription ?? "",
                    supplierId = data.SupplierId,
                    categoryId = data.CategoryId,
                    unit = data.Unit,
                    price = data.Price,
                    photo = data.Photo,
                    isSelling = data.IsSelling

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        //TODO: Nice xu
        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                                    values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);
                                    select @@identity;";
                var parameters = new
                {
                    ProductID = data.ProductId,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        //TODO: Nice xu
        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"begin
                                insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                                values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);
                                select @@identity;
                            end";
                var parameters = new
                {
                    ProductID = data.ProductId,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        //TODO: Nice xu
        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products
                            where	(@searchValue = N'' and @categoryID = 0 and @supplierID = 0) 
                            or (@supplierID = 0 and CategoryID = @categoryID and @searchValue = N'')
                            or (CategoryID = @categoryID and SupplierID = @supplierID and @searchValue = N'')
                            or (CategoryID = @categoryID and SupplierID = @supplierID and ProductName like @searchValue)
                            or (@categoryID = 0 and SupplierID = @supplierID and @searchValue = N'')
                            or (@categoryID = 0 and SupplierID = @supplierID and ProductName like @searchValue)
                            or (@categoryID = @categoryID and @supplierID = 0 and ProductName like @searchValue)";

                var parameters = 
                    new {
                        searchValue,
                        categoryID,
                        supplierID
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql =
                @"if not exists(select * from OrderDetails where ProductID = @productID)
	            begin
		            DELETE FROM Products WHERE ProductID = @productID
		            DELETE FROM ProductPhotos where ProductID = @productID
		            DELETE FROM ProductAttributes where ProductID = @productID
	            end;";

                var parameters = new { productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @attributeID";

                var parameters = new { attributeID = attributeID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @photoID";

                var parameters = new { photoID = photoID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;

            using (var connection = OpenConnection())
            {
                var sql = "select * from Products where ProductID = @productID";
                var parameters = new { productID = productID };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductAttributes where AttributeID = @attributeID";

                var parameters = new { attributeID = attributeID };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductPhotos where PhotoID = @photoID";
                
                var parameters = new { photoID = photoID };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            throw new NotImplementedException();
        }

        // TODO: 
        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0,
            decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                        (
	                        select	*, row_number() over (order by ProductName) as RowNumber
                            from	Products
	                        where	(@searchValue = N'' and @categoryID = 0 and @supplierID = 0) 
                            or (@supplierID = 0 and CategoryID = @categoryID and @searchValue = N'')
                            or (CategoryID = @categoryID and SupplierID = @supplierID and @searchValue = N'')
                            or (CategoryID = @categoryID and SupplierID = @supplierID and ProductName like @searchValue)
                            or (@categoryID = 0 and SupplierID = @supplierID and @searchValue = N'')
                            or (@categoryID = 0 and SupplierID = @supplierID and ProductName like @searchValue)
                            or (@categoryID = @categoryID and @supplierID = 0 and ProductName like @searchValue)
                        )
                            select * from cte
                            where  (@pageSize = 0 and @categoryID = 0 and @supplierID = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                    categoryID,
                    supplierID
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Product>();
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> listAttributes;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductAttributes where ProductID = @productID";
                var parameters = new { productID = productID };
                listAttributes = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }

            if (listAttributes == null)
                listAttributes = new List<ProductAttribute>();

            return listAttributes;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> listPhotos;

            using (var connection = OpenConnection())
            {
                var sql = "select * from ProductPhotos where ProductID = @productID";
                var parameters = new { productID = productID };
                listPhotos = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }

            if (listPhotos == null)
                listPhotos = new List<ProductPhoto>();

            return listPhotos;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductId <> @ProductID and ProductName = @ProductName)
                                begin
                                    update Products
                                    set     ProductName = @productName,
                                            ProductDescription = @productDescription,
                                            SupplierId = @supplierId,
                                            CategoryId = @categoryId,
                                            Unit = @unit,
                                            Price = @price,
                                            Photo = @photo,
                                            IsSelling = @isSelling
                                    where ProductId = @productID
                                end";
                var parameters = new
                {
                    productId = data.ProductId,
                    productName = data.ProductName ?? "",
                    productDescription = data.ProductDescription ?? "",
                    supplierId = data.SupplierId,
                    categoryId = data.CategoryId,
                    unit = data.Unit,
                    price = data.Price,
                    photo = data.Photo ?? "",
                    isSelling = data.IsSelling,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductAttributes where AttributeID = @AttributeId)
                                select -1
                            else
                                begin
                                    update ProductAttributes
                                    set AttributeValue = @AttributeValue,
                                        AttributeName = @AttributeName,
                                        DisplayOrder = @DisplayOrder
                                    where AttributeID = @AttributeId
                                end";
                var parameters = new
                {
                    AttributeId = data.AttributeId,
                    AttributeName =data.AttributeName,
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductPhotos where PhotoID = @PhotoID)
                                select -1
                            else
                                begin
                                    update ProductPhotos
                                    set Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                    where PhotoID = @PhotoID
                                end";
                var parameters = new
                {
                    PhotoID = data.PhotoId,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
