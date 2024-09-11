﻿using System;
using System.Collections.Generic;

using Xabbo.Ext.Model;

namespace Xabbo.Ext.Services;

public interface IWardrobeRepository
{
    void Initialize();

    IEnumerable<FigureModel> Load();

    bool Insert(FigureModel figure);
    int Insert(IEnumerable<FigureModel> figures);
    bool Update(FigureModel figure);
    int Update(IEnumerable<FigureModel> figures);
    bool Delete(FigureModel figure);
    int Delete(IEnumerable<FigureModel> figures);
}
