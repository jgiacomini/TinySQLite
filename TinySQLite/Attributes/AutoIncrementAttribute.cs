﻿using System;

namespace TinySQLite.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}