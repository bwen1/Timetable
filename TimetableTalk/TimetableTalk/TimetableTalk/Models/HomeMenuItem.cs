﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableTalk.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        Thomas,
        Jacob,
        Bill,
        Hano

    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
