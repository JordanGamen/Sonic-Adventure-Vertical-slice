﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public class Tags
    {
        public const string player = "Player";
        public const string playerCol = "PlayerCol";
        public const string boostPad = "BoostPad";
        public const string canvas = "Canvas";
        public const string mainCamera = "MainCamera";
        public const string hazard = "Hazard";
        public const string item = "Item";
        public const string trigger = "Trigger";
        public const string enemy = "Enemy";
        public const string wave = "Wave";
    }

    public class Inputs
    {
        public const string hori = "Horizontal";
        public const string vert = "Vertical";
        public const string jump = "Jump";
        public const string mouseX = "Mouse X";
        public const string mouseY = "Mouse Y";
        public const string pause = "Pause";
        public const string submit = "Submit";
    }

    public class Trigger
    {
        public const string name_0 = "CameraTrigger01";
        public const string name_1 = "CameraTrigger02";
        public const string name_2 = "CameraTrigger03";
        public const string name_3 = "CameraTrigger04";
    }

    public class Value
    {
        public const float ringSeconds = 8.0f;
    }
}
