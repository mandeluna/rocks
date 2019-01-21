using System;

namespace Rocks_for_Food
{
    struct BiomeCell
    {
        public double Elevation;
        public double Moisture;
        public double Geology;
    }

    class Biome
    {
        readonly BiomeCell[,] cells;
        readonly int height;
        readonly int width;
        FastNoise gen1, gen2, gen3;

        public enum Climate
        {
            OCEAN, BEACH,
            SCORCHED, BARE, TUNDRA, SNOW,
            TEMPERATE_DESERT, SHRUBLAND, TAIGA,
            GRASSLAND, TEMPERATE_DECIDUOUS_FOREST, TEMPERATE_RAIN_FOREST,
            SUBTROPICAL_DESERT, TROPICAL_SEASONAL_FOREST, TROPICAL_RAIN_FOREST
        }

        public enum Geology
        {
            SAND, GRAVEL, SALT, TIN, COPPER, IRON, SILVER, GOLD
        }

        public Biome(int width, int height)
        {
            this.height = height;
            this.width = width;
            cells = new BiomeCell[width, height];
            Generate();
        }

        public BiomeCell this[int x, int y]
        {
            get
            {
                return cells[x, y];
            }
        }

        public Climate ClimateType(int x, int y)
        {
            double e = cells[x, y].Elevation;
            double m = cells[x, y].Moisture;

            if (e < 0.1) return Climate.OCEAN;
            if (e < 0.12) return Climate.BEACH;

            if (e > 0.8)
            {
                if (m < 0.1) return Climate.SCORCHED;
                if (m < 0.2) return Climate.BARE;
                if (m < 0.5) return Climate.TUNDRA;
                return Climate.SNOW;
            }

            if (e > 0.6)
            {
                if (m < 0.33) return Climate.TEMPERATE_DESERT;
                if (m < 0.66) return Climate.SHRUBLAND;
                return Climate.TAIGA;
            }

            if (e > 0.3)
            {
                if (m < 0.16) return Climate.TEMPERATE_DESERT;
                if (m < 0.50) return Climate.GRASSLAND;
                if (m < 0.83) return Climate.TEMPERATE_DECIDUOUS_FOREST;
                return Climate.TEMPERATE_RAIN_FOREST;
            }

            if (m < 0.16) return Climate.SUBTROPICAL_DESERT;
            if (m < 0.33) return Climate.GRASSLAND;
            if (m < 0.66) return Climate.TROPICAL_SEASONAL_FOREST;
            return Climate.TROPICAL_RAIN_FOREST;
        }

        public Geology GeologyType(int x, int y)
        {
            double g = cells[x, y].Geology;

            // each is less likely than the next SAND, GRAVEL, MARBLE, GRANITE, SALT, TIN, COPPER, IRON, SILVER, GOLD

            if (g > 0.98) return Geology.GOLD;
            if (g > 0.95) return Geology.SILVER;
            if (g > 0.90) return Geology.IRON;
            if (g > 0.85) return Geology.COPPER;
            if (g > 0.80) return Geology.TIN;
            if (g > 0.75) return Geology.SALT;
            if (g > 0.50) return Geology.GRAVEL;

            return Geology.SAND;
        }

        // convert range of noise values from [-1.0, 1.0] to [0, 1.0]
        private double Normalize(double noiseValue)
        {
            return noiseValue / 2.0 + 0.5;
        }

        private void Generate()
        {
            int seed1 = 317955;
            Random rng = new Random(seed1);
            gen1 = new FastNoise(rng.Next());
            gen2 = new FastNoise(rng.Next());
            gen3 = new FastNoise(rng.Next());
            gen1.SetFrequency(1.00f);
            gen2.SetFrequency(3.00f);
            gen3.SetFrequency(11.0f);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    float nx = x / (float)width + 0.5f;
                    float ny = y / (float)height + 0.5f;

                    var e = (1.00 * Normalize(gen1.GetNoise(1.0f * nx, 1.0f * ny))
                           + 0.50 * Normalize(gen1.GetNoise(2.0f * nx, 2.0f * ny))
                           + 0.25 * Normalize(gen1.GetNoise(4.0f * nx, 4.0f * ny))
                           + 0.13 * Normalize(gen1.GetNoise(8.0f * nx, 8.0f * ny))
                           + 0.06 * Normalize(gen1.GetNoise(16.0f * nx, 16.0f * ny))
                           + 0.03 * Normalize(gen1.GetNoise(32.0f * nx, 32.0f * ny)));
                    e /= (1.00 + 0.50 + 0.25 + 0.13 + 0.06 + 0.03);
                    e = Math.Pow(e, 2.75);
                    var m = (1.00 * Normalize(gen2.GetNoise(1.0f * nx, 1.0f * ny))
                           + 0.75 * Normalize(gen2.GetNoise(2.0f * nx, 2.0f * ny))
                           + 0.33 * Normalize(gen2.GetNoise(4.0f * nx, 4.0f * ny))
                           + 0.33 * Normalize(gen2.GetNoise(8.0f * nx, 8.0f * ny))
                           + 0.33 * Normalize(gen2.GetNoise(16.0f * nx, 16.0f * ny))
                           + 0.50 * Normalize(gen2.GetNoise(32.0f * nx, 32.0f * ny)));
                    m /= (1.00 + 0.75 + 0.33 + 0.33 + 0.33 + 0.50);
                    var g = (0.50 * Normalize(gen3.GetNoise(1.0f * nx, 1.0f * ny))
                           + 0.50 * Normalize(gen3.GetNoise(2.0f * nx, 2.0f * ny))
                           + 0.50 * Normalize(gen3.GetNoise(4.0f * nx, 4.0f * ny))
                           + 0.50 * Normalize(gen3.GetNoise(8.0f * nx, 8.0f * ny))
                           + 0.50 * Normalize(gen3.GetNoise(16.0f * nx, 16.0f * ny))
                           + 0.50 * Normalize(gen3.GetNoise(32.0f * nx, 32.0f * ny)));
                    g /= (0.50 + 0.50 + 0.50 + 0.50 + 0.50 + 0.50);

                    cells[x, y].Elevation = e;
                    cells[x, y].Moisture = m;
                    cells[x, y].Geology = g;
                }
            }

        }
    }
}
