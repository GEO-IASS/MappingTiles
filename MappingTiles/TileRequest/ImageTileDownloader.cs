﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace MappingTiles
{
    public class ImageTileDownloader : TileDownloader
    {
        private Dictionary<TileSource, AsyncTileRequest> tileRequests;
        private Dispatcher uiDispatcher;
        private TileSchema tileSchema;

        public ImageTileDownloader(Dispatcher uiDispatcher)
            : this(new Wgs84TileSchema(), uiDispatcher)
        {
        }

        public ImageTileDownloader(TileSchema tileSchema, Dispatcher uiDispatcher)
        {
            this.tileSchema = tileSchema;
            this.uiDispatcher = uiDispatcher;

            tileRequests = new Dictionary<TileSource, AsyncTileRequest>();
        }

        public TileSchema TileSchema
        {
            get { return tileSchema; }
        }

        public override void CancelTileDownload(TileSource tileSource)
        {
            AsyncTileRequest tileRequest;
            if (!this.tileRequests.TryGetValue(tileSource, out tileRequest))
            {
                throw new InvalidOperationException(ApplicationMessages.TileInProgressCancel);
            }
            tileRequest.IsAborted = true;
            tileRequest.AbortIfInQueue();
            tileRequests.Remove(tileSource);
        }

        public override void DownloadTile(TileInfo tileInfo, TileSource tileSource, AsyncTileRequestCompletedHandler callback, NetworkPriority networkPriority)
        {
            AsyncTileRequest tileRequest;
            if (this.tileRequests.TryGetValue(tileSource, out tileRequest))
            {
                throw new InvalidOperationException("Multiple concurrent downloads of the same tile is not supported.");
            }
            AsyncTileRequestQueue.Instance.CreateRequest(tileSource.GetUri(tileInfo), networkPriority, callback);
        }

        public override void UpdateTileDownloadPriority(TileSource tileSource, int priority)
        {
            AsyncTileRequest tileRequest;
            if (this.tileRequests.TryGetValue(tileSource, out tileRequest))
            {
                tileRequest.NetworkPriority = (NetworkPriority)priority;
            }
        }
    }
}
