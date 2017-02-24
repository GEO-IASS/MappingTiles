﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MappingTiles
{
    public abstract class TileSchema
    {
        protected TileSchema()
        {
            this.IsYAxisReversed = false;
            this.ZoomLevels = new Collection<ZoomLevel>();
        }

        public BoundingBox MaxExtent
        {
            get;
            set;
        }

        public Collection<ZoomLevel> ZoomLevels
        {
            get;
            private set;
        }

        public bool IsYAxisReversed
        {
            get;
            set;
        }

        public string Crs
        {
            get;
            set;
        }

        public ZoomLevel GetNearestZoomLevel(double resolution)
        {
            InternalChecker.CheckArrayIsEmptyOrNull(ZoomLevels, "ZoomLevels");

            var orderedZoomLevels = ZoomLevels.OrderByDescending(z => z.Resolution);

            // smaller than smallest
            if (orderedZoomLevels.Last().Resolution > resolution)
            {
                return orderedZoomLevels.Last();
            }

            // bigger than biggest
            if (orderedZoomLevels.First().Resolution < resolution)
            {
                return orderedZoomLevels.First();
            }

            ZoomLevel result = null;
            double resultDistance = double.MaxValue;
            foreach (var current in orderedZoomLevels)
            {
                double distance = Math.Abs(current.Resolution - resolution);
                if (distance < resultDistance)
                {
                    result = current;
                    resultDistance = distance;
                }
            }

            return result;
        }
    }
}
