﻿using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace RL.ActorControllers
{
    /// <summary>
    /// アクターを構成するのに必要な情報
    /// </summary>
    [Serializable]
    public sealed class ActorSpec
    {
        public Texture Texture;

        public Color Color;

        public ActorParameter Parameter;
    }
}
