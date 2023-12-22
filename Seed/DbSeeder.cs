//using Faker;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TestApiJwt.Models;

//public static class DbSeeder
//{
//    public static void Seed(ApplicationDbContext dbContext)
//    {
//        Check if the database is already seeded

//         Seed user data
//        var faker = new Bogus.Faker();
//        var users = new List<ApplicationUser>();

//        for (int i = 0; i < 200; i++) // Adjust the number of users as needed
//        {
//            var user = new ApplicationUser
//            {
//                UserName = faker.Internet.UserName(),
//                Email = faker.Internet.Email(),
//                FirstName = faker.Name.FirstName(),
//                LastName = faker.Name.LastName(),
//                Add other properties as needed
//            };

//            users.Add(user);
//        }

//        Seed shop data


//        Insert the fake users into the Users table
//        dbContext.Users.AddRange(users);
//        dbContext.SaveChanges();

//        You can similarly seed data for other entities (Shops, Carts, Favorites, etc.)

//       var shopFaker = new Bogus.Faker<Shop>()
//       .RuleFor(s => s.ShopName, f => f.Company.CompanyName())
//       .RuleFor(s => s.ShopLocationLatitude, f => f.Address.Latitude())
//       .RuleFor(s => s.ShopLocationLongitude, f => f.Address.Longitude())
//       .RuleFor(s => s.ShopDescription, f => f.Lorem.Paragraph())
//       .RuleFor(s => s.IsApproved, f => f.Random.Bool())
//       .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

//        var shops = shopFaker.Generate(200); // Adjust the number of shops as needed

//        Insert the fake shops into the Shops table
//        dbContext.Shops.AddRange(shops);
//        dbContext.SaveChanges();

//        var categoryFaker = new Bogus.Faker<Category>()
//          .RuleFor(c => c.CategoryName, f => f.Commerce.Categories(1)[0])
//          .RuleFor(c => c.CategoryDescription, f => f.Lorem.Paragraph())
//          .RuleFor(c => c.ShopId, f => f.PickRandom(shops).ShopId);

//        var categories = categoryFaker.Generate(200);

//        Insert the fake categories into the Categories table
//        dbContext.Categories.AddRange(categories);
//        dbContext.SaveChanges();

//        var productFaker = new Bogus.Faker<Product>()
//           .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
//           .RuleFor(p => p.ProductDescription, f => f.Lorem.Paragraph())
//           .RuleFor(p => p.ProductPrice, f => f.Random.Decimal(1, 1000))
//           .RuleFor(p => p.ProductImage, f => f.Image.PicsumUrl())
//           .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).CategoryId);

//        var products = productFaker.Generate(200);

//        Insert the fake products into the Products table
//        dbContext.Products.AddRange(products);
//        dbContext.SaveChanges();
//        /////////////////
//        Seed cart data
//       var cartFaker = new Bogus.Faker<Cart>()
//           .RuleFor(c => c.UserId, f => f.PickRandom(users).Id)
//           .RuleFor(c => c.TotalPrice, f => f.Random.Decimal(1, 1000));

//        var carts = cartFaker.Generate(50);  // Adjust the number of carts as needed

//        Insert the fake carts into the Carts table
//        dbContext.Carts.AddRange(carts);
//        dbContext.SaveChanges();
//        ////////////////////////////
//        Seed cart item data
//        var cartItemFaker = new Bogus.Faker<CartItem>()
//            .RuleFor(ci => ci.CartId, f => f.PickRandom(carts).CartId)
//            .RuleFor(ci => ci.ProductId, f => f.PickRandom(products).ProductId)
//            .RuleFor(ci => ci.Quantity, f => f.Random.Number(1, 10));  // Adjust the quantity range as needed

//        var cartItems = cartItemFaker.Generate(200);  // Adjust the number of cart items as needed

//        Insert the fake cart items into the CartItems table
//        dbContext.CartItems.AddRange(cartItems);
//        dbContext.SaveChanges();

//        /////
//        Seed order header data
//        var orderHeaderFaker = new Bogus.Faker<OrderHeader>()
//            .RuleFor(oh => oh.UserId, f => f.PickRandom(users).Id)
//            .RuleFor(oh => oh.Title, f => f.Name.Prefix())
//            .RuleFor(oh => oh.FirstName, f => f.Name.FirstName())
//            .RuleFor(oh => oh.LastName, f => f.Name.LastName())
//            .RuleFor(oh => oh.Email, f => f.Internet.Email())
//            .RuleFor(oh => oh.Address, f => f.Address.StreetAddress())
//            .RuleFor(oh => oh.City, f => f.Address.City())
//            .RuleFor(oh => oh.Region, f => f.Address.State())
//            .RuleFor(oh => oh.PhoneNumber, f => f.Phone.PhoneNumber())
//            .RuleFor(oh => oh.paymentMethod, f => f.Lorem.Paragraph());

//        var orderHeaders = orderHeaderFaker.Generate(50);  // Adjust the number of order headers as needed

//        Insert the fake order headers into the OrderHeaders table
//        dbContext.OrderHeaders.AddRange(orderHeaders);
//        dbContext.SaveChanges();
//        //////////////////////////
//        Seed favorite data
//       var favoriteFaker = new Bogus.Faker<Favorite>()
//           .RuleFor(f => f.UserId, f => f.PickRandom(users).Id)
//           .RuleFor(f => f.ShopId, f => f.PickRandom(shops).ShopId);

//        var favorites = favoriteFaker.Generate(50);  // Adjust the number of favorites as needed

//        Insert the fake favorites into the Favorites table
//        dbContext.Favorites.AddRange(favorites);
//        dbContext.SaveChanges();
//        ////////////////////////////
//        Seed advertisement data
//       var advertisementFaker = new Bogus.Faker<Advertisement>()
//           .RuleFor(a => a.ShopId, f => f.PickRandom(shops).ShopId)
//           .RuleFor(a => a.AdDescription, f => f.Lorem.Paragraph());

//        var advertisements = advertisementFaker.Generate(50);  // Adjust the number of advertisements as needed

//        Insert the fake advertisements into the Advertisements table
//        dbContext.Advertisements.AddRange(advertisements);
//        dbContext.SaveChanges();
//        /////////////////////
//        Seed rating data
//       var ratingFaker = new Bogus.Faker<Rating>()
//           .RuleFor(r => r.UserId, f => f.PickRandom(users).Id)
//           .RuleFor(r => r.ShopId, f => f.PickRandom(shops).ShopId)
//           .RuleFor(r => r.Rate, f => f.Random.Int(1, 5));

//        var ratings = ratingFaker.Generate(400);  // Adjust the number of ratings as needed

//        Insert the fake ratings into the Ratings table
//        dbContext.Ratings.AddRange(ratings);
//        dbContext.SaveChanges();




//    }
//}
