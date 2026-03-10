namespace Zava.Models;

public static class ProductCatalog
{
    public static readonly IReadOnlyList<Product> All =
    [
        new(
            "paint-white-matte",
            "Interior Wall Paint - White Matte",
            "Premium interior latex paint with smooth matte finish, low VOC.",
            29.99m,
            "paint-white-matte.svg",
            "Paint",
            true,
            ["paint", "interior", "wall", "matte", "white", "latex"]),
        new(
            "wood-stain-cedar",
            "Exterior Wood Stain - Cedar",
            "Weather-resistant wood stain for decks and siding with UV protection.",
            34.99m,
            "wood-stain-cedar.svg",
            "Paint",
            true,
            ["paint", "stain", "cedar", "wood", "deck", "outdoor"]),
        new(
            "cordless-drill-kit",
            "Cordless Drill Kit",
            "18V cordless drill with two batteries, charger, and 25-piece bit set.",
            79.99m,
            "cordless-drill-kit.svg",
            "Power Tools",
            true,
            ["drill", "tool", "cordless", "battery", "power"]),
        new(
            "circular-saw",
            "Circular Saw - 7 1/4 inch",
            "Powerful circular saw for precise cuts in plywood and dimensional lumber.",
            119.99m,
            "circular-saw.svg",
            "Power Tools",
            true,
            ["saw", "circular", "tool", "cut", "woodworking"]),
        new(
            "plywood-sheet",
            "Plywood Sheet - 3/4 inch",
            "High-quality furniture-grade plywood sheet, 4x8 ft, versatile for cabinetry and shelving.",
            49.99m,
            "plywood-sheet.svg",
            "Lumber",
            true,
            ["plywood", "wood", "sheet", "cabinet", "lumber"]),
        new(
            "paint-roller-kit",
            "Painter's Roller Kit",
            "Complete roller kit with roller covers, tray, and extension pole for smooth wall coverage.",
            19.99m,
            "paint-roller-kit.svg",
            "Accessories",
            true,
            ["paint", "roller", "tray", "kit", "wall"]),
        new(
            "garden-hose",
            "Garden Hose - 50 ft",
            "Durable kink-resistant garden hose for outdoor watering and cleanup jobs.",
            24.99m,
            "garden-hose.svg",
            "Outdoor",
            true,
            ["garden", "hose", "outdoor", "watering"]),
        new(
            "toolbox-steel",
            "Steel Toolbox",
            "Heavy-duty steel toolbox with removable tray and lockable latch for secure storage.",
            39.99m,
            "toolbox-steel.svg",
            "Storage",
            true,
            ["toolbox", "storage", "steel", "tools"])
    ];
}