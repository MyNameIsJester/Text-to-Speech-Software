using AudioGuideAPI.Models;

namespace AudioGuideAPI.Database
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Languages.Any())
            {
                var vi = new Language
                {
                    LanguageCode = "vi",
                    DisplayName = "Tiếng Việt",
                    FlagIcon = "/flags/vn.png",
                    IntroText = "Chào mừng bạn đến với hành trình khám phá phố ẩm thực Vĩnh Khánh, Quận 4.",
                    IsDefault = true
                };

                var en = new Language
                {
                    LanguageCode = "en",
                    DisplayName = "English",
                    FlagIcon = "/flags/gb.png",
                    IntroText = "Welcome to the food discovery experience at Vinh Khanh Street, District 4.",
                    IsDefault = false
                };

                context.Languages.AddRange(vi, en);
                await context.SaveChangesAsync();
            }

            if (!context.FoodStalls.Any())
            {
                var vi = context.Languages.First(x => x.LanguageCode == "vi");
                var en = context.Languages.First(x => x.LanguageCode == "en");

                var stall1 = new FoodStall
                {
                    Latitude = 10.7599,
                    Longitude = 106.7043,
                    Radius = 35,
                    ImageUrl = "/images/oc-nuong-mo-hanh.jpg",
                    Address = "Phố ẩm thực Vĩnh Khánh, Phường 8, Quận 4, TP.HCM",
                    PriceRange = "50.000 - 120.000 VND",
                    Priority = 1,
                    MapLink = "https://maps.google.com/?q=10.7599,106.7043",
                    IsActive = true
                };

                stall1.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = vi.Id,
                    Name = "Quán Ốc Nướng Mỡ Hành",
                    Description = "Đây là một điểm dừng nổi bật trên phố ẩm thực Vĩnh Khánh. Quán được nhiều thực khách yêu thích nhờ các món ốc nướng thơm mùi mỡ hành, đậu phộng và nước chấm đậm đà. Không gian buổi tối thường nhộn nhịp, phù hợp với những ai muốn trải nghiệm ẩm thực đường phố Sài Gòn.",
                    TtsScript = "Bạn đang đến quán Ốc Nướng Mỡ Hành, một điểm dừng nổi bật trên phố ẩm thực Vĩnh Khánh. Quán được nhiều thực khách yêu thích nhờ các món ốc nướng thơm mùi mỡ hành, đậu phộng và nước chấm đậm đà. Không gian buổi tối ở đây thường rất nhộn nhịp và phù hợp để trải nghiệm ẩm thực đường phố Sài Gòn.",
                    Specialty = "Ốc nướng mỡ hành, sò điệp nướng, nghêu hấp sả",
                    AudioUrl = null
                });

                stall1.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = en.Id,
                    Name = "Grilled Seafood Stall",
                    Description = "This is one of the notable stops on Vinh Khanh food street. The stall is popular for its grilled shellfish topped with scallion oil, peanuts, and rich dipping sauce. In the evening, the area becomes lively and gives visitors a clear taste of Saigon street food culture.",
                    TtsScript = "You are arriving at the Grilled Seafood Stall, one of the notable stops on Vinh Khanh food street. The stall is popular for grilled shellfish topped with scallion oil, peanuts, and rich dipping sauce. In the evening, the area becomes lively and gives visitors a clear taste of Saigon street food culture.",
                    Specialty = "Grilled snails with scallion oil, grilled scallops, steamed clams with lemongrass",
                    AudioUrl = null
                });

                var stall2 = new FoodStall
                {
                    Latitude = 10.7603,
                    Longitude = 106.7038,
                    Radius = 35,
                    ImageUrl = "/images/cang-cum-rang-me.jpg",
                    Address = "Phố ẩm thực Vĩnh Khánh, Phường 8, Quận 4, TP.HCM",
                    PriceRange = "70.000 - 150.000 VND",
                    Priority = 2,
                    MapLink = "https://maps.google.com/?q=10.7603,106.7038",
                    IsActive = true
                };

                stall2.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = vi.Id,
                    Name = "Quán Càng Cúm Rang Me",
                    Description = "Quán này nổi bật với hương vị chua ngọt đặc trưng của các món sốt me. Càng cúm rang me là món được nhiều người gọi thử khi ghé khu Vĩnh Khánh lần đầu. Món ăn có vị đậm, thơm và rất phù hợp để thưởng thức cùng bạn bè vào buổi tối.",
                    TtsScript = "Bạn đang đến quán Càng Cúm Rang Me. Quán nổi bật với hương vị chua ngọt đặc trưng của các món sốt me. Càng cúm rang me là món được nhiều người gọi thử khi ghé khu Vĩnh Khánh lần đầu. Món ăn có vị đậm, thơm và rất phù hợp để thưởng thức cùng bạn bè vào buổi tối.",
                    Specialty = "Càng cúm rang me, tôm nướng, cua sốt me",
                    AudioUrl = null
                });

                stall2.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = en.Id,
                    Name = "Tamarind Crab Claw Stall",
                    Description = "This stall is known for seafood dishes cooked in tamarind sauce. Tamarind crab claws are often chosen by first-time visitors to Vinh Khanh Street. The flavor is rich, slightly sweet and sour, and works well for a casual evening meal with friends.",
                    TtsScript = "You are arriving at the Tamarind Crab Claw Stall. This stall is known for seafood dishes cooked in tamarind sauce. Tamarind crab claws are often chosen by first-time visitors to Vinh Khanh Street. The flavor is rich, slightly sweet and sour, and works well for a casual evening meal with friends.",
                    Specialty = "Tamarind crab claws, grilled prawns, crab in tamarind sauce",
                    AudioUrl = null
                });

                var stall3 = new FoodStall
                {
                    Latitude = 10.7607,
                    Longitude = 106.7032,
                    Radius = 35,
                    ImageUrl = "/images/hau-nuong-pho-mai.jpg",
                    Address = "Phố ẩm thực Vĩnh Khánh, Phường 8, Quận 4, TP.HCM",
                    PriceRange = "60.000 - 130.000 VND",
                    Priority = 3,
                    MapLink = "https://maps.google.com/?q=10.7607,106.7032",
                    IsActive = true
                };

                stall3.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = vi.Id,
                    Name = "Quán Hàu Nướng Phô Mai",
                    Description = "Đây là một quán quen thuộc với nhiều bạn trẻ khi ghé phố ẩm thực Vĩnh Khánh. Các món hàu nướng có vị béo, thơm và dễ ăn, đặc biệt phù hợp với không khí buổi tối. Nếu muốn thử một món hải sản phổ biến và dễ thưởng thức, đây là lựa chọn đáng cân nhắc.",
                    TtsScript = "Bạn đang đến quán Hàu Nướng Phô Mai, một điểm dừng quen thuộc với nhiều bạn trẻ khi ghé phố ẩm thực Vĩnh Khánh. Các món hàu nướng ở đây có vị béo, thơm và dễ ăn, đặc biệt phù hợp với không khí buổi tối. Nếu muốn thử một món hải sản phổ biến và dễ thưởng thức, đây là lựa chọn đáng cân nhắc.",
                    Specialty = "Hàu nướng phô mai, hàu nướng trứng cút, sò lông nướng",
                    AudioUrl = null
                });

                stall3.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = en.Id,
                    Name = "Cheese-Grilled Oyster Stall",
                    Description = "This is a familiar stop for many young visitors on Vinh Khanh food street. The grilled oyster dishes are rich, creamy, and easy to enjoy, especially at night. If you want to try a popular and approachable seafood dish, this stall is a good choice.",
                    TtsScript = "You are arriving at the Cheese-Grilled Oyster Stall, a familiar stop for many young visitors on Vinh Khanh food street. The grilled oyster dishes are rich, creamy, and easy to enjoy, especially at night. If you want to try a popular and approachable seafood dish, this stall is a good choice.",
                    Specialty = "Cheese-grilled oysters, quail egg oysters, grilled blood cockles",
                    AudioUrl = null
                });

                var stall4 = new FoodStall
                {
                    Latitude = 10.7611,
                    Longitude = 106.7028,
                    Radius = 35,
                    ImageUrl = "/images/bach-tuoc-nuong-sa-te.jpg",
                    Address = "Phố ẩm thực Vĩnh Khánh, Phường 8, Quận 4, TP.HCM",
                    PriceRange = "80.000 - 160.000 VND",
                    Priority = 4,
                    MapLink = "https://maps.google.com/?q=10.7611,106.7028",
                    IsActive = true
                };

                stall4.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = vi.Id,
                    Name = "Quán Bạch Tuộc Nướng Sa Tế",
                    Description = "Quán thu hút thực khách nhờ mùi thơm đặc trưng của hải sản nướng trên bếp than. Bạch tuộc nướng sa tế có vị cay nhẹ, thơm và đậm đà, thường được gọi nhiều vào giờ cao điểm buổi tối. Đây là lựa chọn phù hợp cho những ai thích món nướng có hương vị mạnh.",
                    TtsScript = "Bạn đang đến quán Bạch Tuộc Nướng Sa Tế. Quán thu hút thực khách nhờ mùi thơm đặc trưng của hải sản nướng trên bếp than. Bạch tuộc nướng sa tế có vị cay nhẹ, thơm và đậm đà, thường được gọi nhiều vào giờ cao điểm buổi tối. Đây là lựa chọn phù hợp cho những ai thích món nướng có hương vị mạnh.",
                    Specialty = "Bạch tuộc nướng sa tế, mực nướng, tôm nướng muối ớt",
                    AudioUrl = null
                });

                stall4.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = en.Id,
                    Name = "Spicy Grilled Octopus Stall",
                    Description = "This stall attracts diners with the smoky aroma of seafood grilled over charcoal. The spicy grilled octopus has a light heat, a fragrant smell, and a bold flavor that makes it popular in the evening. It is a good option for visitors who enjoy stronger grilled dishes.",
                    TtsScript = "You are arriving at the Spicy Grilled Octopus Stall. This stall attracts diners with the smoky aroma of seafood grilled over charcoal. The spicy grilled octopus has a light heat, a fragrant smell, and a bold flavor that makes it popular in the evening. It is a good option for visitors who enjoy stronger grilled dishes.",
                    Specialty = "Spicy grilled octopus, grilled squid, chili salt prawns",
                    AudioUrl = null
                });

                var stall5 = new FoodStall
                {
                    Latitude = 10.7615,
                    Longitude = 106.7024,
                    Radius = 35,
                    ImageUrl = "/images/so-diep-nuong.jpg",
                    Address = "Phố ẩm thực Vĩnh Khánh, Phường 8, Quận 4, TP.HCM",
                    PriceRange = "60.000 - 140.000 VND",
                    Priority = 5,
                    MapLink = "https://maps.google.com/?q=10.7615,106.7024",
                    IsActive = true
                };

                stall5.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = vi.Id,
                    Name = "Quán Sò Điệp Nướng",
                    Description = "Quán nổi bật với các món sò điệp nướng được trình bày bắt mắt và có hương vị dễ ăn. Mỡ hành, phô mai và các loại sốt giúp món ăn trở nên thơm béo và hấp dẫn hơn. Đây là điểm dừng phù hợp nếu bạn muốn thử hải sản nướng theo phong cách quen thuộc của khu Vĩnh Khánh.",
                    TtsScript = "Bạn đang đến quán Sò Điệp Nướng. Quán nổi bật với các món sò điệp nướng được trình bày bắt mắt và có hương vị dễ ăn. Mỡ hành, phô mai và các loại sốt giúp món ăn trở nên thơm béo và hấp dẫn hơn. Đây là điểm dừng phù hợp nếu bạn muốn thử hải sản nướng theo phong cách quen thuộc của khu Vĩnh Khánh.",
                    Specialty = "Sò điệp nướng mỡ hành, sò điệp phô mai, nghêu nướng",
                    AudioUrl = null
                });

                stall5.Translations.Add(new FoodStallTranslation
                {
                    LanguageId = en.Id,
                    Name = "Grilled Scallop Stall",
                    Description = "This stall is known for grilled scallop dishes that are both visually appealing and easy to enjoy. Toppings such as scallion oil, cheese, and savory sauces make the dishes richer and more flavorful. It is a suitable stop if you want to explore the familiar grilled seafood style of Vinh Khanh Street.",
                    TtsScript = "You are arriving at the Grilled Scallop Stall. This stall is known for grilled scallop dishes that are both visually appealing and easy to enjoy. Toppings such as scallion oil, cheese, and savory sauces make the dishes richer and more flavorful. It is a suitable stop if you want to explore the familiar grilled seafood style of Vinh Khanh Street.",
                    Specialty = "Grilled scallops with scallion oil, cheese scallops, grilled clams",
                    AudioUrl = null
                });

                context.FoodStalls.AddRange(stall1, stall2, stall3, stall4, stall5);
                await context.SaveChangesAsync();
            }

            if (!context.PlaybackLogs.Any())
            {
                context.PlaybackLogs.Add(new PlaybackLog
                {
                    FoodStallId = 1,
                    LanguageCode = "vi",
                    TriggerType = "GPS",
                    PlayedAt = DateTime.Now.AddMinutes(-10),
                    DurationSeconds = 45
                });

                context.PlaybackLogs.Add(new PlaybackLog
                {
                    FoodStallId = 2,
                    LanguageCode = "en",
                    TriggerType = "QR",
                    PlayedAt = DateTime.Now.AddMinutes(-5),
                    DurationSeconds = 60
                });

                await context.SaveChangesAsync();
            }

            if (!context.QrMappings.Any())
            {
                context.QrMappings.Add(new QrMapping
                {
                    FoodStallId = 1,
                    CodeValue = "STALL-001",
                    IsActive = true
                });

                context.QrMappings.Add(new QrMapping
                {
                    FoodStallId = 2,
                    CodeValue = "STALL-002",
                    IsActive = true
                });

                await context.SaveChangesAsync();
            }

            if (!context.Tours.Any())
            {
                var vi = context.Languages.First(x => x.LanguageCode == "vi");
                var en = context.Languages.First(x => x.LanguageCode == "en");

                var tour = new Tour
                {
                    IsActive = true
                };

                context.Tours.Add(tour);
                await context.SaveChangesAsync();

                context.TourTranslations.AddRange(
                    new TourTranslation
                    {
                        TourId = tour.Id,
                        LanguageId = vi.Id,
                        Name = "Tour Hải Sản Vĩnh Khánh",
                        Description = "Một hành trình mẫu đi qua các quán hải sản nổi bật trên phố ẩm thực Vĩnh Khánh."
                    },
                    new TourTranslation
                    {
                        TourId = tour.Id,
                        LanguageId = en.Id,
                        Name = "Vinh Khanh Seafood Tour",
                        Description = "A sample food tour through highlighted seafood stalls on Vinh Khanh Street."
                    }
                );

                context.TourItems.AddRange(
                    new TourItem
                    {
                        TourId = tour.Id,
                        FoodStallId = 1,
                        OrderIndex = 1
                    },
                    new TourItem
                    {
                        TourId = tour.Id,
                        FoodStallId = 2,
                        OrderIndex = 2
                    },
                    new TourItem
                    {
                        TourId = tour.Id,
                        FoodStallId = 3,
                        OrderIndex = 3
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}