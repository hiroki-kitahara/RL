﻿using System;
using HK.Framework.Text;
using RL.ActorControllers.AI;
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
        [SerializeField]
        private StringAsset.Finder name;
        public string Name { get { return this.name.Get; } }

        public Texture Texture;

        public Color Color;

        public ActorParameter Parameter;

        [SerializeField]
        private ScriptableAI ai;
        public ScriptableAI ClonedAI { get { return ai.Clone; } }
    }
}
