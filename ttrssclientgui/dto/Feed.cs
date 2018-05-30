﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ttrssclientgui.dto
{
    public class Feed
    {
        public string feed_url { get; set; }
        public string title { get; set; }
        public string id { get; set; }
        public string unread { get; set; }
        public string has_icon { get; set; }
        public string cat_id { get; set; }
        public string last_updated { get; set; }
        public string order_id { get; set; }
        public ObservableCollection<HeadLine> headline { get; set; }
    }
}
