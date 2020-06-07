using Ord.WebApi.Entities;
using Ord.WebApi.Helpers.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Ord.WebApi.Data.Contexts
{
    public static class ApplicationDbContextExtensions
    {
        public static void CreateTestData(this ApplicationDbContext context)
        {
            //context.OrdUsers.RemoveRange(context.OrdUsers);
            //context.Restaurants.RemoveRange(context.Restaurants);
            //context.ServiceAreas.RemoveRange(context.ServiceAreas);
            //context.SaveChanges();

            var users = new List<OrdUser>
            {
                new OrdUser
                {
                    FullName = "Andrew Haine",
                    EmailAddress = "aphaine@gmail.com",
                    MobileNumber = "0780796607",
                    OauthId = 1
                },
                new OrdUser
                {
                    FullName = "Ted Brooks",
                    EmailAddress = "ted.brooks@gmail.com",
                    MobileNumber = "0781121223",
                    OauthId = 2
                },
                new OrdUser
                {
                    FullName = "Richard Green",
                    EmailAddress = "richardg@gmail.com",
                    MobileNumber = "0829121458",
                    OauthId = 3
                }
            };

            if (!context.OrdUsers.Any())
            {
                context.OrdUsers.AddRange(users);
                context.SaveChanges();
            }

            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    Name = "Test Restaurant 1",
                    OrdUserId = context.OrdUsers.Where(u => u.OauthId == 2).FirstOrDefault().Id,
                    PhoneNumber = "0112112353",
                    ShopNumber = "22",
                    ShoppingCenterName = "New World Shopping Center",
                    StreetAddress = "122 Long Street",
                    Suburb = "Ennerdale",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    Country = "South Africa",
                    ServiceArea = new ServiceArea
                    {
                        Suburb = "Ennerdale",
                        City = "Johannesburg",
                        Country = "South Africa"
                    }
                },
                new Restaurant
                {
                    Name = "Test Restaurant 2",
                    OrdUserId = context.OrdUsers.Where(u => u.OauthId == 2).FirstOrDefault().Id,
                    PhoneNumber = "0112112355",
                    ShopNumber = "82",
                    ShoppingCenterName = "Flashpoint Shopping Center",
                    StreetAddress = "33 Newton Avenue",
                    Suburb = "Ennerdale",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    Country = "South Africa",
                    ServiceArea = new ServiceArea
                    {
                        Suburb = "Ennerdale",
                        City = "Johannesburg",
                        Country = "South Africa"
                    }
                },
                new Restaurant
                {
                    Name = "Real Food 3",
                    OrdUserId = context.OrdUsers.Where(u => u.OauthId == 2).FirstOrDefault().Id,
                    PhoneNumber = "0112112751",
                    ShopNumber = "17",
                    ComplexName = "People's Center",
                    StreetAddress = "82 Western Road",
                    Suburb = "Ennerdale",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    Country = "South Africa",
                    ServiceArea = new ServiceArea
                    {
                        Suburb = "Ennerdale",
                        City = "Johannesburg",
                        Country = "South Africa"
                    }
                },
                new Restaurant
                {
                    Name = "Best Restaurant 5",
                    OrdUserId = context.OrdUsers.Where(u => u.OauthId == 2).FirstOrDefault().Id,
                    PhoneNumber = "0112122356",
                    ShopNumber = "75",
                    ComplexName = "Real World Complex",
                    StreetAddress = "213 Awesome Street",
                    Suburb = "Lenasia",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    Country = "South Africa",
                    ServiceArea = new ServiceArea
                    {
                        Suburb = "Lenasia",
                        City = "Johannesburg",
                        Country = "South Africa"
                    }
                }
            };

            if (!context.Restaurants.Any())
            {
                context.Restaurants.AddRange(restaurants);
                context.SaveChanges();
            }

            if (!context.PaymentMethods.Any())
            {
                context.PaymentMethods.AddRange(new List<Entities.Shared.PaymentMethod>
                {
                    // In Store Payment
                    new Entities.Shared.PaymentMethod
                    {
                        Name = OrdConstants.INSTOREPAYMENT
                    },
                    // COD Payment
                    new Entities.Shared.PaymentMethod
                    {
                        Name = OrdConstants.CODPAYMENT
                    },
                    // EFT Payment
                    new Entities.Shared.PaymentMethod
                    {
                        Name = OrdConstants.EFTPAYMENT
                    }
                });
                context.SaveChanges();
            }

            if (!context.RestaurantPaymentMethods.Any())
            {
                context.RestaurantPaymentMethods.AddRange(new List<Entities.Shared.RestaurantPaymentMethod>
                {
                    // Restaurant In Store Payment
                    new Entities.Shared.RestaurantPaymentMethod
                    {
                        PaymentMethodId = context.PaymentMethods
                        .Where(p => p.Name == OrdConstants.INSTOREPAYMENT)
                        .FirstOrDefault()
                        .Id,
                        RestaurantId = context.Restaurants
                        .Where(r => r.Name == "Test Restaurant 2")
                        .FirstOrDefault()
                        .Id
                    },
                    // Restaurant COD Payment
                    new Entities.Shared.RestaurantPaymentMethod
                    {
                        PaymentMethodId = context.PaymentMethods
                        .Where(p => p.Name == OrdConstants.CODPAYMENT)
                        .FirstOrDefault()
                        .Id,
                        RestaurantId = context.Restaurants
                        .Where(r => r.Name == "Test Restaurant 2")
                        .FirstOrDefault()
                        .Id
                    },
                    // Restaurant EFT Payment
                    new Entities.Shared.RestaurantPaymentMethod
                    {
                        PaymentMethodId = context.PaymentMethods
                        .Where(p => p.Name == OrdConstants.EFTPAYMENT)
                        .FirstOrDefault()
                        .Id,
                        RestaurantId = context.Restaurants
                        .Where(r => r.Name == "Test Restaurant 2")
                        .FirstOrDefault()
                        .Id
                    }
                });
                context.SaveChanges();
            }

            if (!context.CollectionTypes.Any())
            {
                context.CollectionTypes.AddRange(new List<Entities.Shared.CollectionType>
                {
                    // Pick Up Collection
                    new Entities.Shared.CollectionType
                    {
                        Name = OrdConstants.PICKUPCOLLECTION
                    },
                    // Delivery Collection
                    new Entities.Shared.CollectionType
                    {
                        Name = OrdConstants.DELIVERYCOLLECTION
                    }
                });
                context.SaveChanges();
            }

            if (!context.RestaurantCollectionTypes.Any())
            {
                context.RestaurantCollectionTypes.AddRange(new List<Entities.Shared.RestaurantCollectionType>
                {
                    // Restaurant Pick Up Collection
                    new Entities.Shared.RestaurantCollectionType
                    {
                        CollectionTypeId = context.CollectionTypes
                        .Where(c => c.Name == OrdConstants.PICKUPCOLLECTION)
                        .FirstOrDefault()
                        .Id,
                        RestaurantId = context.Restaurants
                        .Where(r => r.Name == "Test Restaurant 2")
                        .FirstOrDefault()
                        .Id
                    },
                    // Restaurant Delivery Collection
                    new Entities.Shared.RestaurantCollectionType
                    {
                        CollectionTypeId = context.CollectionTypes
                        .Where(c => c.Name == OrdConstants.DELIVERYCOLLECTION)
                        .FirstOrDefault()
                        .Id,
                        RestaurantId = context.Restaurants
                        .Where(r => r.Name == "Test Restaurant 2")
                        .FirstOrDefault()
                        .Id
                    }
                });
                context.SaveChanges();
            }
        }
    }
}