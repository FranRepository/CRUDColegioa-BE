﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace CTS_ReturnsApp.Models;

public partial class MaterialCts
{
    public int Itemid { get; set; }

    public int? CtsRetursId { get; set; }

    public string PartNumber { get; set; }

    public decimal? NumberParts { get; set; }

    public DateTime? DateRequested { get; set; }

    public DateTime? Eta { get; set; }

    public DateTime? DayMaterialArrived { get; set; }

    public decimal? Costo { get; set; }

    public string Rga { get; set; }

    public string Chargeback { get; set; }

    public string Dm { get; set; }

    public bool? Active { get; set; }

    public string Comment { get; set; }
}