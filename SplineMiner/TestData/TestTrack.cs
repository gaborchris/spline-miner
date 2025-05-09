using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SplineMiner.TestData
{
    public static class TestTrack
    {
        public static List<Vector2> GetLargeTrackNodes()
        {
            return new List<Vector2>
            {
                // Start with a gentle curve
                new(100, 300),
                new(150, 280),
                new(200, 320),
                new(250, 290),
                new(300, 310),
                
                // First major loop
                new(350, 350),
                new(400, 400),
                new(450, 350),
                new(500, 300),
                new(550, 350),
                new(600, 400),
                new(650, 350),
                
                // Downward slope
                new(700, 300),
                new(750, 250),
                new(800, 200),
                new(850, 150),
                new(900, 100),
                
                // Valley section
                new(950, 150),
                new(1000, 200),
                new(1050, 250),
                new(1100, 200),
                new(1150, 150),
                
                // Upward climb
                new(1200, 200),
                new(1250, 250),
                new(1300, 300),
                new(1350, 350),
                new(1400, 400),
                
                // Second major loop
                new(1450, 450),
                new(1500, 500),
                new(1550, 550),
                new(1600, 500),
                new(1650, 450),
                new(1700, 400),
                new(1750, 450),
                
                // Zigzag section
                new(1800, 400),
                new(1850, 350),
                new(1900, 400),
                new(1950, 350),
                new(2000, 400),
                new(2050, 350),
                new(2100, 400),
                
                // Spiral section
                new(2150, 450),
                new(2200, 500),
                new(2250, 450),
                new(2300, 400),
                new(2350, 450),
                new(2400, 500),
                new(2450, 450),
                new(2500, 400),
                new(2550, 450),
                new(2600, 500),
                
                // Mountain section
                new(2650, 550),
                new(2700, 600),
                new(2750, 650),
                new(2800, 700),
                new(2850, 650),
                new(2900, 600),
                new(2950, 550),
                
                // Downhill run
                new(3000, 500),
                new(3050, 450),
                new(3100, 400),
                new(3150, 350),
                new(3200, 300),
                new(3250, 250),
                new(3300, 200),
                
                // Final curves
                new(3350, 250),
                new(3400, 300),
                new(3450, 250),
                new(3500, 300),
                new(3550, 250),
                new(3600, 300),
                new(3650, 250),
                new(3700, 300),
                
                // End section with loops
                new(3750, 350),
                new(3800, 400),
                new(3850, 350),
                new(3900, 300),
                new(3950, 350),
                new(4000, 400),
                new(4050, 350),
                new(4100, 300),
                new(4150, 350),
                new(4200, 400),
                
                // Final stretch
                new(4250, 350),
                new(4300, 300),
                new(4350, 250),
                new(4400, 200),
                new(4450, 150),
                new(4500, 100),
                new(4550, 150),
                new(4600, 200),
                new(4650, 250),
                new(4700, 300),
                
                // Last loop
                new(4750, 350),
                new(4800, 400),
                new(4850, 350),
                new(4900, 300),
                new(4950, 350),
                new(5000, 400)
            };
        }

        public static List<Vector2> GetSmallTrackNodes()
        {
            return new List<Vector2>
            {
                // Start with a gentle curve
                new(100, 300),
                new(150, 280),
                new(200, 320),
                new(250, 290),
                new(300, 310),
                
                // First loop
                new(350, 350),
                new(400, 400),
                new(450, 350),
                new(500, 300),
                new(550, 350),
                
                // Downward slope
                new(600, 300),
                new(650, 250),
                new(700, 200),
                
                // Valley section
                new(750, 250),
                new(800, 300),
                new(850, 250),
                
                // Upward climb
                new(900, 300),
                new(950, 350),
                new(1000, 400),
                
                // Second loop
                new(1050, 350),
                new(1100, 300),
                new(1150, 350),
                
                // Final stretch
                new(1200, 300),
                new(1250, 250),
                new(1300, 200)
            };
        }
    }
}