using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyMix.DatabaseLink;
using PolyMix.DatabaseLink.Models;

namespace CreateOrder
{
    class Program
    {
        static void Main(string[] args)
        {
            var conString = "Server=192.168.0.100;User=oksana;Password=zaza"; // PASSWORD!
            var db = new PolyMixDatabase(conString);
            var orderId = db.CreateOrder(new CreateOrderRequest
            {
                Name = "Тестове замовлення",
                Quantity = 1000,
                FinishDate = DateTime.Now.AddDays(1),
                IsDraft = false,
                KindId = 33,
                RowColor = 16777215,
                OrderState = 10,
                PayState = 3,
                Rate = 10,
                TotalCost = 230,
                OwnCost = 230,
                TotalCostNative = 2300,
                CustomerTotalCost = 230
            });

            Console.WriteLine("Замовлення: " + orderId);

            var itemId = db.CreateProcessItemData(new CreateProcessItemRequest
            {
                OrderId = orderId,
                ProcessId = 11, // Друк
                Enabled = true,
                Description = "Офсет 1 на листе 4+4",
                OwnCost = 142,
                MatCost = 88,
                Cost = 230,
                Part = 1, // Лист 1
                Multiplier = 1,
                IsItemInProfit = true,
                ProductOut = 1000,
                ProductIn = 1150,
                SideCount = 2
            }, new Dictionary<string, object>{
                { "PrintType", 1 },
                { "Pages", 1 },
                { "ColorsA", 4 },
                { "ColorsB", 4 },
                { "Cathegory", 15 },
                { "Price1", 56 },
                { "PriceN", 15 },
                { "PaperFormatX", 500 },
                { "PaperFormatY", 700 },
                { "PaperDensity", 250 },
                { "PaperPrice", null }, 
                { "PaperType", 2 },
                { "PaperCost", 0 },
                { "PaperCourse", 5.5 }, // из Dic_PaperCourse.A1 - не помню используется ли это
                { "PrintPages", 1150 },
                { "PrintPagesWOTech", 1000 },
                { "PrintPagesWPostTech", 1000 },
                { "PrintCost", 112 },
                { "EnabledPrintCost", 112 },
                { "TechAdd", 150 },
                { "FlipPrice", 20 },
                { "ProductBleed", 2 },
                { "OptimSheet", true },
                { "NewPaper", true },
                { "PaperCoef", 10 },
                { "PaperCategory", 1000 },
                { "PaperPriceCategory", 4 },
                { "KGWOTech", 87.5 },

                { "MachNum", 8 },
                { "MachPrice", 11 },
                { "MachCost", 88 },
                { "EnabledMachCost", 88 },
                { "MachCaps", 50000 },
                { "MontagePrice", 0 },
                { "MachEnabled", true },
                { "MontageEnabled", true },
                { "MontageCost", 0 },
                { "SheetNum", 2 },
                { "OutNum", 8 },
                { "OutFormat", 3 },
                { "OutCase", 1 },
                { "OutCaseWithCost", 1 },

                { "PantoneOpacity", 100 }, // думаю это не надо уже
                { "PantoneWeight", 0 }, // думаю это не надо уже

                { "ProtectLakType", 2 },
                { "ProtectLakSideA", true },
                { "ProtectLakSideB", true },
                { "ProtectLakPassNum", 2000 },
                { "ProtectLakCost", 30 },
                { "ProtectLakPrice", 0.015 },
                { "EnabledProtectLakCost", 30 },
            });

            Console.WriteLine("Друк: " + itemId);

            itemId = db.CreateProcessItemData(new CreateProcessItemRequest
            {
                OrderId = orderId,
                ProcessId = 19, // Пакування
                Enabled = true,
                Description = "Упаковка на палету",
                OwnCost = 0,
                Cost = 0,
                Part = 1002, // без привязки
                ProductOut = 1000,
                IsItemInProfit = true
            }, new Dictionary<string, object>
            {
                { "Tirazz", 1000 },
                { "BatchPrice", 5 },
                { "Cathegory", 4 }
            });
            Console.WriteLine("Пакування: " + itemId);

            Console.ReadLine();
        }
    }
}
