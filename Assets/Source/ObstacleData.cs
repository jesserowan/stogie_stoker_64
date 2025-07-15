// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;

[Flags]
public enum ObstacleGrid
{
    None = 0,
    TopLeft = 1 << 1,
    TopMiddle = 1 << 2,
    TopRight = 1 << 3,
    BottomLeft = 1 << 4,
    BottomMiddle = 1 << 5,
    BottomRight = 1 << 6,
    
    // WallLeft = TopLeft | BottomLeft,
    // WallMiddle = TopMiddle | BottomMiddle,
    // WallRight = TopRight | BottomRight,
    //
    // WallBottom = BottomLeft | BottomMiddle | BottomRight,
    // WallTop = TopLeft | TopMiddle | TopRight,
    //
    // WallMiddleLeft = WallLeft | WallMiddle,
    // WallMiddleRight = WallRight | WallMiddle,
    // WallLeftRight = WallLeft | WallRight,
}